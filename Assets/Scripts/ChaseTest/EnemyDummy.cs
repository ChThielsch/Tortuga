using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummy : MonoBehaviour
{

    public PrototypeLevel level;
    public TurtleController prey;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        bool valid = rb != null && prey != null && prey.movementType == TurtleController.MovementType.Chase && prey.chaseWaitTime <= 0;
        if (valid)
        {
            Vector3
               chaseDirection = prey.currentDirection != Vector3.zero ? prey.currentDirection : Vector3.forward,
               chaseRight = Vector3.Cross(Vector3.up, chaseDirection),
               distance = prey.transform.position - transform.position;

            distance = Vector3.ProjectOnPlane(distance, chaseDirection);

            Vector3 force = chaseDirection * prey.chaseDefaultForce * 100 + distance.normalized * prey.chaseSlideForce * 50;
            rb.AddForce(force);
            transform.LookAt(prey.transform.position);
        }
        else
        {
            rb.position = level.spawnPoint.position + Vector3.back * prey.chaseNormalZDistance;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

    }
}
