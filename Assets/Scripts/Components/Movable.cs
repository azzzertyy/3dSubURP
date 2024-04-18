using Unity.Netcode;
using UnityEngine;

[RequireTrigger]
public class Movable : NetworkBehaviour
{
    public NetworkVariable<Vector3> objectVelocity;
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer && other.CompareTag("Player"))
        {
            NetworkObject networkObject = other.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                // Reparent the player object on the server
                networkObject.TrySetParent(transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return; 
        if(other.CompareTag("Player"))
        {
            NetworkObject networkObject = other.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                // Unparent the player object on the server
                networkObject.TryRemoveParent();
            }
        }
    }
}
