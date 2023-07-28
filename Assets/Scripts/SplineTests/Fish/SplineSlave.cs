using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;

[RequireComponent(typeof(SplineContainer))]
public class SplineSlave : MonoBehaviour
{
    [HideInInspector] public SplineSlavePrefab prefab;
    SplineContainer spline;
    SplineAnimate lure;
    Transform hook;
    public CloudFine.FlockBox.FlockBox box;

    SplineSlavePrefab slave;

    [Divider("Settings")]
    [Tooltip("Movement Only as SplineSlave and potential swarm members will always instantiate on awake!")]
    public bool playOnAwake;

    [Header("Lure")]
    [Tooltip("Part of the first loop that will be skipped.")]
    [SerializeField] [Range(0, 1)] float startPathOffset;

    [Space]
    [Range(0, 5)] public float moveSpeed;
    [Tooltip("Applies periodic change to Lure's speed along spline.")]
    public AnimationCurve speedOffsetCurve;
    [Tooltip("Speed at which offset is applied.")]
    [Range(0, 1)] public float speedOffsetSpeed;
    [Tooltip("Strength of the applied offset.")]
    [Range(0, 5)] public float speedOffsetValue;

    [Space]
    [Header("Hook")]
    [Tooltip("Applies periodic change to Hooks local X position. (left-right)")]
    public AnimationCurve XoffsetCurve;
    [Tooltip("Speed at which offset is applied.")]
    [Range(0, 1)] public float XoffsetSpeed;
    [Tooltip("Strength of the applied offset.")]
    [Range(0, 3f)] public float XoffsetDistance;

    [Space]
    [Tooltip("Applies periodic change to Hooks local Y position. (down-up)")]
    public AnimationCurve YoffsetCurve;
    [Tooltip("Speed at which offset is applied.")]
    [Range(0, 1)] public float YoffsetSpeed;
    [Tooltip("Strength of the applied offset.")]
    [Range(0, 3f)] public float YoffsetDistance;

    [Space]
    [Tooltip("Applies periodic change to Hooks local Z position. (back-forward)")]
    public AnimationCurve ZoffsetCurve;
    [Tooltip("Speed at which offset is applied.")]
    [Range(0, 1)] public float ZoffsetSpeed;
    [Tooltip("Strength of the applied offset." +
        "\nHas a lower cap as higher offset here results in drifting through spline curves.")]
    [Range(0, 1.5f)] public float ZoffsetDistance;

    [Header("Swarms")]
    [Tooltip("The Type of swarm member that will be instantiated in the FlockBox." +
        "\nLeave this empty to ingore the feature.")]
    public CloudFine.FlockBox.SteeringAgent agentPrefab;
    [Tooltip("Number of swarm members that will be instantiated")]
    public int swarmSize;
    [Tooltip("Tag of the Hook as well as Tag the swarm will be attracted to." +
        "\nNote that this overrides something in the swarms Behaviour ScriptableObject!")]
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