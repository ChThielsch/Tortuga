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

    internal Player m_player;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(myRigidbody.centerOfMass, 0.05f);

        if (currentMovementType)
        {
            if (currentMovementType.perfectBoost)
            {
                Gizmos.color = Color.green;
            }
            else if (currentMovementType.isBoosting)
            {
                Gizmos.color = Color.yellow;
            }
            else if (currentMovementType.swimBlock)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.gray;
            }
        }
        else
        {
            Gizmos.color = Color.gray;
        }

        Gizmos.DrawLine(leftFin.position, leftFinDirectionHandle.position);
        Gizmos.DrawSphere(leftFinDirectionHandle.position, 0.05f);

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
        m_player.OnBoostDown.AddListener(BuildUpBoost);
        m_player.OnBoostUp.AddListener(ReleaseBoost);
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
                currentMovementType = topDownMovementType;
                break;
            case MovementTypeEnum.Forward:
                currentMovementType = forwardMovementType;
                CameraManager.instance.ActiveThirdPersonCamera();
                break;
            case MovementTypeEnum.Free:
                currentMovementType = freeMovementType;
                CameraManager.instance.ActiveThirdPersonCamera();
                break;
            default:
                CameraManager.instance.ActiveThirdPersonCamera();
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

    /// <summary>
    /// Moves the turtle based on the input provided and applies a continuous force in the direction of rotation.
    /// </summary>
    /// <param name="_input">The input vector used to calculate the target rotation.</param>
    public void Move(float _swimInput, Vector2 _movementInput)
    {
        if (currentMovementType is null)
        {
            return;
        }

        currentMovementType.ApplyTorque(_movementInput, myRigidbody);
        currentMovementType.ApplyConstantForce(_swimInput, _movementInput, myRigidbody);
    }

    public void BuildUpBoost()
    {
        if (currentMovementType is null)
        {
            return;
        }

        //Start coroutine which over time increases boost up until max boost is reached
        if (!currentMovementType.isBoosting && !currentMovementType.swimBlock)
        {
            m_buildUpBoostRoutine = StartCoroutine(currentMovementType.BuildUpBoostRoutine());
            m_player.turtleAnimator.SetTrigger(Constants.AnimatorPull);
        }
    }

    private Coroutine m_buildUpBoostRoutine;

    public void ReleaseBoost()
    {
        if (currentMovementType is null)
        {
            return;
        }

        //Stop BuildUpBoost Routine
        if (m_buildUpBoostRoutine != null)
        {
            StopCoroutine(m_buildUpBoostRoutine);
            m_buildUpBoostRoutine = null;
        }

        //If build up boost is above threshhold
        if (currentMovementType.isBoosting && currentMovementType.boostValue > currentMovementType.boostThreshold)
        {
            //Call SwimRoutine with new build up forceStrength
            StartCoroutine(currentMovementType.BoostRoutine(leftFin, leftFinDirectionHandle, myRigidbody));
            StartCoroutine(currentMovementType.BoostRoutine(rightFin, rightFinDirectionHandle, myRigidbody));
            m_player.turtleAnimator.SetTrigger(Constants.AnimatorPush);
        }
        else
        {
            //Reset boost
            currentMovementType.boostValue = 0;
        }

        currentMovementType.perfectBoost = false;
        currentMovementType.isBoosting = false;
    }
}