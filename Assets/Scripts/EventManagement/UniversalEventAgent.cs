using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalEventAgent : MonoBehaviour
{

    public void SetMaterial(Material mat)
    {
        Renderer ren = gameObject.GetComponent<Renderer>();
        if (ren) ren.material = mat;
    }
}
