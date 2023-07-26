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

}
