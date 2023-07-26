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
    public float pushMultiplier;
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
}
