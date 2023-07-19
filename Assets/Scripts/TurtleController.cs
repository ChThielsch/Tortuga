using UnityEngine;

public class TurtleController : MonoBehaviour
{
    public enum MovementType { Idle, Forward, Free, TopDown, Chase }

    [Header("General")]
    public MovementType movementType;
    public Rigidbody myRigidbody;
    public TurtleFin leftFin;
    public TurtleFin rightFin;
    [Header("Force")]
    [Range(0f, 1000f)]
    public float maxForceX = 20;
    [Range(0f, 1000f)]
    public float maxForceZ = 40;
    [Space]
    [Range(0f, 1f)]
    public float rotationWeight = 0.5f;

    [Divider("Forward")]
    [Range(0f, 1000f)]
    public float forwardGlobalForce = 200;
    [Range(0f, 5f)]
    public float forwardRotationSpeedX = 1f;
    [Range(0f, 5f)]
    public float forwardRotationSpeedY = 1f;
    [Range(0f, 5f)]
    public float forwardRotationSpeedZ = 1f;
    [Range(0f, 90f)]
    public float forwardMinAngleX = 25;
    [Range(0f, 90f)]
    public float forwardMaxAngleX = 45;
    [Range(0f, 90f)]
    public float forwardMaxAngleY = 15;
    [Range(0f, 90f)]
    public float forwardMaxAngleZ = 45;

    [Divider("Free")]
    [Range(0f, 5f)]
    public float freeRotationSpeedX = 1f;
    [Range(0f, 5f)]
    public float freeRotationSpeedY = 1f;
    [Range(0f, 5f)]
    public float freeRotationSpeedZ = 1f;
    [Range(0f, 90f)]
    public float freeMinAngleX = 25;
    [Range(0f, 90f)]
    public float freeMaxAngleX = 45;
    [Range(0f, 90f)]
    public float freeMaxAngleY = 15;
    [Range(0f, 90f)]
    public float freeMaxAngleZ = 45;

    [Divider("Top Down")]
    [Range(0f, 5f)]
    public float topDownRotationSpeed = 20;
    [Range(0f, 90f)]
    public float topDownMaxAngleZ = 45;

    [Divider("Chase")]
    public float chaseXMovementSpeed = 100;
    public float chaseXBounds = 10;
    public float 
        chaseMaxAdvance = 5,
        chaseMaxFallBehind= 0;
    public float chaseMaxAdjustSpeed = 5;
    public ChaseLocus chaseLocus;

    public float m_turnForce;
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
                myRigidbody.constraints = myRigidbody.constraints | RigidbodyConstraints.FreezePositionY;
                break;
            case MovementType.Chase:
                CameraManager.instance.ActiveChaseCamera();
                myRigidbody.useGravity = false;
                chaseLocus.StartChase();
                myRigidbody.constraints = myRigidbody.constraints | RigidbodyConstraints.FreezePositionY;
                break;

