using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseObstacleSpawner : MonoBehaviour
{
    public ChaseControl chase;

    public ChaseObstacle obstaclePrefabBase;
    public ChaseObstacle obstaclePrefabRare1;
    [Range(0, 0.5f)] public float rare1Probability;
    public ChaseObstacle obstaclePrefabRare2;
    [Range(0, 0.5f)] public float rare2Probability;

    public List<ChaseObstacle> obstacles = new List<ChaseObstacle>();
    public float obstacleCooldown;
    float obstacleCooldownTime;

    private void Start()
    {
        chase.OnEndChase += OnDeactivateAll;
        chase.OnStartChase += OnDeactivateAll;
    }
    public void OnDeactivateAll()
    {
        for (int i = 0; i < obstacles.Count; i++)
            obstacles[i].Active = false;
    }

    private void FixedUpdate()
    {
        if (chase.inChase)
        {
            obstacleCooldownTime -= Time.deltaTime;
            if (obstacleCooldownTime < 0)
            {
                obstacleCooldownTime = obstacleCooldown;
                ChaseObstacle obs = GetFreeObstacle();
                obs.Spawn();
            }
        }
    }

    ChaseObstacle GetFreeObstacle()
    {
        List<ChaseObstacle> obstacles = new List<ChaseObstacle>();
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (!obstacles[i].Active)
                obstacles.Add(obstacles[i]);
        }
        if (obstacles.Count != 0)
            return obstacles[Random.Range(0,obstacles.Count)];

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
