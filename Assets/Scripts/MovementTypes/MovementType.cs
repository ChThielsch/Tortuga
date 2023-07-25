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
    public AnimationCurve forceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public float maxSpeed = 10f;
    public float forceBuildUpSpeed = 1f; // New variable for controlling force build-up speed

    private float m_currentForce = 0f;
    private float m_accumulatedForce = 0f;

    [Range(0f, 1f)]
    public float weight = 0.5f;

    public void ApplyConstantForce(float _swimInput, Vector2 _movementInput, Rigidbody _rigidbody)
    {
        if (_swimInput == 0f)
        {
            m_accumulatedForce = 0f;
            return;
        }

        _movementInput *= -1;

        float maxForce = maxSpeed * _swimInput;
        float curveValue = forceCurve.Evaluate(_swimInput);
        m_currentForce = Mathf.Lerp(0f, maxForce, curveValue);

        // Increment the accumulated force over time with the build-up speed control
        m_accumulatedForce = Mathf.MoveTowards(m_accumulatedForce, m_currentForce, forceBuildUpSpeed * Time.deltaTime);

        Vector3 forwardDirection = _rigidbody.transform.forward;
        Vector3 rightDirection = _rigidbody.transform.right;
        Vector3 upDirection = _rigidbody.transform.up;

        Vector3 inputForce = _movementInput.x * rightDirection + _movementInput.y * upDirection;
        Vector3 forwardForce = forwardDirection;

        Vector3 blendedForce = Vector3.Lerp(inputForce, forwardForce, weight).normalized;

        Vector3 forceVector = m_accumulatedForce * blendedForce;

        _rigidbody.AddForce(forceVector, ForceMode.Force);
        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxSpeed);
    }
}