using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable 
{
    public string name { get; }
    public string Tooltip { get; }
    public bool isBlocked();
    public void OnInteract();
}
