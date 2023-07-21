using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePredatorController : MonoBehaviour
{

    [Divider("References")]
    public ChaseControl chase;
    public ChasedTurtleController prey;

    [Divider("Parameters")]

    [Range(0,10)] public float startBaseDistance=8;
    [Range(0, 10)] public float endBaseDistance=6;
    public float baseDistance => Mathf.Lerp(startBaseDistance, endBaseDistance, chase.lerp);

    [Header("Rotation")]
    [Range(0, 30)] public float maxRotationAngle = 10;
    [Range(0, 15)] public float rotationSpeed = 5;

    [Header("Movement")]
    public float sideMoveSpeed;
    public float catchUpFrequency;
    public float 
        minCatchUpDistance,
        maxCatchUpDistance;


    [Divider("Stats")]
    [ShowOnly] [SerializeField] private float catchUpDistance;
    [ShowOnly] [SerializeField] private float moveSideDistance;
    [ShowOnly] [SerializeField] private float rotationAngle;

    private void Start()
    {
        ResetPosition();
        chase.OnStartChase += ResetPosition;
    }

    public void ResetPosition()
    {
        catchUpDistance = 0;
        moveSideDistance = 0;
        rotationAngle = 0;

        StopAllCoroutines();

        transform.localPosition = Vector3.back * baseDistance + Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (chase.inChase)
            Move();
    }

    public void Move()
    {
        float turtleDistance = prey.transform.localPosition.x - transform.localPosition.x;
        moveSideDistance += Mathf.Sign(turtleDistance) * sideMoveSpeed * Time.fixedDeltaTime;

        catchUpDistance =
            (Mathf.Sin(chase.chaseTime * catchUpFrequency) + 1) * 0.5f *
            Mathf.Lerp(minCatchUpDistance,maxCatchUpDistance,(Mathf.Sin(chase.chaseTime * catchUpFrequency*0.66f) + 1) * 0.5f );

        Vector3 sidePosition = Vector3.right * moveSideDistance;
        Vector3 advancePosition = Vector3.forward * catchUpDistance;

        Vector3 position = sidePosition + advancePosition;
        transform.localPosition = Vector3.back * baseDistance + position;


        rotationAngle += Mathf.Sign(turtleDistance) * rotationSpeed*Time.fixedDeltaTime;
        rotationAngle = Mathf.Clamp(rotationAngle, -maxRotationAngle, maxRotationAngle);

        Vector3 forward = Quaternion.AngleAxis(rotationAngle, chase.up) * chase.forward;
        Quaternion rotation = Quaternion.LookRotation(forward, chase.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.5f);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject==prey.gameObject)
        {
            chase.StopChase();
            chase.Invoke("StartChase", 1);
        }
    }
}
