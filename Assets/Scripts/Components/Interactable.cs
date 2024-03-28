using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireCollider]
public class Interactable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnityEvent interactionEvent;
    [SerializeField] private GameObject uiObjectPrefab;
    [SerializeField] private string interactionText;
    [SerializeField] private Sprite interactionSprite;

    private GameObject currentUIObject;

    public void ShowUI(Transform playerTransform)
    {
        if (uiObjectPrefab != null && currentUIObject == null)
        {
            currentUIObject = Instantiate(uiObjectPrefab, transform);
            InteractableUI interactableUI = currentUIObject.GetComponent<InteractableUI>();
            interactableUI.text.text = interactionText;
            interactableUI.interactionImage.sprite = interactionSprite;
            interactableUI.playerTransform = playerTransform;
        }
    }

    public void RemoveUI()
    {
        if (currentUIObject != null)
        {
            Destroy(currentUIObject);
            currentUIObject = null;
        }
    }

    public void TriggerEvent()
    {
        interactionEvent.Invoke();
    }

    public void Test()
    {
        Debug.Log("test");
    }
}
