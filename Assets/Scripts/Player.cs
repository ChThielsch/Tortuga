using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public InputActionReference swimReference;
    public InputActionReference movementReference;

    public float topConstrain;
    public float bottomConstrain;
    public bool holdToSwim;

    private TurtleController m_turtleController;
    private Vector2 m_movementInput;
    private bool m_swimInput;

    private void OnEnable()
    {
        m_turtleController = GetComponent<TurtleController>();
        InitiateInput();
    }

    private void InitiateInput()
    {
        swimReference.action.performed += ctx =>
        {
            m_swimInput = true;
            if (transform.position.y < topConstrain)
            {
                m_turtleController.Swim();
            }
        };

        swimReference.action.canceled += ctx =>
        {
            m_swimInput = false;
        };
    }

    private void Update()
    {
        m_movementInput = movementReference.action.ReadValue<Vector2>();

        if (transform.position.y <= bottomConstrain || (holdToSwim && m_swimInput))
        {
            m_turtleController.Swim();
        }
    }

    private void FixedUpdate()
    {
        m_turtleController.Move(m_movementInput);
    }
}