using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseObstacle : MonoBehaviour
{
    public bool Active
    {
        get=>gameObject.activeSelf;
        set
        {
            gameObject.SetActive(value);
        }
    }
    public float speed;
    public float dragStrength;
    private void Awake()
    {
        Active = false;
    }

    public void Spawn()
    {
        Active = true;

        transform.localPosition = new Vector3(Random.Range(-5, 5), 0, 15);
    }

    private void Update()
    {
        transform.localPosition += Vector3.back * speed * Time.deltaTime;

        if (controller)
            controller.advanceDistance -= dragStrength * Time.deltaTime;
    }

    ChasedTurtleController controller;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision with "+other.name);
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
    private void OnTriggerExit(Collider other)
    {
        if (controller&&controller.gameObject==other.gameObject)
        {
            controller = null;
        }
    }
}
