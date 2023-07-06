using UnityEngine;

public class TurtleController : MonoBehaviour
{
    [Header("General")]
    public Rigidbody myRigidbody;
    public TurtleFin leftFin;
    public TurtleFin rightFin;

    [Header("Rotation")]
    [Tooltip("Adjust this value to control the rotation speed.")]
    [Range(0f, 360f)]
    public float rotationSpeed = 45f;

    [Tooltip("Adjust the rotation damping value to control the rate of rotation slowdown. Higher values result in slower rotation.")]
    [Range(0f, 15f)]
    public float rotationDamping = 2f; // Adjust this value to control the damping effect


    [Tooltip("Adjust this value to control the maximum rotation angle around the X-axis.")]
    [Range(0f, 90f)]
    public float maxXRotationAngle = 45f;

    [Tooltip("Adjust this value to control the maximum rotation angle around the Y-axis.")]
    [Range(0f, 90f)]
    public float maxYRotationAngle = 45f;

    [Tooltip("Adjust this value to control the maximum rotation angle around the Z-axis.")]
    [Range(-90f, 0f)]
    public float minZRotationAngle = -45f;

    [Tooltip("Adjust this value to control the maximum rotation angle around the Z-axis.")]
    [Range(0f, 90f)]
    public float maxZRotationAngle = 45f;

    [Header("Force")]
    [Tooltip("Adjust this value to control the maximum force applied when the turtle is at its maximum rotation.")]
    public float maxXForce = 10f;

    [Tooltip("Adjust this value to control the maximum force applied when the turtle is at its maximum rotation.")]
    public float maxZForce = 10f;

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

        // Calculate the rotation difference between the current rotation and the target rotation.
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(transform.rotation);

        // Calculate the damping factor based on the rotation difference.
        float damping = Mathf.Clamp01(deltaRotation.eulerAngles.magnitude / 180f) * rotationDamping;

        // Smoothly rotate the turtle towards the target rotation using the damping factor.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, damping * Time.fixedDeltaTime);

        // Calculate the rotation values based on the current rotation of the turtle.
        Vector3 rotationAngles = transform.rotation.eulerAngles;
        float xRotation = rotationAngles.x;
        float zRotation = rotationAngles.z;

        // Calculate the force based on the rotation values and apply it to the rigidbody.
        float xForce = Mathf.Lerp(0f, maxXForce, Mathf.Abs(xRotation) / maxXRotationAngle);
        float zForce = Mathf.Lerp(0f, maxZForce, Mathf.Abs(zRotation) / Mathf.Abs(minZRotationAngle));

        Vector3 continuousForce = new Vector3(
            -Mathf.Sin(Mathf.Deg2Rad * zRotation) * zForce,
            0f,
            Mathf.Sin(Mathf.Deg2Rad * xRotation) * xForce
        );

        myRigidbody.AddForce(continuousForce);
    }

    private Quaternion CalculateTargetRotation(Vector2 _input)
    {
        float xRotation = Mathf.Lerp(0f, maxXRotationAngle, Mathf.Abs(_input.x)) * Mathf.Sign(_input.x);
        float yRotation = Mathf.Lerp(0f, maxYRotationAngle, Mathf.Abs(_input.x)) * Mathf.Sign(_input.x);
        float zRotation = Mathf.Lerp(0f, -minZRotationAngle, Mathf.Abs(_input.y)) * Mathf.Sign(_input.y);

        // Clamp the z rotation between the min and max values.
        zRotation = Mathf.Clamp(zRotation, minZRotationAngle, maxZRotationAngle);

        Quaternion targetRotation = m_originalRotation * Quaternion.Euler(xRotation, -yRotation, zRotation);

        return targetRotation;
    }
}