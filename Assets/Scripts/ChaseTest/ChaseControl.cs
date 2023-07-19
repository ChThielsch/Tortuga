using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ChaseControl : MonoBehaviour
{
    [HideInInspector]public SplineAnimate rail;
    public bool inChase;
    public float chaseTime;

    public Vector3 forward => transform.forward;
    public Vector3 right => transform.right;
    public Vector3 up => transform.up;

    private void Awake()
    {
        rail= GetComponent<SplineAnimate>();

        StartChase();
    }

    public void StartChase()
    {
        rail.Pause();
        rail.Invoke("Play",2);
        chaseTime = 0;
        rail.ElapsedTime = 0;
    }
    public void StopChase()
    {
        rail.Pause();
        chaseTime=0;
        rail.ElapsedTime = 0;
    }
    private void FixedUpdate()
    {
        if (inChase)
        {
            chaseTime += Time.fixedDeltaTime;
        }
    }

}
