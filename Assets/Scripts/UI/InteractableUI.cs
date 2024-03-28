using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableUI : MonoBehaviour
{
    public TMP_Text text;
    public Image interactionImage;
    public Transform playerTransform;

    private void Update()
    {
        if(playerTransform != null)
        {
            transform.LookAt(playerTransform);
            transform.Rotate(0, 180, 0);
        }
    }
}
