using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Transform exitPosition;
    
    public void DoorInteraction(GameObject player)
    {
        player.GetComponent<PlayerController>().DoorTeleporation(exitPosition)s;
    }
}
