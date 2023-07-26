using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseObstacle : MonoBehaviour
{
    public bool Active
    {
        get => gameObject.activeSelf;
        set
        {
            gameObject.SetActive(value);
            if (!value && controller) controller.obstacles.Remove(this);
        }
    }
    public float speed;
    public float pushForceMultiplier, pushDurationMultiplier;
    public bool cancelCoroutines;

    protected ChasedTurtleController controller;


    private void Awake()
    {
        Active = false;
    }

    public void Spawn()
    {
        Active = true;

        transform.localPosition = new Vector3(Random.Range(-5, 5), 0, 15);
    }

    protected virtual void Update()
    {
        transform.localPosition += Vector3.back * speed * Time.deltaTime;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision with " + other.name);
        ChasedTurtleController turtle = other.GetComponent<ChasedTurtleController>();
        if (turtle)
        {
            controller = turtle;
            if (cancelCoroutines)
                controller.StopAllCoroutines();
            controller.obstacles.Add(this);
        }
        ChaseDeathWall death = other.GetComponent<ChaseDeathWall>();
        if (death)
        {
            Active = false;
        }
    }
    protected virtual void OnTriggerStay(Collider other)
    {

    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (controller && controller.gameObject == other.gameObject)
        {
            controller.obstacles.Remove(this);

            controller = null;
        }
    }
}