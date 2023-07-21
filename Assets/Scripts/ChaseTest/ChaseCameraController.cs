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
    public AnimationCurve PositionCurve;
    public float PositionAdjustSpeed = 3;
    public float ScaleMargin = 6;
    public AnimationCurve ScaleCurve;
    public float ScaleAdjustSpeed = 3;

    public float
    maxScale,
    minScale;

    private void Update()
    {
        Vector3 distance = (prey.transform.localPosition - predator.transform.localPosition);
        float turtleDistance = distance.magnitude;

        Vector3 rawMidPoint = (prey.transform.localPosition + predator.transform.localPosition) * 0.5f;
        Vector3 ZMidPoint = Vector3.ProjectOnPlane(rawMidPoint, prey.chase.right);

        Vector3 pos = Vector3.Lerp(rawMidPoint,ZMidPoint, PositionCurve.Evaluate(turtleDistance / PositionMargin));
        Vector3 posDir = pos - transform.localPosition;
        transform.localPosition += posDir.normalized * Mathf.Min(PositionAdjustSpeed * Time.deltaTime, posDir.magnitude);

        float targetFOV = Mathf.Lerp(minScale, maxScale, ScaleCurve.Evaluate(turtleDistance / ScaleMargin));
        float FOVdiff = targetFOV - cam.m_Lens.FieldOfView;
        cam.m_Lens.FieldOfView += Mathf.Min(Mathf.Sign(FOVdiff) * ScaleAdjustSpeed*Time.deltaTime, FOVdiff);

    }
}
