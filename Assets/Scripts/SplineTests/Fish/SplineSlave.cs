using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;

public class SplineSlave : MonoBehaviour
{
    public SplineContainer spline;
    SplineAnimate lure;

    [Header("Movement")]
    [Range(0,1)]public float startOffset;
    public float movementSpeed=1;
    public Vector2
        excentricityForce= Vector2.one,
        excentricitySpeed= Vector2.one;

    [Header("Progress")]
    public float progress_readonly;
    public float Progress
    {
        get => lure? lure.ElapsedTime / lure.Duration : 0;
        set => lure.ElapsedTime = lure.Duration * value;
    }

    public (Vector3, Quaternion) GetTargetTrans()
    {
        Vector3
            pos = transform.position,
            nextPos = transform.position+transform.forward*Time.deltaTime;
        Quaternion rot= transform.rotation;
        if (lure)
        {
            pos= lure.transform.position;
            nextPos= lure.transform.position+lure.transform.forward*Time.deltaTime;

            pos += Vector3.up * excentricityForce.y * Mathf.Sin(Time.time * excentricitySpeed.y);
            pos += Vector3.right * excentricityForce.x * Mathf.Sin(Time.time * excentricitySpeed.x);

            nextPos += Vector3.up * excentricityForce.y * Mathf.Sin((Time.time+Time.deltaTime) * excentricitySpeed.y);
            nextPos += Vector3.right * excentricityForce.x * Mathf.Sin((Time.time+Time.deltaTime) * excentricitySpeed.x);

            rot = Quaternion.LookRotation(nextPos - pos/*, lure.transform.up*/);
            rot = Quaternion.Slerp(transform.rotation,rot,0.8f);
        }

        return (pos, rot);
    }
    private Vector3 previousPosition;


    private void Awake()
    {
        lure = GetComponent<SplineAnimate>();

        lure= CreateLure();
        lure.Play();
        previousPosition = transform.position;
    }

    private void Update()
    {
        (Vector3, Quaternion) targetTrans= GetTargetTrans();
        transform.position = targetTrans.Item1;
        progress_readonly = Progress;

        //Vector3 direction = (transform.position - previousPosition).normalized;
        //Quaternion targetRotation = transform.rotation;
        //if (direction != Vector3.zero)
        //    targetRotation = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 100f);

        transform.rotation = targetTrans.Item2;

        previousPosition = transform.position;
    }

    public SplineAnimate CreateLure()
    {
        GameObject lureObject = Instantiate(new GameObject(name+"Lure"),spline.transform);
        SplineAnimate lure= lureObject.AddComponent<SplineAnimate>();
        lure.Container = spline;
        lure.StartOffset = startOffset;
        lure.AnimationMethod = SplineAnimate.Method.Speed;
        lure.MaxSpeed = movementSpeed;

        return lure;
    }
}
