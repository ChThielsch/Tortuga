using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Type/Top Down")]

public class TopDownMovement : MovementType
{
    [Divider("Rotation")]
    [Range(0f, 5f)]
    [Tooltip("Controls the overall speed of rotation. Higher values make the rotation faster, while lower values make it slower.")]
    public float rotationSpeed;

    [Range(0f, 90f)]
    [Tooltip("Sets the maximum angle of rotation around the Z-axis based on the input. This angle will be applied regardless of the input direction.")]
    public float maxAngleZ;

    public override Quaternion GetRotation(Vector2 _input, Rigidbody _rigidbody)
    {
        _input *= -1;

        // Get the current euler angles of the rigidbody's rotation
        Vector3 currentEulerAngles = _rigidbody.rotation.eulerAngles;

        float targetAngleY = currentEulerAngles.y;

        if (_input != Vector2.zero)
        {
            // Calculate the target angle in the Y-axis based on the input vector
            targetAngleY = Mathf.Atan2(_input.y, -_input.x) * Mathf.Rad2Deg;
        }

        // Determine the direction of rotation by calculating the difference between current and target angles
        float rotationDifference = Mathf.DeltaAngle(currentEulerAngles.y, targetAngleY);

        // Clamp the rotation difference to the maximum allowed angle in the Z-axis
        float targetAngleZ = Mathf.Clamp(-rotationDifference, -maxAngleZ, maxAngleZ);

        // Set the new euler angles with a gradual lerp towards zero rotation in the X-axis
        Vector3 newEulerAngles = new Vector3(
            Mathf.LerpAngle(currentEulerAngles.x, 0, rotationSpeed * Time.deltaTime),
            _rigidbody.rotation.eulerAngles.y,
            currentEulerAngles.z);

        // If there is input (movement), update the new euler angles with interpolation towards the target angles
        if (_input != Vector2.zero)
        {
            newEulerAngles = new Vector3(
                0f,
                Mathf.LerpAngle(currentEulerAngles.y, targetAngleY, rotationSpeed * Time.deltaTime),
                Mathf.LerpAngle(currentEulerAngles.x, targetAngleZ, rotationSpeed * Time.deltaTime)
            );
        }

        // Return the new rotation by setting the rigidbody's rotation using a quaternion created from the new euler angles
        return Quaternion.Euler(newEulerAngles);
    }
}