using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ChaseControl : MonoBehaviour
{
    [HideInInspector] public SplineAnimate rail;
    public bool inChase;
    public float chaseTime;

    public delegate void ChaseDelegate();
    public ChaseDelegate OnStartChase, OnEndChase;

    public Vector3 forward => transform.forward;
    public Vector3 right => transform.right;
    public Vector3 up => transform.up;

    public float lerp;

    public ChaseObstacle obstaclePrefabBase;
    public ChaseObstacle obstaclePrefabRare1;
    [Range(0,0.5f)]public float rare1Probability;
    public ChaseObstacle obstaclePrefabRare2;
    [Range(0, 0.5f)] public float rare2Probability;

    public List<ChaseObstacle> obstacles = new List<ChaseObstacle>();
    public float obstacleCooldown;
    float obstacleCooldownTime;

    private void Awake()
    {
        rail= GetComponent<SplineAnimate>();
    }

    private void Start()
    {
        Invoke("StartChase", 1);
    }

    public void StartChase()
    {        
        chaseTime = 0;
        rail.Restart(false);
        rail.Play();
        inChase= true;
        OnStartChase?.Invoke();

        Debug.Log("Start Chase");
    }
    public void StopChase()
    {
        for (int i = 0; i < obstacles.Count; i++)
            obstacles[i].Active = false;

        rail.Pause();
        rail.Restart(false);
        chaseTime = 0;
        rail.ElapsedTime = 0;
        inChase = false;
        OnEndChase?.Invoke();

        Debug.Log("Stop Chase");
    }

    private void FixedUpdate()
    {
        if (inChase)
        {
            chaseTime += Time.fixedDeltaTime;
            lerp = rail.ElapsedTime / rail.Duration;

            obstacleCooldownTime -= Time.deltaTime;
            if (obstacleCooldownTime<0)
            {
                obstacleCooldownTime = obstacleCooldown;
                ChaseObstacle obs = GetFreeObstacle();
                obs.Spawn();
            }
        }
    }

    ChaseObstacle GetFreeObstacle()
    {
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (!obstacles[i].Active)
                return obstacles[i];
        }

        ChaseObstacle prefab = obstaclePrefabBase;
        float roll = Random.value;
        if (roll < rare1Probability) prefab = obstaclePrefabRare1;
        else roll -= rare1Probability;
        if (roll < rare2Probability) prefab = obstaclePrefabRare2;

        ChaseObstacle obs = Instantiate(prefab, transform.position + transform.forward * 15, Quaternion.identity, transform);
        obstacles.Add(obs);
        return obs;
    }

}
