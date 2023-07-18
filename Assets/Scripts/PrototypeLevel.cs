using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeLevel : MonoBehaviour
{
    public Transform spawnPoint;
    public Rigidbody playerRigidbody;

    [ContextMenu("JumpToLevel")]
    public void JumpToLevel()
    {        
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
        playerRigidbody.position = spawnPoint.position;
    }
}