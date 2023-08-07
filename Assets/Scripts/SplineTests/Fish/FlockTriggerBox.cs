using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockTriggerBox : MonoBehaviour
{
    private void Awake()
    {
        Toggle(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            Toggle(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            Toggle(false);
    }
    public void Toggle(bool on)
    {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(on);
    }
}
