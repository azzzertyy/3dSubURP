using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireCollider] // Corrected attribute name
public class Interactable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject uiObjectPrefab;
    [SerializeField] private string interactionText;
    [SerializeField] private Sprite interactionSprite;

    [System.Serializable]
    public class GameObjectEvent : UnityEvent<GameObject> {}

    [SerializeField] private GameObjectEvent interactionEvent;

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

    public void TriggerEvent(GameObject player)
    {
        if(interactionEvent == null)
        {
            return;
        }
        interactionEvent.Invoke(player);
    }

    public void Test(GameObject player)
    {
        Debug.Log("test");
    }
}
