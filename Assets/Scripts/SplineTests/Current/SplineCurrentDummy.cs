using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineCurrentDummy : MonoBehaviour
{
    public float speed = 5;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 targetRotation = SplineCurrentLink.GetAffectedDirection(transform.position).direction;
        rb.velocity = targetRotation * speed;
        if (targetRotation != Vector3.zero)
        {
        transform.rotation = Quaternion.LookRotation(targetRotation);
        }
    }
}