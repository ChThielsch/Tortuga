using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ChaseLocus : MonoBehaviour
{
    private SplineAnimate lure;
    public bool inChase;
    public float chaseTime;

    private void Awake()
    {
        lure= GetComponent<SplineAnimate>();
    }

    public void StartChase()
    {
        lure.Pause();
        lure.Invoke("Play",3);
        chaseTime = 0;
        lure.ElapsedTime = 0;
    }
    public void StopChase()
    {
        lure.Pause();
        chaseTime=0;
        lure.ElapsedTime = 0;
    }
    private void FixedUpdate()
    {
        if (inChase)
        {
            chaseTime += Time.fixedDeltaTime;
            lure.ElapsedTime = Mathf.Max(0, chaseTime);
        }
    }

}
