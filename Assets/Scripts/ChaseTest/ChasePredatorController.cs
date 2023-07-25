using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePredatorController : MonoBehaviour
{

    [Divider("References")]
    public ChaseControl chase;
    public ChasedTurtleController prey;


    private void Start()
    {
        ResetPosition();
        chase.OnStartChase += ResetPosition;
    }

    public void ResetPosition()
    {

        StopAllCoroutines();
    }

    private void Update()
    {
               if (chase.inChase)
            Move();
    }

    public float backDistance;
    public float minDistance, maxDistance;
    public float safeDistance;

    public Vector2 speedAdjust;
    public float turtlePositionUpdateCooldown;
    float tpupdateTime;

    public float advanceFrequencyMajor;
    public float advanceFrequencyMinor;
    public float safetyLerpFrequency;
    [ShowOnly] public float advanceLerpMinor;
    [ShowOnly] public float advanceLerpMajor;
    [ShowOnly] public float safetyLerp;

    float
    minorSine,
    majorSine,
    safetySine,
    frequency;

    Vector3
        lastTurtlePos;


    public void Move()
    {

        //Pos right behind Turtle.
        //Lerp between local base distance and turtle- security distance.
        //Security distance varies dynamically and gets less until end.

        //Special Moves
        //Bite: Happens if turtle is coming closer than security distance
        //Stay there for a sec and play bite anim. Dash a little bit before falling back.

        //Dash: Cooldown goes faster the more advance turtle has.
        //Stay constant for 0.5sec and save current securitydistance+Position. Then Dash there.

        tpupdateTime -= Time.deltaTime;
        if (tpupdateTime < 0)
        {
            tpupdateTime = turtlePositionUpdateCooldown;
            lastTurtlePos = prey.transform.localPosition;
        }

        advanceLerpMinor += Time.deltaTime * advanceFrequencyMinor;
        advanceLerpMajor += Time.deltaTime * advanceFrequencyMajor;
        safetyLerp += Time.deltaTime * safetyLerpFrequency;

        minorSine = ((Mathf.Sin(advanceLerpMinor) + 1) / 2);
        majorSine = ((Mathf.Sin(advanceLerpMajor) + 1) / 2);
        safetySine = 0.25f + ((Mathf.Sin(safetyLerp) + 1) / 4);
        frequency = majorSine * (0.75f + 0.25f * minorSine);

        Vector3 backVec =
            lastTurtlePos.x * Vector3.right +
            Vector3.back * backDistance;
        Vector3 frontVec =
            lastTurtlePos+ 
            Vector3.back * safeDistance*safetySine;

        Vector3 pos = Vector3.Lerp(backVec, frontVec, frequency);
        pos.z= Mathf.Max(minDistance, Mathf.Min(pos.z, maxDistance));

        Vector3 dir= pos - transform.localPosition;
        dir.x = Mathf.Min(dir.x, speedAdjust.x);
        dir.z = Mathf.Min(dir.z, speedAdjust.y);

        Debug.DrawRay(transform.position, dir, Color.green);
        Debug.DrawRay(transform.parent.position, pos, Color.blue);

        transform.localPosition += dir*Time.deltaTime;
    }
   
    private IEnumerator Attack_Execute()
    {
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject==prey.gameObject)
        {
            chase.StopChase();
            chase.Invoke("StartChase", 1);
        }
    }
}
