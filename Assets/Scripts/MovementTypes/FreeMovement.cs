using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Type/Free")]
public class FreeMovement : MovementType
{
    [Divider("Torque")]
    [Range(0f, 90f), Tooltip("The maximum angle in degrees that the object can roll.\n" +
         "Roll refers to the rotational movement around the forward axis. " +
         "Higher values allow for more pronounced banking left or right.")]
    public float rollMaxAngle = 35f;

    [Range(0f, 90f), Tooltip("The maximum angle in degrees that the object can pitch.\n" +
             "Pitch refers to the rotational movement around the right axis. " +
             "Higher values allow for more significant upward or downward tilting.")]
    public float pitchMaxAngle = 33f;

    [Range(0f, 90f), Tooltip("The maximum angle in degrees that the object can yaw.\n" +
             "Yaw refers to the rotational movement around the vertical axis. " +
             "Higher values allow for more significant left or right turning.")]
    public float yawMaxAngle = 45f;

    [Range(0.001f, 0.1f), Tooltip("The torque applied to achieve the desired roll angle.\n" +
             "Roll torque controls the rotational force for banking left or right.")]
    public float rollTorque = 0.01f;

    [Range(0.001f, 0.1f), Tooltip("The torque applied to achieve the desired pitch angle.\n" +
             "Pitch torque controls the rotational force for tilting the object up or down.")]
    public float pitchTorque = 0.002f;

    [Range(0.01f, 1f), Tooltip("The torque applied to achieve the desired yaw angle.\n" +
             "Yaw torque controls the rotational force for rotating the object horizontally (left or right).")]
    public float yawTorque = 0.14f;

    //to make steering easier (hiting left performs a left curve without pitching up or down, apply the yaw torque mixed between world up and local up
    private const float shareApplyYawToWorldUp = 0.4f;

    [Divider("Force")]
    // Animation curve used to control the force based on swim input.
    [Tooltip("The force curve that determines the applied force based on swim input.")]
    public AnimationCurve forceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    // The maximum speed the Rigidbody can reach when swimming at full intensity.
    [Range(0, 100), Tooltip("The maximum speed the object can reach when swimming at full intensity.")]
    public float maxSpeed = 10f;

    // The rate at which the object accelerates in the direction of input.
    [Range(0, 10), Tooltip("The rate at which the object accelerates in the direction of input.")]
    public float accelerationFactor = 0.1f;

    // The rate at which the object decelerates when swim input is reduced or stopped.
    [Range(0, 10), Tooltip("The rate at which the object decelerates when swim input is reduced or stopped.")]
    public float decelerationFactor = 5f;

    // The weight determines how much the input force and forward force are blended.
    [Range(0f, 1f), Tooltip("Determines how much the input force and forward force are blended.\n" +
             "At 0, only forward force is used (pure swimming in the direction the character is facing).\n" +
             "At 1, only input force is used (pure strafing/moving sideways).")]
    public float weight = 0.5f;

    private float m_currentForce = 0f;
    private float m_targetForce = 0f;
    private float m_previousSwimInput = 0f;

    /// <summary>
    /// Applies torque to the object based on the input provided.
    /// The torque is calculated to achieve the desired roll, pitch, and yaw angles.
    /// </summary>
    /// <param name="_input">The input vector containing horizontal and vertical values.</param>
    /// <param name="_rigidbody">The Rigidbody component of the object.</param>
    public override void ApplyTorque(Vector2 _input, Rigidbody _rigidbody)
    {
        // Calculate the target roll and pitch angles based on the input and the current movement type.
        float rollTarget = _input.x * rollMaxAngle;
        float pitchTarget = _input.y * pitchMaxAngle;

        // Calculate the current horizontal forward and right vectors, ignoring the vertical component.
        Vector3 currentHorizontalForward = Vector3.ProjectOnPlane(_rigidbody.transform.rotation * Vector3.forward, Vector3.up).normalized;
        Vector3 currentHorizontalRight = Vector3.ProjectOnPlane(_rigidbody.transform.rotation * Vector3.right, Vector3.up).normalized;

        // Calculate the current pitch and roll angles of the object.
        float currentPitch = Vector3.SignedAngle(currentHorizontalForward, _rigidbody.transform.forward, currentHorizontalRight);
        float currentRoll = Vector3.SignedAngle(Vector3.up, _rigidbody.transform.up, currentHorizontalForward);

        // Calculate the torque to apply for achieving the desired roll, pitch, and yaw (negative of _input.x) angles.
        float rollTorqueToApply = (rollTarget - currentRoll) * rollTorque;
        float pitchTorqueToApply = (pitchTarget - currentPitch) * pitchTorque;
        float yawTorqueToApply = -_input.x * yawTorque;

        // Create a torque vector based on the pitch, yaw, and roll torque values.
        Vector3 torque = new Vector3(pitchTorqueToApply, (1.0f - shareApplyYawToWorldUp) * yawTorqueToApply, rollTorqueToApply);

        // Apply the calculated torque in the object's local space.
        _rigidbody.AddRelativeTorque(torque);

        // Apply additional yaw torque only along the world's up vector (y-axis).
        Vector3 yawTorqueVector = new Vector3(0.0f, shareApplyYawToWorldUp * yawTorqueToApply, 0.0f);
        _rigidbody.AddTorque(yawTorqueVector);
    }

    /// <summary>
    /// Applies the constant force to the Rigidbody based on swim and movement input.
    /// </summary>
    /// <param name="_swimInput">The swim input value between 0 and 1.</param>
    /// <param name="_movementInput">The movement input vector.</param>
    /// <param name="_rigidbody">The Rigidbody to which the force is applied.</param>
    public override void ApplyConstantForce(float _swimInput, Vector2 _movementInput, Rigidbody _rigidbody)
    {
        // If swimInput is 0, gradually reduce the force to 0.
        if (_swimInput == 0f)
        {
            m_targetForce = 0f;
        }
        else
        {
            float maxForce = maxSpeed * _swimInput;
            float curveValue = forceCurve.Evaluate(_swimInput);
            m_targetForce = Mathf.Lerp(0f, maxForce, curveValue);
        }

        float smoothingFactor;
        // Choose between deceleration or acceleration factor based on swim input.
        if (_swimInput < m_previousSwimInput || _swimInput == 0)
        {
            smoothingFactor = decelerationFactor;
        }
        else
        {
            smoothingFactor = accelerationFactor;
        }

        // Smoothly adjust the current force towards the target force.
        m_currentForce = Mathf.Lerp(m_currentForce, m_targetForce, Time.fixedDeltaTime * smoothingFactor);

        m_previousSwimInput = _swimInput;

        Vector3 forwardDirection = _rigidbody.transform.forward;
        Vector3 rightDirection = _rigidbody.transform.right;
        Vector3 upDirection = _rigidbody.transform.up;

        // Invert movement input to align with the forward direction of the Rigidbody.
        _movementInput *= -1;
        Vector3 inputForce = _movementInput.x * rightDirection + _movementInput.y * upDirection;
        Vector3 forwardForce = forwardDirection;

        // Blend the input force with the forward force to make the movement more natural.
        Vector3 blendedForce = Vector3.Lerp(inputForce, forwardForce, 0.5f).normalized;

        Vector3 forceVector = m_currentForce * blendedForce;

        // Apply the force to the Rigidbody only if the velocity is below the maximum speed.
        if (_rigidbody.velocity.magnitude < maxSpeed)
        {
            _rigidbody.AddForce(forceVector, ForceMode.Force);
        }
    }
}
