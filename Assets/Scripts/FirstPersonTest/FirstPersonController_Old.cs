using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController_Old : MonoBehaviour
{
    [Divider("Input")]
    public InputActionReference sprintReference;
    public InputActionReference lookReference;
    public InputActionReference movementReference;
    public InputActionReference interactReference;

    [Divider("Variables")]
    [Header("Gravity")]
    [SerializeField] private LayerMask groundLayerMask;
    public float fallMultiplier = 2.5f;
    [ShowOnly][SerializeField]private bool isGrounded;
    Vector3 groundNormal;


    [Header("Movement")]
    [Range(0,7)]public float normalSpeed = 3f;
    [Range(0, 7)] public float sprintSpeed = 5f;
    [Range(0, 30)] public float smoothMovement = 12f;
    [Range(0, 10)] public float movementStopMultiplier = 2f;

    [Header("Rotation")]
    //We need seperate speeds for different control schemes once those are implemented bc these are way to high for mouse.
    [Range(1, 1500)] public float rotationSpeed = 1000;
    [Range(1, 1500)] public float sprintRotationSpeed = 1500f;
    [Range(0, 20)] public float smoothRotation = 2f;

    [Header("Interaction")]
    [Range(0, 5)] public float interactDistance = 2f;

    [Header("Status")]
    [ShowOnly] [SerializeField] bool isSprinting;
    [ReadOnly][SerializeField] Vector2 
        moveInput,
        lookInput;

    [ReadOnly][SerializeField]Vector3 moveVelocity = Vector3.zero;
    [ReadOnly] [SerializeField]Vector3 moveTargetVelocity=Vector3.zero;

    [ReadOnly] [SerializeField] Vector3 currentLook = Vector3.zero;

    [Divider("Components")]
    public Transform playerCameraTransform;

    private Rigidbody rb;
    [ShowOnly] public IInteractable hovered;


    public TMPro.TMP_Text interactionPrompt;

    public Vector2 GetMovementInput()
    {
        return moveInput;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();

        InitiateInput();
    }
    private void InitiateInput()
    {
        sprintReference.action.started += ctx =>
        {
            isSprinting = true;
        };

        sprintReference.action.canceled += ctx =>
        {
            isSprinting = false;
        };

        interactReference.action.started += ctx =>
        {
            if (hovered!=null)
            {
                if (hovered.isBlocked())
                    Debug.Log("Can't Interacted with: " + hovered.name);
                else
                {
                    Debug.Log("Interacted with: " + hovered.name);
                    hovered.OnInteract();
                }
            }
        };
    }

    private void Update()
    {
        UpdateHover();
        UpdateGroundCheck();

        HandleMovement();
        HandleMouseLook();
    }

    private void UpdateGroundCheck()
    {
        float groundRaycastDistance = 1.25f;
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundRaycastDistance, groundLayerMask);
        groundNormal = isGrounded ? hit.normal : Vector3.up;

        Debug.DrawRay(transform.position, Vector3.down * groundRaycastDistance, isGrounded ? Color.green : Color.red);
    }

    private void HandleMovement()
    {
        moveInput = movementReference.action.ReadValue<Vector2>();
        float moveHorizontal = -moveInput.x;
        float moveVertical = moveInput.y;

        Vector3 moveDirection = (transform.forward * moveVertical + transform.right * moveHorizontal).normalized;


        float slopeAngle= Vector3.Angle(groundNormal, Vector3.up);
        // Apply movement modification on slopes
        if (slopeAngle > 0f)
        {
            Vector3 slopeDirection = Vector3.Cross(Vector3.Cross(groundNormal, moveDirection), groundNormal).normalized;
            moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized;
        }

        moveTargetVelocity = moveDirection * (isSprinting ? sprintSpeed : normalSpeed);

        moveVelocity = Vector3.Lerp(moveVelocity, moveTargetVelocity, 1f / smoothMovement);

        Vector3 gravVelocity= Vector3.zero;

            gravVelocity= Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        Vector3 totalVelocity = moveVelocity + gravVelocity;

        rb.velocity = totalVelocity;

        Debug.DrawRay(transform.position, rb.velocity, Color.green);
    }
    private void HandleMouseLook()
    {
        lookInput = lookReference.action.ReadValue<Vector2>();

        Vector3 vel = currentLook;
        vel.x= lookInput.x;
        vel.y= lookInput.y;
        vel *= Time.deltaTime * (isSprinting ? sprintRotationSpeed : rotationSpeed);
        currentLook = Vector3.Slerp(currentLook, currentLook+vel, .1f);

        playerCameraTransform.localRotation = Quaternion.AngleAxis(-currentLook.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(currentLook.x, transform.up);
    }

    public void UpdateHover()
    {
        RaycastHit hit;
        hovered = null;
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, interactDistance))
        {
            IInteractable interactable= hit.collider.GetComponent<IInteractable>();
            if (interactable!=null)
            {
                hovered = interactable;
            }
            else hovered = null;
        } else hovered = null;

        interactionPrompt.text = hovered!=null? hovered.Tooltip:"";
        if (hovered != null)
            interactionPrompt.color = hovered.isBlocked() ? new Color(1, 1, 1, 0.5f) : Color.white;
        Debug.DrawRay(playerCameraTransform.position,playerCameraTransform.forward*interactDistance,hovered!=null?Color.green:Color.red);
    }
}
