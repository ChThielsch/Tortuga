using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public InputActionReference movementReference;
    public InputActionReference swimReference;
    public InputActionReference boostReference;
    [Space]
    public Animator turtleAnimator;
    [Space]
    public float topConstrain;
    public float bottomConstrain;

    private TurtleController m_turtleController;
    private Vector2 m_movementInput;
    private float m_swimInput;
    private bool m_boostInput;

    private void Start()
    {
        m_turtleController = GetComponent<TurtleController>();
        InitiateInput();
    }

    private void InitiateInput()
    {
        boostReference.action.started += ctx =>
        {
            m_boostInput = true;
            if (transform.position.y < topConstrain)
            {
                m_turtleController.Swim();
            }
        };

        boostReference.action.canceled += ctx =>
        {
            m_boostInput = false;
        };
    }

    private void Update()
    {
        m_movementInput = movementReference.action.ReadValue<Vector2>();
        m_swimInput = swimReference.action.ReadValue<float>();

        if (transform.position.y <= bottomConstrain || (m_boostInput && transform.position.y < topConstrain))
        {
            m_turtleController.Swim();
            turtleAnimator.SetTrigger(Constants.AnimatorPush);
        }

        turtleAnimator.SetFloat(Constants.AnimatorRotationZ, -m_movementInput.y, 1f, Time.deltaTime);
        turtleAnimator.SetFloat(Constants.AnimatorRotationX, m_movementInput.x, 1f, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        m_turtleController.Move(m_swimInput, m_movementInput);
    }
}