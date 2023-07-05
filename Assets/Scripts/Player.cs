using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public InputActionReference swimReference;
    public InputActionReference movementReference;

    private TurtleController m_turtleController;
    private Vector2 m_movementInput;

    private void OnEnable()
    {
        m_turtleController = GetComponent<TurtleController>();
        InitiateInput();
    }

    private void InitiateInput()
    {
        swimReference.action.performed += ctx => { m_turtleController.Swim(); };
    }

    private void Update()
    {
        m_movementInput = movementReference.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        m_turtleController.Move(m_movementInput);
    }
}