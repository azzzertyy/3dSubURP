using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableLayer;

    private Interactable interactable;

    private void Update()
    {
        RaycastCheck();
    }

    public void InteractInput(InputAction.CallbackContext context)
    {
        if(context.performed && interactable!= null)
        {
            interactable.TriggerEvent();
        }
    }

    private void RaycastCheck()
    {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, interactionRange, interactableLayer))
        {
            if (interactable == null)
            {
                interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    ShowInteractableUI(interactable);
                }
            }
        }
        else
        {
            RemoveUI();
        }
    }

    private void ShowInteractableUI(Interactable interactable)
    {
        float distance = Vector3.Distance(playerCamera.position, interactable.transform.position);
        if (distance <= interactionRange)
        {
            interactable.ShowUI(playerCamera);
        }
        else
        {
            RemoveUI();
        }
    }

    private void RemoveUI()
    {
        if (interactable != null)
        {
            interactable.RemoveUI();
            interactable = null;
        }
    }
}
