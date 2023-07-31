using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [Divider("Input")]
    public InputActionReference sprintReference;
    public InputActionReference lookReference;
    public InputActionReference movementReference;
    public InputActionReference interactReference;

    [Divider("Variables")]
    [Header("Movement")]
    [Range(0,7)]public float normalSpeed = 3f;
    [Range(0, 7)] public float sprintSpeed = 5f;
    [Range(0, 30)] public float smoothMovement = 12f;
    [Range(0, 10)] public float movementStopMultiplier = 2f;

    [Header("Mouse Look")]
    [Range(0, 300)] public float mouseSensitivity = 5f;
    [Range(0, 30)] public float smoothRotation = 2f;

    [Header("Raycast Interact")]
    [Range(0, 5)] public float interactDistance = 2f;

    [Header("Status")]
    [ShowOnly] [SerializeField] bool isSprinting;
    [ReadOnly][SerializeField] Vector2 
        moveInput,
        lookInput;

    [SerializeField][ShowOnly]private float currentSpeed;    
    [ReadOnly][SerializeField]Vector3 moveVelocity = Vector3.zero;
    [ReadOnly] [SerializeField]Vector3 moveTargetVelocity=Vector3.zero;

    [SerializeField][ShowOnly]private float camRotX;
    [ReadOnly] [SerializeField] Vector2 currentLook = Vector2.zero;
    [ReadOnly] [SerializeField] Vector2 lookVelocity = Vector2.zero;

    [Divider("Components")]
    public Transform playerCameraTransform;

    private Rigidbody rb;
    [ShowOnly] public Collider hovered;


    public TMPro.TMP_Text interactionPrompt;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        currentSpeed = normalSpeed;
        camRotX = 0;

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
            if (hovered)
            {
                Debug.Log("Interacted with: " + hovered.gameObject.name);
            }
        };
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
        UpdateHover();
    }

    private void HandleMovement()
    {
        moveInput = movementReference.action.ReadValue<Vector2>();
        float moveHorizontal = -moveInput.x;
        float moveVertical = moveInput.y;

        currentSpeed = isSprinting ? sprintSpeed : normalSpeed;

        Vector3 moveDirection = (transform.forward * moveVertical + transform.right * moveHorizontal).normalized;
        moveTargetVelocity = moveDirection * currentSpeed;

        moveVelocity = Vector3.Lerp(moveVelocity, moveTargetVelocity, 1f / smoothMovement);
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    private void HandleMouseLook()
    {
        lookInput = lookReference.action.ReadValue<Vector2>();

        Vector3 currentRotation = transform.rotation.eulerAngles;

        lookInput = Vector2.Scale(lookInput, Vector2.one * mouseSensitivity * Time.deltaTime);
        float rotationSpeed = 1f / smoothRotation;

        Vector2 smoothLook;
        // the interpolated float result between the two float values
        smoothLook.x = Mathf.Lerp(0, lookInput.x, rotationSpeed);
        smoothLook.y = Mathf.Lerp(0, lookInput.y, rotationSpeed);
        lookVelocity= Vector3.Lerp(lookVelocity, smoothLook, rotationSpeed);
        currentLook += lookVelocity;

        playerCameraTransform.localRotation = Quaternion.AngleAxis(-currentLook.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(currentLook.x, transform.up);
    }

    public void UpdateHover()
    {
        RaycastHit hit;
        hovered = null;
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                hovered = hit.collider;
            }
        }
        interactionPrompt.text = hovered?hovered.name:"";
        Debug.DrawRay(playerCameraTransform.position,playerCameraTransform.forward*interactDistance,hovered?Color.green:Color.red);
    }
}
