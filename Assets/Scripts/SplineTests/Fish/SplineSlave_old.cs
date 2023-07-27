using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;

public class SplineSlave_old : MonoBehaviour
{
    public SplineContainer spline;
    private SplineAnimate lure;

    [Header("Movement")]
    [Range(0, 1)] public float startOffset;
    public float movementSpeed = 1;
    public Vector2 excentricityForce = Vector2.one;
    public Vector2 excentricitySpeed = Vector2.one;

    [Header("Progress")]
    public float progress_readonly;
    public float Progress
    {
        get
        {
            // Calculate the progress as a ratio of elapsed time to duration
            if (lure != null)
            {
                return lure.ElapsedTime / lure.Duration;
            }

            return 0;
        }
        set
        {
            // Set the elapsed time based on the desired progress value
            if (lure != null)
            {
                lure.ElapsedTime = lure.Duration * value;
            }
        }
    }

    private void Awake()
    {
        // Get the SplineAnimate component attached to this GameObject
        lure = GetComponent<SplineAnimate>();

        // Create a new SplineAnimate component and assign it to lure variable
        lure = CreateLure();

        // Start playing the animation
        lure.Play();
    }

    private void Update()
    {
        // Get the target position and rotation
        (Vector3 position, Quaternion rotation) targetTrans = GetTargetTrans();

        // Update the position of this GameObject
        transform.position = targetTrans.position;

        // Update the readonly progress value
        progress_readonly = Progress;

        // Update the rotation of this GameObject
        transform.rotation = targetTrans.rotation;
    }

    public (Vector3 position, Quaternion rotation) GetTargetTrans()
    {
        // Initialize position, nextPosition, and rotation variables
        Vector3 position = transform.position;
        Vector3 nextPosition = transform.position + transform.forward * Time.deltaTime;
        Quaternion rotation = transform.rotation;

        if (lure != null)
        {
            // Update position and nextPosition based on spline animation
            position = lure.transform.position;
            nextPosition = lure.transform.position + lure.transform.forward * Time.deltaTime;

            // Apply excentricity forces to create movement variations
            position += Vector3.up * excentricityForce.y * Mathf.Sin(Time.time * excentricitySpeed.y);
            position += Vector3.right * excentricityForce.x * Mathf.Sin(Time.time * excentricitySpeed.x);

            nextPosition += Vector3.up * excentricityForce.y * Mathf.Sin((Time.time + Time.deltaTime) * excentricitySpeed.y);
            nextPosition += Vector3.right * excentricityForce.x * Mathf.Sin((Time.time + Time.deltaTime) * excentricitySpeed.x);

            // Calculate the rotation towards the nextPosition
            rotation = Quaternion.LookRotation(nextPosition - position);
            rotation = Quaternion.Slerp(transform.rotation, rotation, 0.8f);
        }

        // Return the target position and rotation
        return (position, rotation);
    }

    public SplineAnimate CreateLure()
    {
        // Create a new GameObject for the lure
        GameObject lureObject = Instantiate(new GameObject(name + "_Lure"), spline.transform);

        // Add a SplineAnimate component to the lure GameObject
        SplineAnimate lure = lureObject.AddComponent<SplineAnimate>();

        // Set the properties of the SplineAnimate component
        lure.Container = spline;
        lure.StartOffset = startOffset;
        lure.AnimationMethod = SplineAnimate.Method.Speed;
        lure.MaxSpeed = movementSpeed;

        // Return the created SplineAnimate component
        return lure;
    }
}
