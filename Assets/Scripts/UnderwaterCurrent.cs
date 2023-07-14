using System.Collections.Generic;
using UnityEngine;

public class UnderwaterCurrent : MonoBehaviour
{
    public float defaultForce = 10f;
    [ShowOnly] public float currentForce;
    [Range(0, 5)]
    public float minNoiseMultiplier = 1f;
    [Range(1, 5)]
    public float maxNoiseMultiplier = 1f;
    public float minNoiseTransitionTime = 1f;
    public float maxNoiseTransitionTime = 3f;

    private float m_targetNoise;
    [ShowOnly] private float m_currentNoise;
    private float m_noiseTransitionTime;
    private float m_transitionTimer;

    private void Start()
    {
        m_targetNoise = maxNoiseMultiplier;
        m_currentNoise = minNoiseMultiplier;
        GenerateNewTransitionTime();
        GenerateNewTargetNoise();
    }

    private void FixedUpdate()
    {
        m_currentNoise = Mathf.Lerp(m_currentNoise, m_targetNoise, m_transitionTimer / m_noiseTransitionTime);
        m_transitionTimer += Time.fixedDeltaTime;

        if (Mathf.Approximately(m_currentNoise, m_targetNoise))
        {
            GenerateNewTransitionTime();
            GenerateNewTargetNoise();
        }

        currentForce = defaultForce * m_currentNoise;
    }

    private void GenerateNewTransitionTime()
    {
        m_noiseTransitionTime = Random.Range(minNoiseTransitionTime, maxNoiseTransitionTime);
        m_transitionTimer = 0f;
    }

    private void GenerateNewTargetNoise()
    {
        m_targetNoise = Random.Range(minNoiseMultiplier, maxNoiseMultiplier);
    }
}