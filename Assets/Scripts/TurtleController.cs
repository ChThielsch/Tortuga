using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleController : MonoBehaviour
{
    public Rigidbody myRigidbody;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(myRigidbody.centerOfMass, 0.05f);
    }

    public void Swim()
    {

    }

    public void Move(Vector2 _input)
    {
        
    }
}