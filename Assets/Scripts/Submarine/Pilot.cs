using UnityEngine;

public class Pilot : MonoBehaviour
{
    [SerializeField] GameObject submarineObject;
    [SerializeField] Transform pilotPosition;

    private GameObject currentPilot;

    public void WheelInteraction(GameObject player)
    {
        Debug.Log("Interacted");
        if (currentPilot != null)
        {
            return;
        }
        
        if (player.transform.parent == null || player.transform.parent.gameObject != submarineObject)
        {
            return;
        }
        currentPilot = player;

        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.SetSubmarine(submarineObject, pilotPosition.position, transform.rotation);
        
    }
}
