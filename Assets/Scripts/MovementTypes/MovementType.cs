using UnityEngine;

public class MovementType : ScriptableObject
{
    [Divider("Fins")]
    [Tooltip("The maximum force applied to the turtle along the given curve")]
    public float maxBoostStrength;
    public float perfectBoostStrength;

    public float boostDuration;
    public float perfectBoostDuration;

    public float boostThreshold;

    [Tooltip("The duration in seconds during which the force is applied along the curve")]
    public float swimDuration = 1.25f;
    [Tooltip("The curve on which the force is applied upto its maximun forceStrength for the duration of swimDuration")]
    public AnimationCurve paddleCurve;

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
}