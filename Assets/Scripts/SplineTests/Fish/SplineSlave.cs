using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;

[RequireComponent(typeof(SplineContainer))]
public class SplineSlave : MonoBehaviour
{
    [HideInInspector]public SplineSlavePrefab prefab;
    SplineContainer spline;
    SplineAnimate lure;
    Transform hook;
    public CloudFine.FlockBox.FlockBox box;

    SplineSlavePrefab slave;

    [Divider("Settings")]
    public bool playOnAwake;

    [Header("Lure")]
    [SerializeField][Range(0,1)] float startPathOffset;

    [Range(0,5)] public float moveSpeed;
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

    [Header("Swarms")]
    public CloudFine.FlockBox.SteeringAgent agentPrefab;
    public int swarmSize;
    [Range(0, 2)] public int lureTag;

    [Header("Status")]
    [ShowOnly]bool isPlaying;
    float 
        travelTime,
        loopTime,
        elapsedLoopTime;



    private void Awake()
    {
        if (prefab == null)
        {
            Debug.LogError("NullReferenceException: Unassigned SplineSlavePrefab on "+name+". \nDid you mayhaps forget to assign it?");
            return;
        }
        spline = GetComponent<SplineContainer>();
        lure = GetComponentInChildren<SplineAnimate>();
        hook = lure.transform.GetChild(0);

        SetUp();

        if (playOnAwake) Play();
    }
    private void Start()
    {
        loopTime = lure.Duration;
        elapsedLoopTime = startPathOffset*loopTime;
    }
    private void FixedUpdate()
    {
        if (isPlaying)
        {
            travelTime += Time.fixedDeltaTime;
            AnimateLure();
            AnimateHook();
        }
    }

    public void SetUp()
    {
        lure.Container = spline;
        lure.PlayOnAwake = false;
        lure.StartOffset = 0;
        lure.AnimationMethod = SplineAnimate.Method.Speed;
        lure.MaxSpeed = 1;

        slave = Instantiate(prefab, hook.position, hook.rotation, hook);

        string tag = $"Lure_{lureTag}";
        hook.tag = tag;

        if (agentPrefab)
        {
            for (int i = 0; i < swarmSize; i++)
            {
                CloudFine.FlockBox.SteeringAgent a = Instantiate(agentPrefab, hook.position , Random.rotation, box.transform);
                a.activeSettings.GetBehavior<CloudFine.FlockBox.PursuitBehavior>().filterTags = new string[] { tag };
                a.Spawn(box, hook.position + Random.onUnitSphere,true);
            }
        }
    }
    public void AnimateLure()
    {
        elapsedLoopTime += (moveSpeed + speedOffsetCurve.Evaluate(travelTime * speedOffsetSpeed) * speedOffsetValue) * Time.fixedDeltaTime;
        elapsedLoopTime %= loopTime;
        lure.ElapsedTime = elapsedLoopTime;
    }
    public void AnimateHook()
    {
        Vector3 pos = Vector3.one;

        pos.x *= XoffsetCurve.Evaluate(travelTime * XoffsetSpeed)*XoffsetDistance;
        pos.y *= YoffsetCurve.Evaluate(travelTime * YoffsetSpeed)*YoffsetDistance;
        pos.z *= ZoffsetCurve.Evaluate(travelTime * ZoffsetSpeed)*ZoffsetDistance;

        hook.localPosition = pos;

        Debug.DrawLine(lure.transform.position, hook.transform.position, Color.blue);
    }

    public void Play() //Plays
    {
        isPlaying = true;
    }
    public void Pause() //Pauses
    {
        isPlaying = false;
    }
    public void Restart(bool startPlaying=true)
    {
        elapsedLoopTime = 0;
        lure.ElapsedTime = 0;
        isPlaying = startPlaying;
    }

    public void SetSlaveProfile(SplineSlavePrefab s)
    {
        moveSpeed = s.moveSpeed;
        speedOffsetCurve = s.speedOffsetCurve;
        speedOffsetSpeed = s.speedOffsetSpeed;
        speedOffsetValue = s.speedOffsetValue;
 
        XoffsetCurve = s.XoffsetCurve;
        XoffsetSpeed = s.YoffsetSpeed;
        XoffsetDistance = s.XoffsetDistance;

        YoffsetCurve = s.YoffsetCurve;
        YoffsetSpeed = s.YoffsetSpeed;
        YoffsetDistance = s.YoffsetDistance;

        ZoffsetCurve = s.ZoffsetCurve;
        ZoffsetSpeed = s.ZoffsetSpeed;
        ZoffsetDistance = s.ZoffsetDistance;

        agentPrefab = s.agentPrefab;
        swarmSize = s.swarmSize;
        lureTag = s.lureTag;
    }
}

[CustomEditor(typeof(SplineSlave))]
public class SplineSlaveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SplineSlave splineSlave = (SplineSlave)target;

        SplineSlavePrefab prefab = null;
        prefab = (SplineSlavePrefab)EditorGUI.ObjectField(EditorGUILayout.GetControlRect(), "Prefab", splineSlave.prefab, typeof(SplineSlavePrefab), false);
        if (prefab != splineSlave.prefab)
        {
            splineSlave.prefab = prefab;
            splineSlave.SetSlaveProfile(prefab);
        }
        else if (GUILayout.Button("Reset Overrides"))
        {
            splineSlave.SetSlaveProfile(prefab);
        }

        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Play"))
        {
            splineSlave.Play();
        }

        if (GUILayout.Button("Stop"))
        {
            splineSlave.Pause();
        }

        if (GUILayout.Button("Restart"))
        {
            splineSlave.Restart();
        }
    }
}