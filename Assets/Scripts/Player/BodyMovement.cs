using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;

public class BodyMovement : NetworkBehaviour
{
    [Header("Rotation Sensitivity")]
    [Tooltip("Sensitivity for horizontal camera rotation.")]
    public float sensX;
    [Tooltip("Sensitivity for vertical camera rotation.")]
    public float sensY;

    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform camera;
    [SerializeField] private Transform head;
    [SerializeField] private Transform body;
    [SerializeField] private PlayerController playerController;

    [Header("Head Rotation")]
    [Tooltip("Maximum rotation allowed for the head before the body starts to rotate.")]
    [SerializeField] private float maxHeadRotation;

    // Variables to store rotation values
    private float xRotation;
    private float yRotation;

    // Network variables to synchronize head and body rotation across clients
    private NetworkVariable<Quaternion> headRotation = new();
    private NetworkVariable<Quaternion> bodyRotation = new();

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if(IsClient && IsOwner)
        {
            HandleCameraRotation();
            HandleHeadRotation();
            HandleBodyRotation();
        }
        head.rotation = headRotation.Value;
        body.rotation = bodyRotation.Value;
        Debug.DrawRay(camera.position, camera.forward * 10000, Color.green);
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camera.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void HandleBodyRotation()
    {
        float angleY = Mathf.Abs(body.rotation.eulerAngles.y - camera.rotation.eulerAngles.y);

        if (angleY > maxHeadRotation)
        {
            Quaternion newBodyRotation = Quaternion.Euler(body.rotation.eulerAngles.x, camera.rotation.eulerAngles.y, body.rotation.eulerAngles.z);
            UpdateBodyRotationServerRpc(newBodyRotation);
        }
        else if (playerController.networkPlayerState.Value == PlayerController.PlayerState.Walk)
        {
            Quaternion newBodyRotation = Quaternion.Euler(body.rotation.eulerAngles.x, camera.rotation.eulerAngles.y, body.rotation.eulerAngles.z);
            UpdateBodyRotationServerRpc(newBodyRotation);
        }
    }


    private void HandleHeadRotation()
    {
        Quaternion cameraRotation = camera.rotation;
        UpdateHeadRotationServerRpc(cameraRotation);
    }

    [ServerRpc]
    private void UpdateHeadRotationServerRpc(Quaternion newRotation)
    {
        headRotation.Value = newRotation;
    }
    [ServerRpc]
    private void UpdateBodyRotationServerRpc(Quaternion newRotation)
    {
        bodyRotation.Value = newRotation;
    }
}
