using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCamera : MonoBehaviour
{
    public TurtleController target;
    float addition;
    private void Awake()
    {
        addition= transform.rotation.eulerAngles.y;
    }
    private void FixedUpdate()
    {
        Vector3
            dir = target.chaseLocus.transform.forward,
            vec = Quaternion.FromToRotation(Vector3.right, dir != Vector3.zero ? dir : Vector3.forward).eulerAngles;

        vec.x = 90;
        transform.rotation = Quaternion.Euler(vec);
    }
}