            case MovementType.Forward:
            case MovementType.Free:
            default:
                CameraManager.instance.ActiveThirdPersonCamera();
                myRigidbody.useGravity = false;
                myRigidbody.constraints = myRigidbody.constraints & ~RigidbodyConstraints.FreezePositionY;
                break;
        }
        if (movementType!=MovementType.Chase)
            chaseLocus.StopChase();

    }

    /// <summary>
    /// Changes the movement type and adjusts camera and gravity settings accordingly.
    /// </summary>
    /// <param name="_movementIndex">The movement type index to set.</param>
    public void ChangeMovementType(int _movementIndex)
    {
        ChangeMovementType((MovementType)_movementIndex);
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
        (Vector3 direction, float force) splineCurrent= (Vector3.zero, 0);
        if (movementType == MovementType.Chase)
        {
            splineCurrent = GetChaseCurrent(_input);
        }
        else
        {
            // Get the current direction and force from the spline at the turtle's position
            splineCurrent = SplineCurrentLink.GetAffectedDirection(transform.position);

            //Project it on the XZ plane if topdown for safety
            if (movementType == MovementType.TopDown)
                splineCurrent.direction = Vector3.ProjectOnPlane(splineCurrent.direction, Vector3.up);
        }


        Vector3 currentDirection = splineCurrent.direction;
        float currentForce = splineCurrent.force;

        Quaternion currentRotation = Quaternion.identity;
        bool inCurrent = currentDirection != Vector3.zero;

        Vector3 totalForce = (currentDirection * currentForce) + GetForceBasedOnRotation();

        if (myRigidbody.velocity.magnitude < 10)
        {
            totalForce += GetRotationForce(m_turnForce);
        }

        if (inCurrent)
        {
            // Disable gravity if there is a current direction
            myRigidbody.useGravity = false;
            // Calculate the rotation towards the current direction
            currentRotation = RotateTowardsDirection(currentDirection);
        }
        else
        {
            // Enable gravity if there is no current direction
            if (movementType != MovementType.TopDown && movementType != MovementType.Chase && movementType != MovementType.Idle)
            {
                myRigidbody.useGravity = true;
            }
        }

        Quaternion movementRotation = Quaternion.identity;
        switch (movementType)
        {
            case MovementType.Forward:
                // Calculate the rotation for forward movement
                movementRotation = GetForwardRotation(_input);
                totalForce += Vector3.forward * forwardGlobalForce;
                break;
            case MovementType.Free:
                // Calculate the rotation for free movement
                movementRotation = GetFreeRotation(_input);
                break;
            case MovementType.TopDown:
                // Calculate the rotation for top-down movement
                movementRotation = GetTopDownRotation(_input);
                break;
            case MovementType.Chase:
                // Calculate the rotation for top-down chase movement
                movementRotation = currentRotation;
                totalForce += GetChaseMovement(_input);
                break;
            default:
                break;
        }

        // Apply the force to the Rigidbody
        myRigidbody.AddForce(totalForce);

        // Weight the rotation between the current rotation and the movement rotation
        Quaternion finalRotation = Quaternion.identity;
        if (inCurrent&&movementType!=MovementType.Chase)
        {
            finalRotation = WeightRotation(currentRotation, movementRotation, rotationWeight);
        }
        else
        {
            finalRotation = movementRotation;
        }

        // Move the Rigidbody to the weighted rotation
        myRigidbody.MoveRotation(finalRotation);
    }

    private Quaternion GetForwardRotation(Vector2 _input)
    {
        // Calculate the target rotation angles based on the input vector
        float targetRotationX = (_input.y > 0) ? _input.y * forwardMaxAngleX : _input.y * forwardMinAngleX;
        float targetRotationY = _input.x * forwardMaxAngleY;
        float targetRotationZ = _input.x * forwardMaxAngleZ;

        // Calculate the current euler angles
        Vector3 currentEulerAngles = myRigidbody.rotation.eulerAngles;

        // Calculate the new euler angles based on the target and current angles
        Vector3 newEulerAngles = new Vector3(
            Mathf.LerpAngle(currentEulerAngles.x, targetRotationX, forwardRotationSpeedX * Time.fixedDeltaTime),
            Mathf.LerpAngle(currentEulerAngles.y, -targetRotationY, forwardRotationSpeedY * Time.fixedDeltaTime),
            Mathf.LerpAngle(currentEulerAngles.z, targetRotationZ, forwardRotationSpeedZ * Time.fixedDeltaTime)
        );

        // Return the rotation
        return Quaternion.Euler(newEulerAngles);
    }

    private Quaternion GetFreeRotation(Vector2 _input)
    {
        // Calculate the target rotation angles based on the input vector
        float targetRotationX = (_input.y > 0) ? _input.y * freeMaxAngleX : _input.y * freeMinAngleX;
        float targetRotationZ = _input.x * freeMaxAngleZ;

        // Calculate the current euler angles
        Vector3 currentEulerAngles = myRigidbody.rotation.eulerAngles;

        // Calculate the new euler angles based on the target and current angles
        Vector3 newEulerAngles = new Vector3(
            Mathf.LerpAngle(currentEulerAngles.x, targetRotationX, freeRotationSpeedX * Time.fixedDeltaTime),
            Mathf.LerpAngle(currentEulerAngles.y, currentEulerAngles.y + -_input.x * freeRotationSpeedY * 100 * Time.fixedDeltaTime, freeRotationSpeedY * 100 * Time.fixedDeltaTime),
            Mathf.LerpAngle(currentEulerAngles.z, targetRotationZ, freeRotationSpeedZ * Time.fixedDeltaTime)
        );

        m_turnForce = Mathf.DeltaAngle(currentEulerAngles.y, newEulerAngles.y);

        // Return the rotation
        return Quaternion.Euler(newEulerAngles);
    }

    private Quaternion GetTopDownRotation(Vector2 _input)
    {
        _input *= -1;

        // Get the current euler angles of the rigidbody's rotation
        Vector3 currentEulerAngles = myRigidbody.rotation.eulerAngles;

        float targetAngleY = currentEulerAngles.y;

        if (_input != Vector2.zero)
        {
            // Calculate the target angle in the Y-axis based on the input vector
            targetAngleY = Mathf.Atan2(_input.y, -_input.x) * Mathf.Rad2Deg;
        }

        // Determine the direction of rotation by calculating the difference between current and target angles
        float rotationDifference = Mathf.DeltaAngle(currentEulerAngles.y, targetAngleY);

        // Clamp the rotation difference to the maximum allowed angle in the X-axis
        float targetAngleZ = Mathf.Clamp(-rotationDifference, -topDownMaxAngleZ, topDownMaxAngleZ);

        // Set the new euler angles with a gradual lerp towards zero rotation in the X-axis
        Vector3 newEulerAngles = new Vector3(
            Mathf.LerpAngle(currentEulerAngles.x, 0, topDownRotationSpeed * Time.deltaTime),
            myRigidbody.rotation.eulerAngles.y,
            currentEulerAngles.z);

        // If there is input (movement), update the new euler angles with interpolation towards the target angles
        if (_input != Vector2.zero)
        {
            newEulerAngles = new Vector3(
                0f,
                Mathf.LerpAngle(currentEulerAngles.y, targetAngleY, topDownRotationSpeed * Time.deltaTime),
                Mathf.LerpAngle(currentEulerAngles.x, targetAngleZ, topDownRotationSpeed * Time.deltaTime)
            );
        }

        m_turnForce = rotationDifference;

        // Return the new rotation by setting the rigidbody's rotation using a quaternion created from the new euler angles
        return Quaternion.Euler(newEulerAngles);
    }
    
    private Quaternion GetChaseRotation(Vector2 _input) => RotateTowardsDirection(chaseLocus.transform.forward);
    public (Vector3, float) GetChaseCurrent(Vector2 _input)
    {
        Vector3
            locusPos = chaseLocus.transform.position,
            locusDir = chaseLocus.transform.forward;
        Vector3 directionRaw = transform.position - locusPos;

        Vector3
            ZLine = Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(directionRaw, chaseLocus.transform.up), chaseLocus.transform.right);

        bool isAhead = locusDir * ZLine.magnitude == ZLine;

        float locusDisance = ZLine.magnitude;
        float maxDistance = isAhead ? chaseMaxAdvance : chaseMaxFallBehind;
        float force = (locusDisance / Mathf.Max(maxDistance,0.1f)) * chaseMaxAdjustSpeed;

       
        return (locusDir,force);
    }
    public Vector3 GetChaseMovement(Vector3 _input)
    {
        Vector3
            locusPos = chaseLocus.transform.position,
            locusDir = chaseLocus.transform.forward,
            locusRight = Vector3.Cross(Vector3.up, chaseLocus.transform.forward);
        Vector3 directionRaw = transform.position - locusPos;

        Vector3 XLine = Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(directionRaw, chaseLocus.transform.up), chaseLocus.transform.forward);

        Vector3 movement=Vector3.zero;
        //if (_input.y == 0) //Steer back to center without input
        //    movement = XLine * chaseXMovementSpeed*0.5f;
        //else
        movement = locusRight * _input.y;

        Vector3 targetPos= XLine + movement;
        targetPos = targetPos.normalized * Mathf.Min(targetPos.magnitude, chaseXBounds);
        targetPos = locusPos + targetPos;

        Vector3 direction = targetPos - transform.position;
        direction = direction * chaseXMovementSpeed;

        Debug.DrawRay(transform.position,direction);

        return direction;
    }

    private Vector3 GetRotationForce(float rotationDifference)
    {
        // Calculate the force magnitude based on the absolute rotation difference
        float forceMagnitude = Mathf.Abs(rotationDifference);

        // Get the forward direction of the object
        Vector3 forwardDirection = myRigidbody.transform.forward;

        // Get the rotation direction as a vector relative to the current rotation
        Vector3 rotationDirection = Quaternion.Euler(0, rotationDifference, 0) * forwardDirection;

        // Calculate the average direction between forward direction and rotation direction
        Vector3 averageDirection = (forwardDirection + rotationDirection).normalized;

        // Apply the force to the rigidbody
        return averageDirection * forceMagnitude;
    }
    private Vector3 GetForceBasedOnRotation()
    {
        Vector3 eulerAngles = transform.eulerAngles;

        float angleX = ConvertAngleToRange(eulerAngles.x);
        float angleZ = ConvertAngleToRange(eulerAngles.z);

        Vector3 localForward = transform.forward;
        Vector3 localRight = transform.right;

        Vector3 localFloatingForce = localForward * angleX * maxForceZ * Time.fixedDeltaTime +
                                     localRight * angleZ * -maxForceX * Time.fixedDeltaTime;


        return localFloatingForce;
    }

    public static float ConvertAngleToRange(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }

        return angle;
    }

    public Quaternion RotateTowardsDirection(Vector3 direction)
    {
        Quaternion currentRotation = myRigidbody.rotation;
        Vector3 forwardDirection = myRigidbody.transform.forward;

        // Calculate the target rotation based on the direction vector
        Quaternion targetRotation = Quaternion.FromToRotation(forwardDirection, direction) * currentRotation;

        // Smoothly rotate the Rigidbody towards the target rotation
        Quaternion newRotation = Quaternion.Lerp(currentRotation, targetRotation, forwardRotationSpeedY * Time.deltaTime);

        return newRotation;
    }

    public Quaternion WeightRotation(Quaternion rotationA, Quaternion rotationB, float weight)
    {
        // Ensure the weight is between 0 and 1
        weight = Mathf.Clamp01(weight);

        // Perform the weighted interpolation between the two rotations
        Quaternion newRotation = Quaternion.Lerp(rotationA, rotationB, weight);

        // Apply the rotation to the Rigidbody
        return newRotation;
    }
}