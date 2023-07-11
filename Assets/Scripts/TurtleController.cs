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
    public float RotationSpeedX = 45f;
    public float RotationSpeedY = 45f;
    public float RotationSpeedZ = 45f;
    public float maxAngleX = 45;
    public float maxAngleY = 15;
    public float minAngleZ = -45;
    public float maxAngleZ = 25;
    [Header("Top Down")]
    public float TopDownRotationSpeed = 20;
    public float TopDownMaxAngleX = 45;


    [Header("Force")]
    public float maxForceX = 20;
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
            default:
                CameraManager.instance.ActiveThirdPersonCamera();
                myRigidbody.useGravity = true;
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
        // Calculate the target rotation angles based on the input vector
        float targetRotationX = -_input.x * maxAngleX;
        float targetRotationY = _input.x * maxAngleY;
        float targetRotationZ = (_input.y > 0) ? _input.y * maxAngleZ : _input.y * -minAngleZ;

        // Calculate the current euler angles
        Vector3 currentEulerAngles = myRigidbody.rotation.eulerAngles;

        // Calculate the new euler angles based on the target and current angles
        Vector3 newEulerAngles = new Vector3(
            Mathf.LerpAngle(currentEulerAngles.x, targetRotationX, RotationSpeedX * Time.fixedDeltaTime),
            Mathf.LerpAngle(currentEulerAngles.y, targetRotationY, RotationSpeedY * Time.fixedDeltaTime),
            Mathf.LerpAngle(currentEulerAngles.z, targetRotationZ, RotationSpeedZ * Time.fixedDeltaTime)
        );

        // Apply the rotation
        myRigidbody.MoveRotation(Quaternion.Euler(newEulerAngles));
    }

    private void MoveFree(Vector2 _input)
    {
        // Calculate the target rotation angles based on the input vector
        float targetRotationX = -_input.x * maxAngleX;
        float targetRotationZ = (_input.y > 0) ? _input.y * maxAngleZ : _input.y * -minAngleZ;

        // Calculate the current euler angles
        Vector3 currentEulerAngles = myRigidbody.rotation.eulerAngles;

        // Calculate the new euler angles based on the target and current angles
        Vector3 newEulerAngles = new Vector3(
            Mathf.LerpAngle(currentEulerAngles.x, targetRotationX, RotationSpeedX * Time.fixedDeltaTime),
            Mathf.LerpAngle(currentEulerAngles.y, currentEulerAngles.y + _input.x * RotationSpeedY * 100 * Time.fixedDeltaTime, RotationSpeedY * 100 * Time.fixedDeltaTime),
            Mathf.LerpAngle(currentEulerAngles.z, targetRotationZ, RotationSpeedZ * Time.fixedDeltaTime)
        );

        // Apply the rotation
        myRigidbody.MoveRotation(Quaternion.Euler(newEulerAngles));
    }

    private void MoveTopDown(Vector2 _input)
    {
        float targetAngleY = Mathf.Atan2(_input.x, _input.y) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0f, targetAngleY, 0f);

        Quaternion currentRotation = myRigidbody.rotation;

        // Determine the direction of rotation
        float rotationDifference = Mathf.DeltaAngle(currentRotation.eulerAngles.y, targetRotation.eulerAngles.y);
        float targetAngleX = 0;
        if (rotationDifference == 0 || _input == Vector2.zero)
        {
            targetAngleX = 0;
        }
        else if (rotationDifference < 0f)
        {
            targetAngleX = TopDownMaxAngleX;
        }
        else
        {
            targetAngleX = -TopDownMaxAngleX;
        }

        if (_input != Vector2.zero)
        {
            targetRotation = Quaternion.Euler(targetAngleX, targetAngleY, 0f);
            myRigidbody.MoveRotation(Quaternion.RotateTowards(currentRotation, targetRotation, TopDownRotationSpeed * Time.deltaTime));
        }
        else
        {
            Quaternion XTargetRotation = Quaternion.Euler(targetAngleX, currentRotation.eulerAngles.y, 0f);
            myRigidbody.MoveRotation(Quaternion.RotateTowards(currentRotation, XTargetRotation, TopDownRotationSpeed * Time.deltaTime));
        }
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