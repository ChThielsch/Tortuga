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
    public float maxForce = 50f;
    public AnimationCurve forceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public float timeToReachMaxForce = 2f;

    private float m_currentForce = 0f;
    private float m_elapsedTime = 0f;

    public float maxSpeed = 10f;

    public float horizontalMultiplier = 1f;
    public float verticalMultiplier = 1f;

    [Range(0f, 1f)] public float weight = 0.5f; // New weight variable

    public void ApplyConstantForce(float _swimInput, Vector2 _movementInput, Rigidbody _rigidbody)
    {
        if (_swimInput == 0f)
        {
            ResetForce();
            return;
        }

        _movementInput *= -1;

        float normalizedSwimInput = _swimInput;
        float modifiedMaxForce = maxForce * normalizedSwimInput;
        float modifiedTimeToReachMaxForce = timeToReachMaxForce * (1f / normalizedSwimInput);

        m_elapsedTime += Time.deltaTime;

        // Evaluate the AnimationCurve at the current time to get the force increment factor
        float curveValue = forceCurve.Evaluate(m_elapsedTime / modifiedTimeToReachMaxForce);

        // Calculate the current force based on the modified max force and the curve value
        m_currentForce = Mathf.Lerp(0f, modifiedMaxForce, curveValue);

        Vector3 forwardDirection = _rigidbody.transform.forward;
        Vector3 rightDirection = _rigidbody.transform.right;
        Vector3 upDirection = _rigidbody.transform.up;

        // Calculate the force based on the input and the rigidbody's forward direction
        Vector3 inputForce = _movementInput.x * rightDirection + _movementInput.y * upDirection;
        Vector3 forwardForce = forwardDirection;

        // Blend the two forces based on the weight
        Vector3 blendedForce = Vector3.Lerp(inputForce, forwardForce, weight).normalized;

        Vector3 forceVector = m_currentForce * blendedForce;

        _rigidbody.AddForce(forceVector, ForceMode.Force);

        // Limit the Rigidbody's velocity to the specified maxSpeed
        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxSpeed);
    }

    public void ResetForce()
    {
        m_currentForce = 0f;
        m_elapsedTime = 0f;
    }
}