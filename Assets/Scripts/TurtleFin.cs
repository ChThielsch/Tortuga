using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleFin : MonoBehaviour
{
    public Vector3 forceDirection;
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
        DrawArrow.ForGizmo(transform.position, transform.position + forceDirection, Color.green);
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
        float duration = 1f; // Duration in seconds
        float initialForceStrength = 0f; // Initial force strength

        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;
            float curveValue = _curve.Evaluate(normalizedTime); // Evaluate the animation curve at the normalized time

            float currentForceStrength = Mathf.Lerp(initialForceStrength, forceStrength, curveValue);

            _rigidbody.AddForceAtPosition(forceDirection.normalized * currentForceStrength, transform.position, ForceMode.Acceleration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        m_forceCoroutine = null;
    }
}
