using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float normalSpeed = 5f;
    public float sprintSpeed = 10f;
    public float smoothMovement = 12f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public float smoothRotation = 12f;

    [Header("Raycast Interact")]
    public float interactDistance = 2f;

    public Transform playerCameraTransform;

    private Rigidbody rb;
    [ShowOnly] public Collider hovered;
    [SerializeField][ShowOnly]private float currentSpeed;
    [SerializeField][ShowOnly]private float camRotX;
    private bool isSprinting;

    public TMPro.TMP_Text interactionPrompt;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        currentSpeed = normalSpeed;
        camRotX = 0;
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
        UpdateHover();
        HandleInteract();
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        isSprinting = Input.GetKey(KeyCode.Space);
        currentSpeed = isSprinting ? sprintSpeed : normalSpeed;

        Vector3 moveDirection = (transform.forward * moveVertical + transform.right * moveHorizontal).normalized;
        Vector3 moveVelocity = moveDirection * currentSpeed;

        Vector3 rbVelocity = rb.velocity;

        rbVelocity.x = Mathf.Lerp(rb.velocity.x, moveVelocity.x, 
            Time.deltaTime * smoothMovement* (moveHorizontal == 0 ? 2 : 1));
        rbVelocity.z = Mathf.Lerp(rb.velocity.z, moveVelocity.z, 
            Time.deltaTime * smoothMovement * (moveVertical == 0 ? 2 : 1));

        rb.velocity = rbVelocity;
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        camRotX = Mathf.Clamp(camRotX + mouseY, -90, 90);

        playerCameraTransform.localRotation = Quaternion.Slerp(playerCameraTransform.localRotation, Quaternion.Euler(-camRotX, 0f, 0f), 
            Time.deltaTime * smoothRotation * (mouseY == 0 ? 2 : 1));
    }

    private void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E)&&hovered)
        {
            Debug.Log("Interacted with: " + hovered.gameObject.name);
        }
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
