using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    // Enum for player states
    public enum PlayerState
    {
        Idle,
        Walk
    }
    public NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>(PlayerState.Idle);

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float gravity;

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerInput inputSystem;
    private CharacterController characterController;

    // Variables for tracking player state and movement
    private PlayerState previousState = PlayerState.Idle;
    private Vector2 movementValue;

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

    private void Update()
    {
        if (IsClient && IsOwner)
        {
            HandleState();
        }
        HandleMovement();
        ClientVisuals();
    }

    private void HandleState()
    {
        if (networkPlayerState.Value != PlayerState.Walk && movementValue.magnitude > 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.Walk);
        }
        else if (networkPlayerState.Value != PlayerState.Idle && movementValue.magnitude < 0.1)
        {
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        }
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = playerCamera.transform.forward * movementValue.y + playerCamera.transform.right * movementValue.x;
        moveDirection = moveDirection * Time.deltaTime * moveSpeed;
        
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (IsServer && IsLocalPlayer)
        {
            Move(moveDirection);
        }
        else if (IsClient && IsLocalPlayer)
        {
            MoveServerRpc(moveDirection);
        }
    }

    private void ClientVisuals()
    {
        PlayerState currentState = networkPlayerState.Value;
        if (currentState != previousState)
        {
            switch (currentState)
            {
                case PlayerState.Walk:
                    playerAnimator.SetBool("Walking", true);
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

    [ServerRpc]
    private void MoveServerRpc(Vector3 _input)
    {
        Move(_input);
    }

    private void Move(Vector3 _input)
    {
        characterController.Move(_input);
    }
}
