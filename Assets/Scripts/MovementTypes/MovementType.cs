using UnityEngine;

public class MovementType : ScriptableObject
{
    [Divider("Fins")]
    [Tooltip("The maximum force applied to the turtle along the given curve")]
    public float forceStrength;
    [Tooltip("The default unity force mode of the force")]
    public ForceMode forceMode;
    [Tooltip("The duration in seconds during which the force is applied along the curve")]
    public float swimDuration = 1.25f;
    [Tooltip("The curve on which the force is applied upto its maximun forceStrength for the duration of swimDuration")]
    public AnimationCurve paddleCurve;

    public virtual void ApplyTorque(Vector2 _input, Rigidbody rigidbody)
    {

    }

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
    /// Applies the constant force to the Rigidbody based on swim and movement input.
    /// </summary>
    /// <param name="_swimInput">The swim input value between 0 and 1.</param>
    /// <param name="_movementInput">The movement input vector.</param>
    /// <param name="_rigidbody">The Rigidbody to which the force is applied.</param>
    public void ApplyConstantForce(float _swimInput, Vector2 _movementInput, Rigidbody _rigidbody)
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