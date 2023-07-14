using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineCurrentLink : MonoBehaviour
{
    public static List<SplineCurrentLink> affectedLinks = new List<SplineCurrentLink>();

    /// <summary>
    /// Calculates the affected direction based on a start position.
    /// </summary>
    /// <param name="_startPosition">The start position for calculating the affected direction.</param>
    /// <returns>The affected direction vector.</returns>
    public static Vector3 GetAffectedDirection(Vector3 _startPosition)
    {
        if (affectedLinks.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 direction = Vector3.zero;
        float magnitude = 0;

        for (int i = 0; i < affectedLinks.Count; i++)
        {
            Vector3 linkDir = affectedLinks[i].GetCurrentDirection(_startPosition);
            direction += linkDir;
            magnitude += linkDir.magnitude;
        }
        direction = direction.normalized * (magnitude / affectedLinks.Count);

        return direction;
    }

    internal Vector3 GetCurrentDirection(Vector3 _startPosition) => (transform.forward + Vector3.ProjectOnPlane(transform.position - _startPosition, transform.forward)).normalized;

    private void Awake()
    {
        affectedLinks = new List<SplineCurrentLink>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!affectedLinks.Contains(this) && other.GetComponent<Rigidbody>())
            affectedLinks.Add(this);
    }
    private void OnTriggerExit(Collider other)
    {
        if (affectedLinks.Contains(this) && other.GetComponent<Rigidbody>())
            affectedLinks.Remove(this);
    }
}