using System.Collections;
using UnityEngine;

public class TurtleController : MonoBehaviour
{
    public enum MovementTypeEnum { Idle, Forward, Free, TopDown }

    [Header("General")]
    public MovementTypeEnum movementType;
    [ReadOnly] public MovementType currentMovementType;
    public ForwardMovement forwardMovementType;
    public FreeMovement freeMovementType;
    public TopDownMovement topDownMovementType;
    public Rigidbody myRigidbody;
    [Divider("Fins")]
    public Transform leftFin;
    public Transform leftFinDirectionHandle;
    [Space]
    public Transform rightFin;
    public Transform rightFinDirectionHandle;

    private bool m_swimBlock;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(myRigidbody.centerOfMass, 0.05f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(leftFin.position, leftFinDirectionHandle.position);
        Gizmos.DrawSphere(leftFinDirectionHandle.position, 0.05f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(rightFin.position, rightFinDirectionHandle.position);
        Gizmos.DrawSphere(rightFinDirectionHandle.position, 0.05f);

        //ONLY IN PLAYMODE FROM HERE ON_____________________
        if (!Application.isPlaying)
        {
            return;
        }

        Gizmos.color = Color.red;
        Vector3 velocityVector = myRigidbody.velocity;
        Gizmos.DrawRay(transform.position, velocityVector);
    }

    private void Start()
    {
        ChangeMovementType(movementType);
    }

    /// <summary>
    /// Changes the movement type and adjusts camera and gravity settings accordingly.
    /// </summary>
    /// <param name="_movementType">The movement type to set.</param>
    public void ChangeMovementType(MovementTypeEnum _movementType)
    {
        movementType = _movementType;

        switch (movementType)
        {
            case MovementTypeEnum.TopDown:
                CameraManager.instance.ActiveTopDownCamera();
                myRigidbody.useGravity = false;
                currentMovementType = topDownMovementType;
                break;
            case MovementTypeEnum.Forward:
                currentMovementType = forwardMovementType;
                CameraManager.instance.ActiveThirdPersonCamera();
                myRigidbody.useGravity = true;
                break;
            case MovementTypeEnum.Free:
                currentMovementType = freeMovementType;
                CameraManager.instance.ActiveThirdPersonCamera();
                myRigidbody.useGravity = true;
                break;
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
        ChangeMovementType((MovementTypeEnum)_movementIndex);
    }

    public void Swim()
    {
        if (currentMovementType is null)
        {
            return;
        }

        if (!m_swimBlock)
        {
            StartCoroutine(SwimRoutine(Utils.GetDirectionBetweenPoints(leftFin.position, leftFinDirectionHandle.position)));
            StartCoroutine(SwimRoutine(Utils.GetDirectionBetweenPoints(rightFin.position, rightFinDirectionHandle.position)));
        }
    }

    /// <summary>
    /// Moves the turtle based on the input provided and applies a continuous force in the direction of rotation.
    /// </summary>
    /// <param name="_input">The input vector used to calculate the target rotation.</param>
    public void Move(Vector2 _input)
    {
        if (currentMovementType is null)
        {
            return;
        }

        Quaternion movementRotation = currentMovementType.GetRotation(_input, myRigidbody);

        // Apply the force to the Rigidbody
        myRigidbody.AddForce(currentMovementType.GetForceBasedOnRotation(transform), ForceMode.Acceleration);

        // Move the Rigidbody to the weighted rotation
        myRigidbody.MoveRotation(movementRotation);

        if (Vector3.Dot(myRigidbody.velocity, transform.forward) > 1)
        {
            Vector3 strafeForce = new Vector3(-_input.x, 0, 0) * 15;
            Vector3 localStrafeForce = myRigidbody.transform.TransformDirection(strafeForce);
            myRigidbody.AddForce(localStrafeForce, ForceMode.Acceleration);
        }
    }

    public IEnumerator SwimRoutine(Vector3 _swimForceDirection)
    {
        m_swimBlock = true;
        float elapsedTime = 0f;
        float initialForceStrength = 0f; // Initial force strength

        while (elapsedTime < currentMovementType.swimDuration)
        {
            float normalizedTime = elapsedTime / currentMovementType.swimDuration;
            float curveValue = currentMovementType.paddleCurve.Evaluate(normalizedTime); // Evaluate the animation curve at the normalized time
            float currentForceStrength = Mathf.Lerp(initialForceStrength, currentMovementType.forceStrength, curveValue);

            Vector3 swimDirection = Vector3.zero;
            if (movementType == MovementTypeEnum.TopDown)
            {
                // Apply force in the direction the turtle is facing
                swimDirection = transform.forward;
            }
            else
            {
                // Apply force in the force direction
                swimDirection = _swimForceDirection;
            }

            Vector3 swimForce = swimDirection * currentForceStrength;

            myRigidbody.AddForce(swimForce, currentMovementType.forceMode);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        m_swimBlock = false;
    }
}