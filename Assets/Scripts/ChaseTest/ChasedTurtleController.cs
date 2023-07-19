using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChasedTurtleController : MonoBehaviour
{

    [Space]
    public Animator turtleAnimator;

    [Divider("Input")]
    public InputActionReference swimReference;
    public InputActionReference movementReference;

    private Vector2 movementInput;
    private bool swimInput;

    [Divider("Components")]
    public ChaseControl chase;

    [Divider("Parameters")]
    public float maxRotationAngle;
    public float rotationSpeed;

    public float maxSideDistance;
    public float sideMoveSpeed;

    public float maxAdvanceDistance;
    public float advancePushPower;
    public float advancePushDuration;
    public AnimationCurve advancePushCurve;

    public float PredatorSpeedAdvantage;

    [Divider("Stats")]
    [ShowOnly] [SerializeField] private float advanceDistance;
    [ShowOnly] [SerializeField] private float moveSideDistance, pushSideDistance;
    [ShowOnly] [SerializeField] private float rotationAngle;
    private void Start()
    {
        InitiateInput();
    }
    private void InitiateInput()
    {
        swimReference.action.started += ctx =>
        {
            swimInput = true;
        };

        swimReference.action.canceled += ctx =>
        {
            swimInput = false;
        };
    }
    private void Update()
    {
        if(chase.rail.ElapsedTime==0) advanceDistance = 0;

        movementInput = -movementReference.action.ReadValue<Vector2>();

        if (swimInput)
        {
            Push();
            swimInput = false;
        }

        turtleAnimator.SetFloat(Constants.AnimatorRotationZ, 0, 1f, Time.deltaTime);
        turtleAnimator.SetFloat(Constants.AnimatorRotationX, rotationAngle/maxRotationAngle, 1f, Time.deltaTime);
    }
    private void FixedUpdate()
    {
        Move(movementInput);
    }

    public void Move(Vector2 input)
    {
        //Take Input
        moveSideDistance += input.y * sideMoveSpeed * Time.fixedDeltaTime;
        moveSideDistance = Mathf.Clamp(moveSideDistance, -maxSideDistance, maxSideDistance);

        if (input.y != 0) rotationAngle += input.y * rotationSpeed;
        else rotationAngle += -Mathf.Sign(rotationAngle) * rotationSpeed;
        rotationAngle = Mathf.Clamp(rotationAngle,-maxRotationAngle,maxRotationAngle);

        advanceDistance -= PredatorSpeedAdvantage*Time.fixedDeltaTime;

        //Make Vector
        Vector3 sidePosition = Vector3.right * (moveSideDistance + pushSideDistance);
        Vector3 advancePosition = Vector3.forward * advanceDistance;

        Vector3 position = sidePosition + advancePosition;
        transform.localPosition = position;

        Vector3 forward = Quaternion.AngleAxis(rotationAngle, chase.up) * chase.forward;
        Quaternion rotation = Quaternion.LookRotation(forward,chase.up);
        
        transform.rotation = Quaternion.Slerp(transform.rotation,rotation,0.5f);
    }
    public void Push()
    {
        float rotationMargin = rotationAngle / 60;

        float 
            pushValue = advancePushPower * (1 - Mathf.Abs(rotationMargin)),
            sideValue= advancePushPower*rotationMargin;

        Advance(pushValue, sideValue, advancePushDuration);
        turtleAnimator.SetTrigger(Constants.AnimatorPush);
    }

    public void Advance(float valueForward, float valueSide, float duration)
    {
        StartCoroutine(Advance_Execute(valueForward, valueSide, duration));
    }
    public IEnumerator Advance_Execute(float valueForward, float valueSide, float duration)
    {
        float time = 0;
        float
            lastForwardValue = 0,
            lastSideValue = 0;
        while (time < duration)
        {
            advanceDistance -= lastForwardValue;
            pushSideDistance -= lastSideValue;

           float forwardValue = advancePushCurve.Evaluate(time / duration) * valueForward;
           float sideValue = advancePushCurve.Evaluate(time / duration) * valueSide;

            advanceDistance += forwardValue;
            pushSideDistance += sideValue;

            lastForwardValue = forwardValue;
            lastSideValue = sideValue;

            yield return null;
            time += Time.deltaTime;
        }
    }
}
