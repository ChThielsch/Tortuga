using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleDiveInfo : MonoBehaviour
{
    [Header("Stats")]
    public float depth;
    public Vector2 position, moveDirection;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        position= new Vector2 (transform.position.x,transform.position.z);
        moveDirection = new Vector2(rb.velocity.x, rb.velocity.z);
        depth = transform.position.y;

        Debug.DrawRay(transform.position, moveDirection, Color.green, Time.fixedDeltaTime);
        Debug.DrawLine(position, transform.position, Color.blue, Time.fixedDeltaTime);
    }
}
