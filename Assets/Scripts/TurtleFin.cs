using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleFin : MonoBehaviour
{
    public Transform forceDirectionTransform;
    public float freeForceStrength;
    public ForceMode FreeForceMode;
    public float topDownForceStrength;
    public ForceMode TopDownForceMode;
    public float swimDuration = 1.25f;
    public AnimationCurve paddleCurve;
    public TurtleController turtleController;

    private ElementTransition m_swimAnimation;
    private Coroutine m_forceCoroutine;

    private void Awake()
    {
        m_swimAnimation = GetComponent<ElementTransition>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, forceDirectionTransform.position);
        Gizmos.DrawSphere(forceDirectionTransform.position, 0.05f);
    }

    public void Swim()
    {
        if (m_forceCoroutine is null)
        {
            m_forceCoroutine = StartCoroutine(SwimRoutine());
            m_swimAnimation.JumpToPosition(0);
            m_swimAnimation.PlayAnimation(1);
        }
    }

    public IEnumerator SwimRoutine()
    {
        float elapsedTime = 0f;
        float initialForceStrength = 0f; // Initial force strength

        while (elapsedTime < swimDuration)
        {
            float normalizedTime = elapsedTime / swimDuration;
            float curveValue = paddleCurve.Evaluate(normalizedTime); // Evaluate the animation curve at the normalized time


            if (turtleController.movementType == TurtleController.MovementType.TopDown||turtleController.movementType==TurtleController.MovementType.Chase)
            {
                float currentForceStrength = Mathf.Lerp(initialForceStrength, topDownForceStrength, curveValue);
                // Apply force in the direction the turtle is facing
                turtleController.myRigidbody.AddForce(turtleController.transform.forward * currentForceStrength, TopDownForceMode);
            }
            else
            {
                float currentForceStrength = Mathf.Lerp(initialForceStrength, freeForceStrength, curveValue);
                // Apply force in the force direction
                turtleController.myRigidbody.AddForceAtPosition(CalculateForce(transform, forceDirectionTransform) * currentForceStrength, transform.position, FreeForceMode);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        m_forceCoroutine = null;
    }

    private Vector3 CalculateForce(Transform sourceTransform, Transform targetTransform)
    {
        // Calculate the direction between the two transforms
        Vector3 direction = targetTransform.position - sourceTransform.position;

        // Normalize the direction vector
        direction.Normalize();

        // Return the normalized direction as a force
        return direction;
    }
}