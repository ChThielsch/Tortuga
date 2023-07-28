using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineSlavePrefab : MonoBehaviour
{
    [Range(0, 5)] public float moveSpeed;
    public AnimationCurve speedOffsetCurve;
    [Range(0, 1)] public float speedOffsetSpeed;
    [Range(0, 5)] public float speedOffsetValue;

    [Header("Hook")]
    public AnimationCurve XoffsetCurve;
    [Range(0, 1)] public float XoffsetSpeed;
    [Range(0, 3f)] public float XoffsetDistance;
    public AnimationCurve YoffsetCurve;
    [Range(0, 1)] public float YoffsetSpeed;
    [Range(0, 3f)] public float YoffsetDistance;
    public AnimationCurve ZoffsetCurve;
    [Range(0, 1)] public float ZoffsetSpeed;
    [Range(0, 1.5f)] public float ZoffsetDistance;

    [Header("For Swarms only. Will be ignored if prefab is null.")]
    public CloudFine.FlockBox.SteeringAgent agentPrefab;
    public int swarmSize;
    [Range(0,2)]public int lureTag;
}
