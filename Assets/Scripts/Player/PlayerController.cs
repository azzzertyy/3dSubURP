using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.VisualScripting;

public class PlayerController : NetworkBehaviour
{
    #region Enums
        public enum PlayerState
        {
            Idle,
            Walk,
            Jump,
            Swim,
            Pilot
        }
    #endregion
    #region MovementSettings
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float gravity;
        [SerializeField] private float jumpForce;
        [SerializeField] private float ballastFillRate;
        [SerializeField] private float accelerationSpeed;
        [SerializeField] private float frictionAmount;
        [SerializeField] private float maxBallast;
    #endregion
    #region References
        [Header("References")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Animator playerAnimator;
        [SerializeField] private PlayerInput inputSystem;
        [SerializeField] private BodyMovement bodyMovement;
        private CharacterController characterController;
    #endregion
    #region PrivateVars
        private PlayerState previousState = PlayerState.Idle;
        private Vector2 movementValue;
        private bool isJumping = false;
        private bool isCrouching = false;
        private bool isSwimming = false;
        private float previousGravity;
        private SubMovement subMovement;
        private Vector3 submarineAcceleration;
        private float ballastLevel;
    #endregion

    public NetworkVariable<PlayerState> networkPlayerState = new(PlayerState.Idle);


    public override void OnNetworkSpawn()
    {
        inputSystem.enabled = IsOwner;
        playerCamera.gameObject.SetActive(IsOwner);
        characterController = GetComponent<CharacterController>();
    }

    public void MovementInput(InputAction.CallbackContext context)
    {
        movementValue = context.ReadValue<Vector2>();
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isJumping = true;
        }
        else if (context.canceled)
        {
            isJumping = false;
        }
    }

    public void CrouchInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isCrouching = true;
        }
        else if (context.canceled)
        {
            isCrouching = false;
        }
    }

    private void Update()
    {
        PlayerState currentState = networkPlayerState.Value;
        if (currentState == PlayerState.Pilot)
        {
            SubmarineMovement();
            return;
        }

        if (IsClient && IsOwner)
        {
            HandleState(currentState);
        }
        HandleJump();
        HandleMovement();
        AnimationHandler(currentState);
    }

    private void HandleState(PlayerState currentState)
    {
        if (currentState != PlayerState.Walk && movementValue.magnitude > 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.Walk);
        }
        else if (currentState != PlayerState.Idle && movementValue.magnitude < 0.1)
        {
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        }
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = playerCamera.transform.forward * movementValue.y + playerCamera.transform.right * movementValue.x;
        moveDirection = moveDirection * Time.deltaTime * moveSpeed;

        moveDirection += ClientParentSync(moveDirection);
        
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        Move(moveDirection); // Always move locally

        if (IsOwner)
        {
            MoveClientRpc(transform.position, transform.rotation, moveDirection);
        }
    }

    private Vector3 ClientParentSync(Vector3 moveDirection)
    {
        if(!(IsClient && IsOwner && transform.parent != null)) return Vector3.zero;
        
        Movable movable = transform.parent.GetComponent<Movable>();
        Vector3 objectVelocity = movable.objectVelocity.Value;
        return objectVelocity * Time.deltaTime;
    }

    private void Jump(Vector3 _jumpVector)
    {
        characterController.Move(_jumpVector * Time.deltaTime);
    }

    private void Move(Vector3 _input)
    {
        characterController.Move(_input);
    }

    private void HandleJump()
    {
        if (!isJumping)
        {
            return;
        }
        isJumping = false;
        Vector3 jumpVector = Vector3.up * jumpForce;
        Jump(jumpVector); // Always jump locally
        if (IsOwner)
        {
            JumpClientRpc(jumpVector);
        }
    }

    private void AnimationHandler(PlayerState currentState)
    {
        if (currentState != previousState)
        {
            switch (currentState)
            {
                case PlayerState.Walk:
                    playerAnimator.SetBool("Walking", true);
                    break;
                case PlayerState.Pilot:
                    playerAnimator.SetBool("Pilot", true);
                    break;
                default:
                    playerAnimator.SetBool("Walking", false);
                    break;
            }
            previousState = currentState;
        }
    }

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState newState)
    {
        Debug.Log("Updated player state to: " + newState);
        networkPlayerState.Value = newState;
    }

    [ClientRpc]
    private void MoveClientRpc(Vector3 position, Quaternion rotation, Vector3 input)
    {
        if (!IsLocalPlayer)
        {
            if (IsOwner)
            {
                Vector3 predictedPosition = ClientPredictedPosition(transform.position, input);
                CorrectClientPosition(predictedPosition, position);
            }
            else
            {
                transform.position = position;
            }
            transform.rotation = rotation;
        }
    }

    private Vector3 ClientPredictedPosition(Vector3 currentPosition, Vector3 moveDirection)
    {
        // Predict the future position based on current velocity
        return currentPosition + moveDirection * Time.deltaTime;
    }

    private void CorrectClientPosition(Vector3 predictedPosition, Vector3 serverPosition)
    {
        // Correct the client position based on the server position
        float positionCorrectionThreshold = 0.01f;
        float positionCorrectionSpeed = 10f;

        float errorDistance = Vector3.Distance(predictedPosition, serverPosition);
        if (errorDistance > positionCorrectionThreshold)
        {
            transform.position = serverPosition;
        }
        else
        {
            // Interpolate towards the corrected position
            transform.position = Vector3.Lerp(transform.position, serverPosition, positionCorrectionSpeed * Time.deltaTime);
        }
    }

    [ClientRpc]
    private void JumpClientRpc(Vector3 jumpVector)
    {
        if (!IsLocalPlayer)
        {
            characterController.Move(jumpVector * Time.deltaTime);
        }
    }

    public void SetSubmarine(GameObject _submarine, Vector3 _pilotPosition, Quaternion _lookAt)
    {
        subMovement = _submarine.GetComponent<SubMovement>();
        UpdatePlayerStateServerRpc(PlayerState.Pilot);
        transform.position = _pilotPosition;
        bodyMovement.BeginPiloting(_lookAt);
        previousGravity = gravity;
        gravity = 0;
        characterController.enabled = false;
    }

    public void UnsetSubmarine()
    {
        if(subMovement == null)
        {
            return;
        }
        subMovement = null;
        UpdatePlayerStateServerRpc(PlayerState.Idle);
        bodyMovement.EndPiloting();
        gravity = previousGravity;
        characterController.enabled = true;
    }


    private void SubmarineMovement()
    {
        submarineAcceleration.x -= movementValue.y * accelerationSpeed * Time.deltaTime;

        float ballastInput = (isJumping ? 1 : 0) + (isCrouching ? -1 : 0);
        ballastLevel += ballastInput * ballastFillRate * Time.deltaTime;
        ballastLevel = Mathf.Clamp(ballastLevel, -maxBallast, maxBallast);
        submarineAcceleration.y += ballastLevel * Time.deltaTime;
        if (IsOwner)
        {
            SubmarineTestClientRpc(submarineAcceleration);
        }
    }

    [ClientRpc]
    private void SubmarineTestClientRpc(Vector3 acceleration)
    {
        if (subMovement)
        {
            subMovement.submarineAcceleration.Value = acceleration;
        }
    }

    public void DoorTeleporation(Transform _newPosition)
    {
        //TODO. Disabling and re-enabling the character for telportation has issues with trigger colliders. This means that the player will be stuck parented to the submarine. Bad. Not good.
        characterController.enabled = false;
        transform.position = _newPosition.position;
        characterController.enabled = true;
    }
}