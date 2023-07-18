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
        Vector3 vec= Quaternion.FromToRotation(Vector3.right,target.currentDirection!=Vector3.zero? target.currentDirection : Vector3.forward).eulerAngles;
        vec.x = 90;
        transform.rotation = Quaternion.Euler(vec);
    }
}
