using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurtleDiveInfo : MonoBehaviour
{
    [Header("Stats")]
    public float depth;
    public Vector2 position, moveDirection;

    public TMP_Text depthText;
    public TMP_Text pressureText;
    public TMP_Text speedText;

    public Rigidbody rb;

    private void FixedUpdate()
    {
        position = new Vector2(rb.position.x, rb.position.z);
        moveDirection = new Vector2(rb.velocity.x, rb.velocity.z);
        depth = rb.position.y;

        Debug.DrawRay(transform.position, moveDirection, Color.green, Time.fixedDeltaTime);
        Debug.DrawLine(position, transform.position, Color.blue, Time.fixedDeltaTime);

        depthText.text = $"{depth.ToString("F0")} m";
        pressureText.text = CalculatePressure(depth);
        speedText.text = CalculateKmPerHour(rb.velocity);
    }

    public string CalculatePressure(float depth)
    {
        float waterDensity = 1000f; // Density of water in kg/m³
        float gravity = 9.81f; // Acceleration due to gravity in m/s²

        float pressure = waterDensity * gravity * depth;
        float pressureInBars = pressure * -0.00001f; // Convert Pascal to Bar

        return pressureInBars.ToString("F2") + " bar";
    }

    public string CalculateKmPerHour(Vector3 _velocity)
    {
        float velocityMagnitude = _velocity.magnitude;
        float velocityInKmPerHour = velocityMagnitude * 3.6f; // Conversion factor from m/s to km/h

        return velocityInKmPerHour.ToString("F2") + " km/h";
    }
}
