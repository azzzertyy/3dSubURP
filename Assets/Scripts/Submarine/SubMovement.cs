using Unity.Netcode;
using UnityEngine;

public class SubMovement : NetworkBehaviour
{
    [SerializeField] Movable movable;
    public NetworkVariable<Vector3> submarineAcceleration = new();
    public void Update()
    {
        
        Vector3 acceleration = submarineAcceleration.Value;
        transform.position += acceleration * Time.deltaTime;
        if (!IsServer) return; 
        Vector3 objectVelocity = movable.objectVelocity.Value;
        if(objectVelocity != acceleration)
        {
            movable.objectVelocity.Value = acceleration;
        }
    }
    
    [ServerRpc]
    private void MoveSubmarineServerRpc(Vector3 _moveVector)
    {
        MoveSubmarine(_moveVector);
    }

    private void MoveSubmarine(Vector3 _moveVector)
    {
        transform.position += _moveVector * Time.deltaTime;
    }
}