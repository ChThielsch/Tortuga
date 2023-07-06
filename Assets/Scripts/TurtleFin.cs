using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleFin : MonoBehaviour
{
    public Transform forceDirectionTransform;
    public float forceStrength;
    public AnimationCurve paddleCurve;


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

    public void Swim(Rigidbody _rigidbody)
    {
        if (m_forceCoroutine is null)
        {
            m_forceCoroutine = StartCoroutine(SwimRoutine(_rigidbody, paddleCurve));
            m_swimAnimation.JumpToPosition(0);
            m_swimAnimation.PlayAnimation(1);
        }
    }

    public IEnumerator SwimRoutine(Rigidbody _rigidbody, AnimationCurve _curve)
    {
        float elapsedTime = 0f;
        float duration = 1.25f; // Duration in seconds
        float initialForceStrength = 0f; // Initial force strength

        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;
            float curveValue = _curve.Evaluate(normalizedTime); // Evaluate the animation curve at the normalized time

            float currentForceStrength = Mathf.Lerp(initialForceStrength, forceStrength, curveValue);

            _rigidbody.AddForceAtPosition(CalculateForce(transform, forceDirectionTransform) * currentForceStrength, transform.position, ForceMode.Acceleration);

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