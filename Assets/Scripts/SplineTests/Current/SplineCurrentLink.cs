using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineCurrentLink : MonoBehaviour
{
    //Static
    public static List<SplineCurrentLink> affectedLinks = new List<SplineCurrentLink>();
    public static Vector3 GetAffectedDirection(Vector3 fromPos)
    {
        if (affectedLinks.Count == 0) return Vector3.zero;

        Vector3 dir = Vector3.zero;
        float magnitude=0;

        for (int i = 0; i < affectedLinks.Count; i++)
        {
            Vector3 linkDir = affectedLinks[i].GetCurrentDirection(fromPos);
            dir += linkDir;
            magnitude+=linkDir.magnitude;
        }
        dir = dir.normalized*(magnitude/affectedLinks.Count);

        return dir;
    }

    //Internal
    public Vector3 GetCurrentDirection(Vector3 fromPos) => (transform.forward + Vector3.ProjectOnPlane(transform.position - fromPos, transform.forward)).normalized;


    private void Awake()
    {
        affectedLinks = new List<SplineCurrentLink>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SplineCurrentDummy>() && !affectedLinks.Contains(this)) 
            affectedLinks.Add(this);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<SplineCurrentDummy>() && affectedLinks.Contains(this))
            affectedLinks.Remove(this);
    }
}
