using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Type/Free")]
public class FreeMovement : MovementType
{
    [Divider("Torque")]
    [Tooltip("The maximum angle in degrees that the object can roll.\n" +
         "Roll refers to the rotational movement around the forward axis. " +
         "Higher values allow for more pronounced banking left or right."), Range(0f, 90f)]
    public float rollMaxAngle = 35f;

    [Tooltip("The maximum angle in degrees that the object can pitch.\n" +
             "Pitch refers to the rotational movement around the right axis. " +
             "Higher values allow for more significant upward or downward tilting."), Range(0f, 90f)]
    public float pitchMaxAngle = 33f;

    [Tooltip("The torque applied to achieve the desired roll angle.\n" +
             "Roll torque controls the rotational force for banking left or right."), Range(0.001f, 0.1f)]
    public float rollTorque = 0.01f;

    [Tooltip("The torque applied to achieve the desired pitch angle.\n" +
             "Pitch torque controls the rotational force for tilting the object up or down."), Range(0.001f, 0.1f)]
    public float pitchTorque = 0.002f;

    [Tooltip("The torque applied to achieve the desired yaw angle.\n" +
             "Yaw torque controls the rotational force for rotating the object horizontally (left or right)."), Range(0.01f, 1f)]
    public float yawTorque = 0.14f;

    //to make steering easier (hiting left performs a left curve without pitching up or down, apply the yaw torque mixed between world up and local up
    private const float shareApplyYawToWorldUp = 0.4f;

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
}
