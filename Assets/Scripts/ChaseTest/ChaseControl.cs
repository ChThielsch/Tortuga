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

    [SerializeField]private float rotationUpdateInterval = 1f; // Time interval to update the rotation
    private float timeSinceLastUpdate = 0f;
    public float previousRotationY;

    private void Awake()
    {
        rail= GetComponent<SplineAnimate>();
        previousRotationY = transform.rotation.eulerAngles.y;
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
            Debug.Log(lerp);
            if (lerp == 1)
            {
                StopChase();
                StartChase();
            }

            timeSinceLastUpdate += Time.deltaTime;
            if (timeSinceLastUpdate >= rotationUpdateInterval)
            {
                float 
                    currentRotationY = transform.rotation.eulerAngles.y,
                    rotationDeltaY = Mathf.DeltaAngle(previousRotationY, currentRotationY);

                timeSinceLastUpdate = 0f;
                previousRotationY = currentRotationY;
            }
        }
    }

}
