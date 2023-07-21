using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Type/Free")]
public class FreeMovement : MovementType
{
    [Divider("Rotation")]
    [Range(0f, 5f)]
    [Tooltip("Controls the speed of rotation around the X-axis. Higher values make the rotation faster, while lower values make it slower.")]
    public float rotationSpeedX = 1f;

    [Range(0f, 5f)]
    [Tooltip("Controls the speed of rotation around the Y-axis. Higher values make the rotation faster, while lower values make it slower.")]
    public float rotationSpeedY = 1f;

    [Range(0f, 5f)]
    [Tooltip("Controls the speed of rotation around the Z-axis. Higher values make the rotation faster, while lower values make it slower.")]
    public float rotationSpeedZ = 1f;

    [Range(0f, 90f)]
    [Tooltip("Sets the minimum angle of rotation around the X-axis based on the input. This angle will be applied when the input vector points downwards.")]
    public float minAngleX = 25;

    [Range(0f, 90f)]
    [Tooltip("Sets the maximum angle of rotation around the X-axis based on the input. This angle will be applied when the input vector points upwards.")]
    public float maxAngleX = 45;

    [Range(0f, 90f)]
    [Tooltip("Sets the maximum angle of rotation around the Z-axis based on the input. This angle will be applied regardless of the input direction.")]
    public float maxAngleZ = 45;

    public override Quaternion GetRotation(Vector2 _input, Rigidbody _rigidbody)
    {
        // Calculate the target rotation angles based on the input vector
        float targetRotationX = (_input.y > 0) ? _input.y * maxAngleX : _input.y * minAngleX;
        float targetRotationZ = _input.x * maxAngleZ;

        // Calculate the current euler angles
        Vector3 currentEulerAngles = _rigidbody.rotation.eulerAngles;

        // Calculate the new euler angles based on the target and current angles
        Vector3 newEulerAngles = new Vector3(
            Mathf.LerpAngle(currentEulerAngles.x, targetRotationX, rotationSpeedX * Time.fixedDeltaTime),
            Mathf.LerpAngle(currentEulerAngles.y, currentEulerAngles.y + -_input.x * rotationSpeedY * 100 * Time.fixedDeltaTime, rotationSpeedY * 100 * Time.fixedDeltaTime),
            Mathf.LerpAngle(currentEulerAngles.z, targetRotationZ, rotationSpeedZ * Time.fixedDeltaTime)
        );

        // Return the rotation
        return Quaternion.Euler(newEulerAngles);
    }
}
