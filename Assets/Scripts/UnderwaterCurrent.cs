using System.Collections.Generic;
using UnityEngine;

public class UnderwaterCurrent : MonoBehaviour
{
    public Vector3 currentDirection = Vector3.forward;
    public float currentForce = 10f;
    public float noise = 1f;
    public float minNoiseTransitionTime = 1f;
    public float maxNoiseTransitionTime = 3f;

    private float m_targetNoise;
    private float m_currentNoise;
    private float m_noiseTransitionTime;
    private float m_transitionTimer;
    private List<Rigidbody> m_affectedRigidbodies = new List<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigidbody = other.GetComponent<Rigidbody>();
        if (rigidbody != null && !m_affectedRigidbodies.Contains(rigidbody))
        {
            m_affectedRigidbodies.Add(rigidbody);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rigidbody = other.GetComponent<Rigidbody>();
        if (rigidbody != null && m_affectedRigidbodies.Contains(rigidbody))
        {
            m_affectedRigidbodies.Remove(rigidbody);
        }
    }

    private void Start()
    {
        m_targetNoise = noise;
        m_currentNoise = noise;
        GenerateNewTransitionTime();
    }

    private void FixedUpdate()
    {
        m_currentNoise = Mathf.Lerp(m_currentNoise, m_targetNoise, m_transitionTimer / m_noiseTransitionTime);
        m_transitionTimer += Time.fixedDeltaTime;

        ApplyCurrentForce();

        if (Mathf.Approximately(m_currentNoise, m_targetNoise))
        {
            GenerateNewTransitionTime();
            m_targetNoise = Random.Range(0f, noise);
        }
    }

    private void ApplyCurrentForce()
    {
        Vector3 currentForceModified = currentDirection.normalized * currentForce;

        foreach (Rigidbody rigidbody in m_affectedRigidbodies)
        {
            Vector3 forceToAdd = currentForceModified * (1f + Random.Range(-m_currentNoise, m_currentNoise));
            rigidbody.AddForce(transform.TransformDirection(forceToAdd), ForceMode.Force);
        }
    }

    private void GenerateNewTransitionTime()
    {
        m_noiseTransitionTime = Random.Range(minNoiseTransitionTime, maxNoiseTransitionTime);
        m_transitionTimer = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(currentDirection.normalized) * transform.localScale.magnitude);
    }
}