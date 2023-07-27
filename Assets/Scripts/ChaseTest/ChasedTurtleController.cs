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

    [Header("Side Move [A|D]")]
    [Range(0, 6)] public float maxSideDistance=3;
    [Range(0, 15)] public float sideMoveSpeed=8;
    [ShowOnly] [SerializeField] private float movementStrengthY;
    [ShowOnly] [SerializeField] private float movementStrengthX;

    [Header("Advance [Space]")]
    [Range(1, 12)] public float maxAdvanceDistance = 5;
    [Range(1, 12)] public float maxBehindDistance = 5;
    [Space]
    [Range(0, 5)] public float advancePushPower =2;
    [Range(0, 5)] public float advancePushDuration = 1;
    [Tooltip("How will speed be applied. Don't return to 0 to make it gain advance permanently.")]
    public AnimationCurve advancePushCurve;
    public float advancePushCooldown =1;
    [Range(0,3)] public float 
        advanceDrive = 1f,
        advanceDropoff = 1.5f;

    [Divider("Stats")]
    [ShowOnly] [SerializeField] private float advancePushCooldownTimer = 0;
    [ShowOnly] [SerializeField] public float advanceDistance;
    [ShowOnly] [SerializeField] private float moveSideDistance, pushSideDistance;
    [ShowOnly] [SerializeField] private float rotationAngle;

    public List<ChaseObstacle> obstacles= new List<ChaseObstacle>();

    [ShowOnly]
    [SerializeField]
    float
        lastforcem,
        lastdurationm;
    float obstaclePushForceMultiplier
    {
        get
        {
            if (obstacles.Count == 0) return 1;

            float value = 0;
            foreach (ChaseObstacle o in obstacles)
                value += o.pushForceMultiplier;
            value /= Mathf.Max(1, obstacles.Count);
            lastforcem = value;
            return value;
        }
    }
    float obstaclePushDurationMultiplier
    {
        get
        {
            if (obstacles.Count == 0) return 1;

            float value = 0;
            foreach (ChaseObstacle o in obstacles)
                value += o.pushDurationMultiplier;
            value /= Mathf.Max(1, obstacles.Count);
            lastdurationm = value;
            return value;
        }
    }
    float obstaclePushBoostMultiplier
    {
        get
        {
            if (obstacles.Count == 0) return 1;

            float value = 0;
            foreach (ChaseObstacle o in obstacles)
                value += o.pushBoostMultiplier;
            value /= Mathf.Max(1, obstacles.Count);
            lastdurationm = value;
            return value;
        }
    }
    float obstacleSideSpeedMultiplier
    {
        get
        {
            if (obstacles.Count == 0) return 1;

            float value = 0;
            foreach (ChaseObstacle o in obstacles)
                value += o.sideSpeedMultiplier;
            value /= Mathf.Max(1, obstacles.Count);
            lastdurationm = value;
            return value;
        }
    }

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
        movementInput = -movementReference.action.ReadValue<Vector2>();

        if (chase.inChase&&swimInput)
        {
            Push();
            swimInput = false;
        }
        advancePushCooldownTimer = Mathf.Min(advancePushCooldownTimer + Time.deltaTime, advancePushCooldown);

        turtleAnimator.SetFloat(Constants.AnimatorRotationZ, 0, 1f, Time.deltaTime);
        turtleAnimator.SetFloat(Constants.AnimatorRotationX, movementInput.y*0.5f, 1f, Time.deltaTime);

        if (chase.inChase)
            Move(movementInput);
    }

    public void Move(Vector2 input)
    {
        //Take Input
        float iy = input.y == 0 ? -Mathf.Sign(movementStrengthY) * 0.75f : input.y;
        movementStrengthY = Mathf.Clamp(movementStrengthY + iy * 2 * Time.deltaTime, -1, 1);

        float ix = -input.x<= 0 ? -Mathf.Sign(advanceDropoff) * 0.75f : -input.x;
        movementStrengthX = Mathf.Clamp(movementStrengthX + ix * Time.deltaTime, 0, 1);


        //Apply Values
        moveSideDistance += sideMoveSpeed * Time.deltaTime * movementStrengthY*obstacleSideSpeedMultiplier;
        moveSideDistance = Mathf.Clamp(moveSideDistance, -maxSideDistance, maxSideDistance);

        float gradualX = -input.x * advanceDrive * movementStrengthX;
        gradualX -= advanceDropoff * Mathf.Max(1, advanceDistance / maxAdvanceDistance);
        advanceDistance += gradualX * Time.deltaTime;
        advanceDistance = Mathf.Clamp(advanceDistance, -maxBehindDistance, maxAdvanceDistance * 1.5f);


        //Make Vector
        Vector3 sidePosition = Vector3.right * (moveSideDistance + pushSideDistance);
        Vector3 advancePosition = Vector3.forward * advanceDistance;

        Vector3 position = sidePosition + advancePosition;
        transform.localPosition = position;
    }
    public void Push()
    {
        float 
            rotationMargin = rotationAngle / 60,
            cooldownForce= Mathf.Pow(advancePushCooldownTimer / advancePushCooldown,1.5f);

        float
            pushValue = advancePushPower * (1 - Mathf.Abs(rotationMargin));

        pushValue *= cooldownForce;
        pushValue *= obstaclePushForceMultiplier;

        Advance(pushValue, advancePushDuration*obstaclePushDurationMultiplier);
        turtleAnimator.SetTrigger(Constants.AnimatorPush);

        advancePushCooldownTimer = 0;
    }
    public void Collide(float strength, float duration)
    {
        Debug.Log("Collide");
        StopAllCoroutines();
        Advance(-strength, duration);
    }

    public void Advance(float valueForward, float duration)
    {
        StartCoroutine(Advance_Execute(valueForward, duration));
    }
    public IEnumerator Advance_Execute(float valueForward, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            float forwardValue = advancePushCurve.Evaluate(time / duration) * valueForward;

            advanceDistance +=forwardValue*obstaclePushBoostMultiplier* Time.deltaTime;
            yield return null;
            time += Time.deltaTime;
        }
    }
}
