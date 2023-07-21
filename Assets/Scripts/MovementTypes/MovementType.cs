using UnityEngine;

public class MovementType : ScriptableObject
{
    [Range(0f, 1000f)]
    public float maxForceX = 300;
    [Range(0f, 1000f)]
    public float maxForceZ = 400;
    [Divider("Fins")]
    [Tooltip("The maximum force applied to the turtle along the given curve")]
    public float forceStrength;
    [Tooltip("The default unity force mode of the force")]
    public ForceMode forceMode;
    [Tooltip("The duration in seconds during which the force is applied along the curve")]
    public float swimDuration = 1.25f;
    [Tooltip("The curve on which the force is applied upto its maximun forceStrength for the duration of swimDuration")]
    public AnimationCurve paddleCurve;

    /// <summary>
    /// Returns a new rotation based on the given input and rigidbody
    /// </summary>
    /// <param name="_input"></param>
    /// <param name="_rigidbody"></param>
    /// <returns></returns>
    public virtual Quaternion GetRotation(Vector2 _input, Rigidbody _rigidbody)
    {
        return Quaternion.identity;
    }

    public Vector3 GetForceBasedOnRotation(Transform _transform)
    {
        Vector3 eulerAngles = _transform.eulerAngles;

        float angleX = Utils.ConvertAngleToRange(eulerAngles.x);
        float angleZ = Utils.ConvertAngleToRange(eulerAngles.z);

        Vector3 localForward = _transform.forward;
        Vector3 localRight = _transform.right;

        Vector3 localFloatingForce = localForward * angleX * maxForceZ * Time.fixedDeltaTime +
                                     localRight * angleZ * -maxForceX * Time.fixedDeltaTime;

        return localFloatingForce;
    }
}