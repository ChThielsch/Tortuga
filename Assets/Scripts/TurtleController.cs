using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleController : MonoBehaviour
{
    public Rigidbody myRigidbody;
    public TurtleFin leftFin;
    public TurtleFin rightFin;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(myRigidbody.centerOfMass, 0.05f);
    }

    public void Swim()
    {
        leftFin.Swim(myRigidbody);
        rightFin.Swim(myRigidbody);
    }

    public void Move(Vector2 _input)
    {

    }
}