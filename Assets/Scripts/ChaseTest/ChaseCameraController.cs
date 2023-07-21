using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ChaseCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera cam;
    public ChasePredatorController predator;
    public ChasedTurtleController prey;

    public float PositionMargin = 5;
    public float ScaleMargin = 6;
    public float
        maxScale,
        minScale;

    private void Update()
    {
        float turtleDistance = (prey.transform.position - predator.transform.position).magnitude;

        Vector3 middlePosition = Vector3.Lerp(Vector3.zero, predator.transform.localPosition, 0.75f);
        Vector3 averagePosition = Vector3.Lerp(middlePosition, Vector3.zero,Mathf.Min(PositionMargin,turtleDistance/PositionMargin));

        cam.m_Lens.FieldOfView = Mathf.Lerp(minScale, maxScale, Mathf.Min(ScaleMargin, turtleDistance / ScaleMargin));

        transform.localPosition = averagePosition;
    }
}
