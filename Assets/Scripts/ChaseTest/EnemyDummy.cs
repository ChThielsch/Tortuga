using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummy : MonoBehaviour
{
    public TurtleController prey;
    Rigidbody rb;
    public float
        chasePushForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (rb != null && prey != null && prey.movementType == TurtleController.MovementType.Chase)
            rb.AddForce(Vector3.forward * chasePushForce, ForceMode.Acceleration);
    }
}
