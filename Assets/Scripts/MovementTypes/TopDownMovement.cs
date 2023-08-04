using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Movement Type/Top Down")]

public class TopDownMovement : MovementType
{
    [Divider("Torque")]
    [Range(0f, 5f)]
    [Tooltip("Controls the overall speed of rotation. Higher values make the rotation faster, while lower values make it slower.")]
    public float rotationSpeed;

    [Range(0f, 90f)]
    [Tooltip("Sets the maximum angle of rotation around the Z-axis based on the input. This angle will be applied regardless of the input direction.")]
    public float maxAngleZ;

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

    private float m_currentForce = 0f;
    private float m_targetForce = 0f;
    private float m_previousSwimInput = 0f;

    public override void ApplyTorque(Vector2 _input, Rigidbody _rigidbody)
    {
        // Get the current euler angles of the rigidbody's rotation
        Vector3 currentEulerAngles = _rigidbody.rotation.eulerAngles;

        float targetAngleY = currentEulerAngles.y;

        if (_input != Vector2.zero)
        {
            // Calculate the target angle in the Y-axis based on the input vector
            targetAngleY = Mathf.Atan2(_input.y, _input.x) * Mathf.Rad2Deg;
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

        _rigidbody.MoveRotation(Quaternion.Euler(newEulerAngles));
    }

    public float forceMultiplier = 1f;

    /// <summary>
    /// Applies the constant force to the Rigidbody based on swim and movement input.
    /// </summary>
    /// <param name="_swimInput">The swim input value between 0 and 1.</param>
    /// <param name="_movementInput">The movement input vector. (NOT USED IN THIS CASE)</param>
    /// <param name="_rigidbody">The Rigidbody to which the force is applied.</param>
    public override void ApplyConstantForce(float _swimInput, Vector2 _movementInput, Rigidbody _rigidbody)
    {
        float targetForce = forceCurve.Evaluate(_swimInput) * forceMultiplier;

        if (targetForce > m_currentForce)
        {
            m_currentForce = Mathf.Lerp(m_currentForce, targetForce, Time.deltaTime * accelerationFactor);
        }
        else
        {
            m_currentForce = Mathf.Lerp(m_currentForce, targetForce, Time.deltaTime);
        }

        Vector3 forwardDirection = _rigidbody.transform.forward;
        Vector3 swimForce = forwardDirection * m_currentForce;

        if (_rigidbody.velocity.magnitude < maxSpeed)
        {
            _rigidbody.AddForce(swimForce);
        }
    }
}