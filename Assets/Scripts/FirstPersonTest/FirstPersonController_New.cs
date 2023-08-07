using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController_New:MonoBehaviour
{

    [Divider("Input")]
    public InputActionReference sprintReference;
    public InputActionReference lookReference;
    public InputActionReference movementReference;
    public InputActionReference interactReference;

    private bool isSprinting;
    [HideInInspector]public Vector2 
        lookInput,
        moveInput;

    [Header("Movement")]
    public float MovementSpeed = 1;
    public float smoothMovement = 5;
    public float Gravity = 9.8f;
    private float fallVelocity = 0;
    private Vector3 moveVelocity,targetMoveVelocity;
    CharacterController characterController;

    [Header("Rotation")]
    public float horizontalSpeed = 1f;
    public float verticalSpeed = 1f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    public Transform cam;

    [Header("Interaction")]
    [Range(0, 5)] public float interactDistance = 2f;
    [ShowOnly] public IInteractable hovered;
    public TMPro.TMP_Text interactionPrompt;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
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
            if (hovered != null)
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

    void Update()
    {
        //UpdateInput
        lookInput = lookReference.action.ReadValue<Vector2>();
        moveInput = movementReference.action.ReadValue<Vector2>();

        HandleMovement();
        HandleRotation();

        UpdateHover();
    }

    public void HandleMovement()
    {
        // player movement - forward, backward, left, right
        float horizontal = -moveInput.x * MovementSpeed;
        float vertical = moveInput.y * MovementSpeed;
        targetMoveVelocity =(transform.right * horizontal + transform.forward * vertical);
        moveVelocity = Vector3.Lerp(moveVelocity, targetMoveVelocity, 1f / smoothMovement);
        characterController.Move(moveVelocity * Time.deltaTime);

        // Gravity
        if (characterController.isGrounded)
            fallVelocity = 0;
        else
        {
            fallVelocity -= Gravity * Time.deltaTime;
            characterController.Move(new Vector3(0, fallVelocity, 0));
        }
    }
    public void HandleRotation()
    {
        float mouseX = lookInput.x * horizontalSpeed;
        float mouseY = lookInput.y * verticalSpeed;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60, 45);

        cam.transform.localEulerAngles = new Vector3(xRotation, 0, 0.0f);
        transform.localEulerAngles= new Vector3(0, yRotation, 0.0f);
    }
    public void UpdateHover()
    {
        RaycastHit hit;
        hovered = null;
        if (Physics.Raycast(cam.position, cam.forward, out hit, interactDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                hovered = interactable;
            }
            else hovered = null;
        }
        else hovered = null;

        interactionPrompt.text = hovered != null ? hovered.Tooltip : "";
        if (hovered != null)
            interactionPrompt.color = hovered.isBlocked() ? new Color(1, 1, 1, 0.5f) : Color.white;
        Debug.DrawRay(cam.position, cam.forward * interactDistance, hovered != null ? Color.green : Color.red);
    }
}