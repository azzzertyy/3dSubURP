using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerStateManager : NetworkBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Jump,
    }

    public NetworkVariable<PlayerState> currentState;
    
    [SerializeField] private AnimationManager animationManager;
    [SerializeField] private MovementManager movementManager;
    
    private Vector2 movementValue;

    public void MovementInput(InputAction.CallbackContext context)
    {

        if (context.phase == InputActionPhase.Started)
        {
            movementValue = context.ReadValue<Vector2>();
            UpdatePlayerStateServerRpc(PlayerState.Walk);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            movementValue = Vector2.zero;
            UpdatePlayerStateServerRpc(PlayerState.Walk);
        }
    }

    public override void OnNetworkSpawn()
    {
        currentState.Value = PlayerState.Idle;
    }

    [ServerRpc]

    public void UpdatePlayerStateServerRpc(PlayerState newState)
    {
        Debug.Log("Updated player state to: " + newState);
        currentState.Value = newState;
    }

    private void Update()
    {
        if(IsClient && IsOwner)
        {
            if(movementValue.magnitude > 0)
            {
                UpdatePlayerStateServerRpc(PlayerState.Walk);
            }
            switch (currentState.Value)
            {
                case PlayerState.Idle:
                    animationManager.UpdatePlayerClient("Walking", false);
                    break;
                case PlayerState.Walk:
                    animationManager.UpdatePlayerClient("Walking", true);
                    break;
                case PlayerState.Jump:
                    break;
                default:
                    break;
            }
        }
    }
}