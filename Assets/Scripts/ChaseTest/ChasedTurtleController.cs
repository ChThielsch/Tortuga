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

    [Divider("References")]
    public ChaseControl chase;

    [Divider("Parameters")]
    [Header("Rotation [A|D]")]
    [Range(0,30)]public float maxRotationAngle=10;
    [Range(0, 15)] public float rotationSpeed=5;

    [Header("Side Move [A|D]")]
    [Range(0, 4)] public float maxSideDistance=3;
    [Range(0, 15)] public float sideMoveSpeed=8;
    [ShowOnly][SerializeField]private float movementStrength;

    [Header("Advance [Space]")]
    [Range(1, 7)] public float maxAdvanceDistance = 5;
    [Range(1, 7)] public float maxBehindDistance = 5;
    [Space]
    [Range(0, 5)] public float advancePushPower =2;
    [Range(0, 5)] public float advancePushDuration = 1;
    [Tooltip("How will speed be applied. Don't return to 0 to make it gain advance permanently.")]
    public AnimationCurve advancePushCurve;
    public float advancePushCooldown =1;
    [Space]
    [Tooltip("How far does the turtle fall behind per second?")][Range(0,2)] public float advancePassiveDropoff=0.5f;

    [Divider("Stats")]
    [ShowOnly] [SerializeField] private float advancePushCooldownTimer = 0;
    [ShowOnly] [SerializeField] private float advanceDistance;
    [ShowOnly] [SerializeField] private float moveSideDistance, pushSideDistance;
    [ShowOnly] [SerializeField] private float rotationAngle;
    private void Start()
    {
        InitiateInput();
        ResetPosition();
        chase.OnStartChase += ResetPosition;
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

    public void ResetPosition()
    {
        advanceDistance = 0;
        moveSideDistance = 0;
        pushSideDistance = 0;
        rotationAngle = 0;

        advancePushCooldownTimer = advancePushCooldown;
        StopAllCoroutines();

        transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        if(chase.rail.ElapsedTime==0) advanceDistance = 0;

        movementInput = -movementReference.action.ReadValue<Vector2>();

        if (chase.inChase&&swimInput)
        {
            Push();
            swimInput = false;
        }
        advancePushCooldownTimer = Mathf.Min(advancePushCooldownTimer + Time.deltaTime, advancePushCooldown);

        turtleAnimator.SetFloat(Constants.AnimatorRotationZ, 0, 1f, Time.deltaTime);
        turtleAnimator.SetFloat(Constants.AnimatorRotationX, rotationAngle/maxRotationAngle, 1f, Time.deltaTime);
    }
    private void FixedUpdate()
    {
        if(chase.inChase)
        Move(movementInput);
    }

    public void Move(Vector2 input)
    {
        //Take Input
        float i = input.y == 0 ? -Mathf.Sign(movementStrength) * 0.75f : input.y;
        movementStrength = Mathf.Clamp(movementStrength + i * 3 * Time.fixedDeltaTime, -1,1);

        moveSideDistance += sideMoveSpeed * Time.fixedDeltaTime*movementStrength;
        moveSideDistance = Mathf.Clamp(moveSideDistance, -maxSideDistance, maxSideDistance);

        //if (input.y != 0) rotationAngle += input.y * rotationSpeed*Time.fixedDeltaTime;
        //else rotationAngle += -Mathf.Sign(rotationAngle) * rotationSpeed;
        //rotationAngle = Mathf.Clamp(rotationAngle,-maxRotationAngle,maxRotationAngle);

        advanceDistance -= advancePassiveDropoff*Time.fixedDeltaTime;
        advanceDistance = Mathf.Clamp(advanceDistance, -maxBehindDistance, maxAdvanceDistance);

        //Make Vector
        Vector3 sidePosition = Vector3.right * (moveSideDistance + pushSideDistance);
        Vector3 advancePosition = Vector3.forward * advanceDistance;

        Vector3 position = sidePosition + advancePosition;
        transform.localPosition = position;

        //Vector3 forward = Quaternion.AngleAxis(rotationAngle, chase.up) * chase.forward;
        //Quaternion rotation = Quaternion.LookRotation(forward,chase.up);
        
        //transform.rotation = Quaternion.Slerp(transform.rotation,rotation,0.5f);
    }
    public void Push()
    {
        float 
            rotationMargin = rotationAngle / 60,
            cooldownForce= Mathf.Pow(advancePushCooldownTimer / advancePushCooldown,1.5f);

        float 
            pushValue = advancePushPower * (1 - Mathf.Abs(rotationMargin)),
            sideValue= advancePushPower*rotationMargin;

        pushValue *= cooldownForce;

        Advance(pushValue, sideValue, advancePushDuration);
        turtleAnimator.SetTrigger(Constants.AnimatorPush);

        advancePushCooldownTimer = 0;
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
