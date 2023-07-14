using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Scrapbook/Page")]
public class PageData : ScriptableObject
{
    [TextArea(3, 20)] public string story;

    public MementoOption[] mementos;

    public int pickedMementoIndex = -1;
    public MementoOption PickedMemento 
        => pickedMementoIndex < 0 || pickedMementoIndex >= mementos.Length ? null : mementos[pickedMementoIndex];
    public bool isResolved => pickedMementoIndex >= 0;
    public Vector2Int mementoPos;
}

[System.Serializable] public class MementoOption
{
    public Sprite picture;
    public Vector2Int dimensions;
    [TextArea(3,20)]public string story;
}