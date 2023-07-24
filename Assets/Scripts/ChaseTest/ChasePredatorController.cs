using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePredatorController : MonoBehaviour
{

    [Divider("References")]
    public ChaseControl chase;
    public ChasedTurtleController prey;

    [Divider("Parameters")]

    [Range(0,10)] public float startBaseDistance=8;
    [Range(0, 10)] public float endBaseDistance=6;
    public float baseDistance => Mathf.Lerp(startBaseDistance, endBaseDistance, chase.lerp);

    //[Header("Rotation")]
    //[Range(0, 30)] public float maxRotationAngle = 10;
    //[Range(0, 15)] public float rotationSpeed = 5;

    [Header("Movement")]
    public float sideMoveSpeed;
    public float wiggleFrequency;
    public float catchUpFrequency;
    public float 
        minCatchUpDistance,
        maxCatchUpDistance;


    [Divider("Stats")]
    [ShowOnly] [SerializeField] private float catchUpDistance;
    [ShowOnly] [SerializeField] private float moveSideDistance;
    [ShowOnly] [SerializeField] private float rotationAngle;

    private void Start()
    {
        ResetPosition();
        chase.OnStartChase += ResetPosition;
    }

    public void ResetPosition()
    {
        catchUpDistance = 0;
        moveSideDistance = 0;
        rotationAngle = 0;
        attackCooldownTime = attackCooldown;
        prototype_AttackIndicator.SetActive(false);

        StopAllCoroutines();

        transform.localPosition = Vector3.back * baseDistance + Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (chase.inChase)
            Move();
    }

    public void Move()
    {
        if (inAttack) return;

        float turtleDistance = prey.transform.localPosition.x - transform.localPosition.x;
        float sideDistance = Mathf.Sign(turtleDistance) * sideMoveSpeed * Time.fixedDeltaTime;
        if (Mathf.Abs(sideDistance) > Mathf.Abs(turtleDistance)) sideDistance = turtleDistance;
        moveSideDistance += sideDistance;

        catchUpDistance =
            (Mathf.Sin(chase.chaseTime * catchUpFrequency) + 1) * 0.5f *
            Mathf.Lerp(minCatchUpDistance,maxCatchUpDistance,(Mathf.Sin(chase.chaseTime * catchUpFrequency*0.66f) + 1) * 0.5f );

        Vector3 sidePosition = Vector3.right * Mathf.Lerp(moveSideDistance-1f, moveSideDistance+1f,(Mathf.Sin(chase.chaseTime*wiggleFrequency)+1)*0.5f);
        Vector3 advancePosition = Vector3.forward * catchUpDistance;

        Vector3 position = sidePosition + advancePosition;
        transform.localPosition = Vector3.back * baseDistance + position;

        attackCooldownTime -= Time.fixedDeltaTime;
        if (attackCooldownTime <= 0)
        {
            //Not working
        }
    }
    [SerializeField] GameObject prototype_AttackIndicator;
    public float attackCooldown=5;
    float attackCooldownTime=5;
    public float attackDistance=4;
    public float attackSpeed = 2;
    public AnimationCurve attackCurve;
    bool inAttack;
    private IEnumerator Attack_Execute()
    {
        inAttack = true;
        prototype_AttackIndicator.SetActive(true);

        yield return new WaitForSeconds(1);

        float startDistance = catchUpDistance;
        float i = 0;
        while (i <= 1)
        {
            catchUpDistance = Mathf.Lerp(startDistance, attackDistance, attackCurve.Evaluate(i));

            transform.localPosition = Vector3.back * baseDistance
                +Vector3.right*moveSideDistance
                +Vector3.forward*catchUpDistance;

            i += attackSpeed * Time.deltaTime;

            yield return null;
        }
        prototype_AttackIndicator.SetActive(false);

        attackCooldownTime = Random.Range(attackCooldown * 0.75f, attackCooldown * 1.25f);
        inAttack = false;
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
