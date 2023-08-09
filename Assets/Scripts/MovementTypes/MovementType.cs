using System.Collections;
using UnityEngine;

public class MovementType : ScriptableObject
{
    [Divider("Fins")]
    [Range(0, 100f), Tooltip("The maximum force applied to the turtle along the given curve")]
    public float maxBoostStrength = 12;
    [Range(0, 100f), Tooltip("The force applied to the turtle if boosted during the perfectBoostDuration")]
    public float perfectBoostStrength = 15;

    [Range(0, 10f), Tooltip("The time in seconds until the maxBoostStrength is reached")]
    public float boostBuildUpDuration = 1.25f;
    [Range(0, 100f), Tooltip("The after the maxBoostStrength is reached where a perfect boost will be applied")]
    public float perfectBoostDuration = 0.25f;

    [Range(0, 100f), Tooltip("The minimum of build up force before it gets applied")]
    public float boostThreshold = 0.2f;

    [Range(0, 10f), Tooltip("The duration in seconds during which the force is applied along the curve")]
    public float boostDuration = 1.25f;
    [Tooltip("The curve on which the force is applied upto its maximun forceStrength for the duration of swimDuration")]
    public AnimationCurve boostCurve;

    [Space]
    [ReadOnly] public bool isBoosting = false;
    [ReadOnly] public bool perfectBoost = false;
    [ReadOnly] public float boostValue = 0f; // Current boost value
    [ReadOnly] public bool swimBlock;
    [ReadOnly] public float currentMaxSpeed;

    /// <summary>
    /// Applies torque to the object based on the input provided.
    /// The torque is calculated to achieve the desired roll, pitch, and yaw angles.
    /// </summary>
    /// <param name="_input">The input vector containing horizontal and vertical values.</param>
    /// <param name="_rigidbody">The Rigidbody component of the object.</param>
    public virtual void ApplyTorque(Vector2 _input, Rigidbody rigidbody)
    {

    }

    /// <summary>
    /// Applies the constant force to the Rigidbody based on swim and movement input.
    /// </summary>
    /// <param name="_swimInput">The swim input value between 0 and 1.</param>
    /// <param name="_movementInput">The movement input vector.</param>
    /// <param name="_rigidbody">The Rigidbody to which the force is applied.</param>
    public virtual void ApplyConstantForce(float _swimInput, Vector2 _movementInput, Rigidbody _rigidbody)
    {

    }

    public IEnumerator BuildUpBoostRoutine()
    {
        isBoosting = true;
        float startTime = Time.time;
        float endTime = startTime + boostBuildUpDuration;

        while (Time.time < endTime)
        {
            float timeRatio = (Time.time - startTime) / boostBuildUpDuration;
            boostValue = Mathf.Lerp(0f, maxBoostStrength, timeRatio);
            yield return null;
        }

        // Ensure the boost value reaches the max at the end
        boostValue = maxBoostStrength;

        startTime = Time.time;
        endTime = startTime + perfectBoostDuration;
        while (Time.time < endTime)
        {
            boostValue = perfectBoostStrength;
            perfectBoost = true;
            yield return null;
        }

        // Ensure the boost value reaches the max at the end
        boostValue = maxBoostStrength;
        perfectBoost = false;
    }

    public virtual IEnumerator BoostRoutine(Transform _originPosition, Transform _targetPosition, Rigidbody _rigidbody)
    {
        yield return null;
    }
}