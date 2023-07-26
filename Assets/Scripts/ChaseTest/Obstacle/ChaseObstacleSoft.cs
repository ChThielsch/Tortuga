using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseObstacleSoft : ChaseObstacle
{
    public float dragStrength;
    protected override void Update()
    {
        base.Update();

        if (controller)
            controller.advanceDistance -= dragStrength * Time.deltaTime;
    }

    ChasedTurtleController controller;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision with " + other.name);
        ChasedTurtleController turtle = other.GetComponent<ChasedTurtleController>();
        if (turtle)
        {
            controller = turtle;
        }
        ChaseDeathWall death = other.GetComponent<ChaseDeathWall>();
        if (death)
        {
            Active = false;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (controller && controller.gameObject == other.gameObject)
            if (controller.obstaclePushMultiplier < pushMultiplier)
                controller.obstaclePushMultiplier *= pushMultiplier;
    }
    private void OnTriggerExit(Collider other)
    {
        if (controller && controller.gameObject == other.gameObject)
        {
            controller.obstaclePushMultiplier = 1;
            controller = null;
        }
    }
}
