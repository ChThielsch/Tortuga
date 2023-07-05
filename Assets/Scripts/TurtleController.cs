using UnityEngine;

public class TurtleController : MonoBehaviour
{
    public Rigidbody myRigidbody;
    public TurtleFin leftFin;
    public TurtleFin rightFin;

    [Tooltip("Adjust this value to control the rotation speed.")]
    [Range(0f, 360f)]
    public float rotationSpeed = 45f;

    [Tooltip("Adjust this value to control the maximum rotation angle around the X-axis.")]
    [Range(0f, 90f)]
    public float maxXRotationAngle = 45f;

    [Tooltip("Adjust this value to control the maximum rotation angle around the Z-axis.")]
    [Range(0f, 90f)]
    public float maxZRotationAngle = 45f;

    [Tooltip("Adjust this value to control the strength of the continuous force")]
    [Range(0f, 100f)]
    public float forceMultiplier = 1f;

    private Quaternion m_originalRotation;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(myRigidbody.centerOfMass, 0.05f);
    }

    private void Start()
    {
        m_originalRotation = transform.rotation;
    }

    public void Swim()
    {
        leftFin.Swim(myRigidbody);
        rightFin.Swim(myRigidbody);
    }

    /// <summary>
    /// Moves the turtle based on the input provided and applies a continuous force in the direction of rotation.
    /// </summary>
    /// <param name="_input">The input vector used to calculate the target rotation.</param>
    public void Move(Vector2 _input)
    {
        // Calculate the target rotation based on inverted input.
        Quaternion targetRotation = CalculateTargetRotation(-_input);

        // Calculate the rotation speed adjusted for frame rate.
        float adjustedRotationSpeed = rotationSpeed * Time.fixedDeltaTime * Time.timeScale;

        // Smoothly rotate the turtle towards the target rotation.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, adjustedRotationSpeed * Time.timeScale);

        // Calculate the rotation values based on the current rotation of the turtle.
        Vector3 rotationAngles = transform.rotation.eulerAngles;
        float xRotation = rotationAngles.x;
        float zRotation = rotationAngles.z;

        // Calculate the continuous force based on the rotation values and apply it to the rigidbody.
        Vector3 continuousForce = new Vector3(
            -Mathf.Sin(Mathf.Deg2Rad * zRotation),
            0f,
            Mathf.Sin(Mathf.Deg2Rad * xRotation)
        ) * forceMultiplier;
        myRigidbody.AddForce(continuousForce);
    }

    private Quaternion CalculateTargetRotation(Vector2 _input)
    {
        float xRotation = Mathf.Lerp(0f, maxXRotationAngle, Mathf.Abs(_input.x)) * Mathf.Sign(_input.x);
        float zRotation = Mathf.Lerp(0f, maxZRotationAngle, Mathf.Abs(_input.y)) * Mathf.Sign(_input.y);

        Quaternion targetRotation = m_originalRotation * Quaternion.Euler(xRotation, 0f, zRotation);

        return targetRotation;
    }
}