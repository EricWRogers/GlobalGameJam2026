using UnityEngine;
using UnityEngine.InputSystem;

public class WASDPlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3.0f;
    public float sprintMultipler = 2.0f;

    public float jump = 3.0f;
    public float gravity = -9.81f;

    public bool isMoving;
    private Vector3 currentMovement = Vector3.zero;

    public CharacterController cc;
    private Vector3 velocity;

    [SerializeField]
    private InputActionAsset PlayerControls;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private Vector2 moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        moveAction = PlayerControls.FindActionMap("Player").FindAction("Move");
        jumpAction = PlayerControls.FindActionMap("Player").FindAction("Jump");
        sprintAction = PlayerControls.FindActionMap("Player").FindAction("Sprint");

        moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => moveInput = Vector2.zero;

    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();    
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();    
    }
    

    // Update is called once per frame
    void Update()
    {

        HandleMovement();

        Debug.Log("Is the player grounded" + cc.isGrounded);
    }

    void HandleMovement()
    {
        float speedMultipler = sprintAction.ReadValue<float>() > 0 ? sprintMultipler : 1f;

        float verticalSpeed = moveInput.y * walkSpeed * speedMultipler;
        float horizontalSpeed = moveInput.x * walkSpeed * speedMultipler;

        Vector3 horizontalMovement = new Vector3(horizontalSpeed, 0f ,verticalSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;

        HandleGravityAndJumping();

        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;

        cc.Move(currentMovement * Time.deltaTime);

        isMoving = moveInput.y != 0 || moveInput.x != 0;
    }

    void HandleGravityAndJumping()
    {
        if(cc.isGrounded)
        {
            currentMovement.y = -0.5f;

            if(jumpAction.triggered)
            {
                currentMovement.y = Mathf.Sqrt(jump * -2f * gravity);
            }
        }
        else
        {
            currentMovement.y += gravity * Time.deltaTime;
        }
    }
}
