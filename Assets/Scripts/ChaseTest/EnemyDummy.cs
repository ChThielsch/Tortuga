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
        bool valid = rb != null && prey != null && prey.movementType == TurtleController.MovementType.Chase&& prey.chaseWaitTime<=0;
        if (valid)
        {
            rb.AddForce(Vector3.forward * prey.chasePushForce, ForceMode.Acceleration);
            rb.AddForce((Vector3.right*(prey.transform.position.x-transform.position.x)).normalized*prey.chaseSlideForce,ForceMode.Acceleration);
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
