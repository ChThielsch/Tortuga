using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseDeathWall : MonoBehaviour
{
    public ChaseControl chase;
    public ChasedTurtleController prey;
 
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == prey.gameObject)
        {
            chase.StopChase();
            chase.Invoke("StartChase", 1);
            return;
        }
    }
}
