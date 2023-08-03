using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipInteractable : MonoBehaviour, IInteractable
{
    public string tooltip;
    public string Tooltip=> tooltip;

    public bool blocked;

    public bool isBlocked()
    {
        return blocked;
    }

    public void OnInteract()
    {
    }
}
