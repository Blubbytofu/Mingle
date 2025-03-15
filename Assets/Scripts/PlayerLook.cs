using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform eyes;
    [SerializeField] Camera playerCamera;

    [Header("Player Settings")]
    [SerializeField] float sensitivityX;
    [SerializeField] float sensitivityY;
    [SerializeField] float sensitivityMultiplier;
    [SerializeField] int fieldOfView;

    float rawInputX;
    float rawInputY;
    float rotationX;
    float rotationY;

    [Header("Look Limits")]
    [SerializeField] int maxVerticalLookAngle;

    private void Start()
    {
        playerCamera.fieldOfView = fieldOfView;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    private void Update()
    {
        FreeLook();
    }

    private void FreeLook()
    {
        rawInputX = Input.GetAxisRaw("Mouse X");
        rawInputY = Input.GetAxisRaw("Mouse Y");

        rotationX += rawInputX * sensitivityX * sensitivityMultiplier;
        rotationY -= rawInputY * sensitivityY * sensitivityMultiplier;

        rotationY = Mathf.Clamp(rotationY, -maxVerticalLookAngle, maxVerticalLookAngle);

        orientation.rotation = Quaternion.Euler(0, rotationX, 0);
        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0);
    }

    private void LateUpdate()
    {
        transform.position = eyes.position;
    }
}
