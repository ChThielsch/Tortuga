using UnityEngine;

public class TurtleController : MonoBehaviour
{
    public enum MovementType { Idle, Forward, Free, TopDown }

    [Header("General")]
    public MovementType movementType;
    public Rigidbody myRigidbody;
    public TurtleFin leftFin;
    public TurtleFin rightFin;

    [Header("Rotation")]
    [Range(0f, 5f)]
    public float RotationSpeedX = 1f;
    [Range(0f, 5f)]
    public float RotationSpeedY = 1f;
    [Range(0f, 5f)]
    public float RotationSpeedZ = 1f;
    [Range(0f, 90f)]
    public float maxAngleX = 45;
    [Range(0f, 90f)]
    public float maxAngleY = 15;
    [Range(0f, 90f)]
    public float minAngleZ = 25;
    [Range(0f, 90f)]
    public float maxAngleZ = 45;
    [Header("Top Down")]
    [Range(0f, 5f)]
    public float TopDownRotationSpeed = 20;
    [Range(0f, 90f)]
    public float TopDownMaxAngleX = 45;


    [Header("Force")]
    [Range(0f, 100f)]
    public float maxForceX = 20;
    [Range(0f, 100f)]
    public float maxForceZ = 40;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(myRigidbody.centerOfMass, 0.05f);
    }

    private void Start()
    {
        ChangeMovementType(movementType);
        leftFin.turtleController = this;
        rightFin.turtleController = this;
    }

    /// <summary>
    /// Changes the movement type and adjusts camera and gravity settings accordingly.
    /// </summary>
    /// <param name="_movementType">The movement type to set.</param>
    public void ChangeMovementType(MovementType _movementType)
    {
        movementType = _movementType;

        switch (movementType)
        {
            case MovementType.TopDown:
                CameraManager.instance.ActiveTopDownCamera();
                myRigidbody.useGravity = false;
                break;

            case MovementType.Forward:
            case MovementType.Free:
            default:
                CameraManager.instance.ActiveThirdPersonCamera();
                myRigidbody.useGravity = false;
                break;
        }
    }

    /// <summary>
    /// Changes the movement type and adjusts camera and gravity settings accordingly.
    /// </summary>
    /// <param name="_movementIndex">The movement type index to set.</param>
    public void ChangeMovementType(int _movementIndex)
    {
        movementType = (MovementType)_movementIndex;

        switch (_movementIndex)
        {
            case (int)MovementType.TopDown:
                CameraManager.instance.ActiveTopDownCamera();
                myRigidbody.useGravity = false;
                break;

            case (int)MovementType.Forward:
            case (int)MovementType.Free:
                CameraManager.instance.ActiveThirdPersonCamera();
                myRigidbody.useGravity = true;
                break;
            default:
                CameraManager.instance.ActiveThirdPersonCamera();
                myRigidbody.useGravity = false;
                break;
        }
    }

    public void Swim()
    {
        leftFin.Swim();
        rightFin.Swim();
    }

    /// <summary>
    /// Moves the turtle based on the input provided and applies a continuous force in the direction of rotation.
    /// </summary>
    /// <param name="_input">The input vector used to calculate the target rotation.</param>
    public void Move(Vector2 _input)
    {
        switch (movementType)
        {
            case MovementType.Forward:
                MoveForward(_input);
                ApplyForceBasedOnRotation();
                break;
            case MovementType.Free:
                MoveFree(_input);
                ApplyForceBasedOnRotation();
                break;
            case MovementType.TopDown:
                MoveTopDown(_input);
                break;
            default:
                break;
        }
    }

    private void MoveForward(Vector2 _input)
    {
        _input *= -1;

        // Calculate the target rotation angles based on the input vector
        float targetRotationX = _input.x * maxAngleX;
        float targetRotationY = _input.x * maxAngleY;
        float targetRotationZ = (_input.y > 0) ? _input.y * maxAngleZ : _input.y * minAngleZ;

        // Calculate the current euler angles
        Vector3 currentEulerAngles = myRigidbody.rotation.eulerAngles;

        // Calculate the new euler angles based on the target and current angles
        Vector3 newEulerAngles = new Vector3(
            Mathf.LerpAngle(currentEulerAngles.x, targetRotationX, RotationSpeedX * Time.fixedDeltaTime),
            Mathf.LerpAngle(currentEulerAngles.y, -targetRotationY, RotationSpeedY * Time.fixedDeltaTime),
            Mathf.LerpAngle(currentEulerAngles.z, targetRotationZ, RotationSpeedZ * Time.fixedDeltaTime)
        );

        // Apply the rotation
        myRigidbody.MoveRotation(Quaternion.Euler(newEulerAngles));
    }

    private void MoveFree(Vector2 _input)
    {
        _input *= -1;

        // Calculate the target rotation angles based on the input vector
        float targetRotationX = _input.x * maxAngleX;
        float targetRotationZ = (_input.y > 0) ? _input.y * maxAngleZ : _input.y * minAngleZ;

        // Calculate the current euler angles
        Vector3 currentEulerAngles = myRigidbody.rotation.eulerAngles;

        // Calculate the new euler angles based on the target and current angles
        Vector3 newEulerAngles = new Vector3(
            Mathf.LerpAngle(currentEulerAngles.x, targetRotationX, RotationSpeedX * Time.fixedDeltaTime),
            Mathf.LerpAngle(currentEulerAngles.y, currentEulerAngles.y + -_input.x * RotationSpeedY * 100 * Time.fixedDeltaTime, RotationSpeedY * 100 * Time.fixedDeltaTime),
            Mathf.LerpAngle(currentEulerAngles.z, targetRotationZ, RotationSpeedZ * Time.fixedDeltaTime)
        );

        // Apply the rotation
        myRigidbody.MoveRotation(Quaternion.Euler(newEulerAngles));
    }

    private void MoveTopDown(Vector2 _input)
    {
        // Calculate the target angle in the Y-axis based on the input vector
        float targetAngleY = Mathf.Atan2(_input.x, _input.y) * Mathf.Rad2Deg;

        // Get the current euler angles of the rigidbody's rotation
        Vector3 currentEulerAngles = myRigidbody.rotation.eulerAngles;

        // Determine the direction of rotation by calculating the difference between current and target angles
        float rotationDifference = Mathf.DeltaAngle(currentEulerAngles.y, targetAngleY);

        // Clamp the rotation difference to the maximum allowed angle in the X-axis
        float targetAngleX = Mathf.Clamp(-rotationDifference, -TopDownMaxAngleX, TopDownMaxAngleX);

        // Set the new euler angles with a gradual lerp towards zero rotation in the X-axis
        Vector3 newEulerAngles = new Vector3(
            Mathf.LerpAngle(currentEulerAngles.x, 0, TopDownRotationSpeed * Time.deltaTime),
            myRigidbody.rotation.eulerAngles.y,
            currentEulerAngles.z);

        // If there is input (movement), update the new euler angles with interpolation towards the target angles
        if (_input != Vector2.zero)
        {
            newEulerAngles = new Vector3(
                Mathf.LerpAngle(currentEulerAngles.x, targetAngleX, TopDownRotationSpeed * Time.deltaTime),
                Mathf.LerpAngle(currentEulerAngles.y, targetAngleY, TopDownRotationSpeed * Time.deltaTime),
                0f
            );
        }

        // Apply the new rotation by setting the rigidbody's rotation using a quaternion created from the new euler angles
        myRigidbody.MoveRotation(Quaternion.Euler(newEulerAngles));
    }

    private void ApplyForceBasedOnRotation()
    {
        Vector3 eulerAngles = transform.eulerAngles;

        float angleX = ConvertAngleToRange(eulerAngles.x);
        float angleZ = ConvertAngleToRange(eulerAngles.z);

        Vector3 localForward = transform.forward;
        Vector3 localRight = transform.right;

        Vector3 localFloatingForce = localForward * angleX * maxForceZ * Time.fixedDeltaTime +
                                     localRight * angleZ * -maxForceX * Time.fixedDeltaTime;

        // Set the y component of the force to 0
        localFloatingForce.y = 0f;

        myRigidbody.AddForce(localFloatingForce);
    }

    public static float ConvertAngleToRange(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }

        return angle;
    }
}