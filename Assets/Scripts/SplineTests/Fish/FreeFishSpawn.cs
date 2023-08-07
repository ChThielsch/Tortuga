using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudFine.FlockBox;

public class FreeFishSpawn : MonoBehaviour
{
    public FlockBox box;

    [Tooltip("The Type of swarm member that will be instantiated in the FlockBox.")]
    public SteeringAgent agentPrefab;
    [Tooltip("Number of swarm members that will be instantiated")]
    public int swarmSize;

    private void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        for (int i = 0; i < swarmSize; i++)
        {
            SteeringAgent a = Instantiate(agentPrefab, transform.position, Random.rotation, box.transform);
            a.Spawn(box, transform.position + Random.onUnitSphere, true);
        }
    }
}
