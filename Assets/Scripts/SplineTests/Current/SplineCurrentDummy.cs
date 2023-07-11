using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineCurrentDummy : MonoBehaviour
{
    public float speed=5;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 vec = SplineCurrentLink.GetAffectedDirection(transform.position);
        rb.velocity = vec*speed;
        transform.rotation = Quaternion.LookRotation(vec);
    }
}
