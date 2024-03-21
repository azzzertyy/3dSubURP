using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private bool isMoving = false;

    private Vector2 turnValue;

    private Rigidbody rb;

    public void TurnInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isMoving = true;
            turnValue = context.ReadValue<Vector2>();
            Debug.Log("Turn Input: " + turnValue);
        }
        else if (context.canceled)
        {
            isMoving = false;
            turnValue = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        PerformMovement();
    }

    private void PerformMovement()
    {
        if(IsOwner && isMoving)
        {
            if(NetworkManager.Singleton.IsServer)
            {
                Debug.Log("perform server movement");
            }
            else
            {
                RequestMovementServerRpc(turnValue);
            }
        }
    }

    [ServerRpc]
    void RequestMovementServerRpc(Vector2 movementVector)
    {
        Debug.Log("Server recieved movement request: " + movementVector);
    }
}
