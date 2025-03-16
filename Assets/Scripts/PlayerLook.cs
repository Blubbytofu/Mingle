using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform eyes;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera weaponCamera;

    [Header("Player Settings")]
    [SerializeField] private float sensitivityX;
    [SerializeField] private float sensitivityY;
    [SerializeField] private float sensitivityMultiplier;
    [SerializeField] private int fieldOfView;
    [SerializeField] private int weaponFieldOfView;

    private float rawInputX;
    private float rawInputY;
    private float rotationX;
    private float rotationY;

    [Header("Look Limits")]
    [SerializeField] private int maxVerticalLookAngle;

    private void Start()
    {
        playerCamera.fieldOfView = fieldOfView;
        weaponCamera.fieldOfView = weaponFieldOfView;
        // change it to none after done testing
        Cursor.lockState = CursorLockMode.Locked;
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
