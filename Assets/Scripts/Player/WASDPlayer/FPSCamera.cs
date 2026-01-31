using UnityEngine;
using UnityEngine.InputSystem;

public class FPSCamera : MonoBehaviour
{
    public float sens = 2f;
    private float xRotation = 0f;
    [SerializeField]
    private InputActionAsset PlayerControls;
    private InputAction lookAction;
    private Vector2 lookInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        
        lookAction = PlayerControls.FindActionMap("Player").FindAction("Look");

        lookAction.performed += context => lookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => lookInput = Vector2.zero;
    }

    private void OnEnable()
    {
        lookAction.Enable();
    }

    private void Disable()
    {
        lookAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = lookInput.x * sens;
        float mouseY = lookInput.y * sens;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
