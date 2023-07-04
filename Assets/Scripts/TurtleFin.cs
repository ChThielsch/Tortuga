using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleFin : MonoBehaviour
{
    public Vector3 forceDirection;
    public float forceStrength;


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
            m_forceCoroutine = StartCoroutine(SwimRoutine(_rigidbody));
            m_swimAnimation.JumpToPosition(0);
            m_swimAnimation.PlayAnimation(1);
        }
    }

    public IEnumerator SwimRoutine(Rigidbody _rigidbody)
    {
        float elapsedTime = 0f;
        float duration = 1f; // Duration in seconds
        float initialForceStrength = 0f; // Initial force strength

        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;
            float currentForceStrength = Mathf.Lerp(initialForceStrength, forceStrength, normalizedTime);

            _rigidbody.AddForceAtPosition(forceDirection.normalized * currentForceStrength, transform.position, ForceMode.Acceleration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        m_forceCoroutine = null;
    }
}
