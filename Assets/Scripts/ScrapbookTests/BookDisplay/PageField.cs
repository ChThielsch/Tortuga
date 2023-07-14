using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class PageField : MonoBehaviour/*, IDropHandler*/
{
    [HideInInspector]public RectTransform rectTrans;
    [HideInInspector]public PageDisplay page;
    public bool isBlocked;
    public Vector2Int pos;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
    }
    //public void OnDrop(PointerEventData eventData)
    //{
    //    Debug.Log("Dropped on "+pos);
    //    MementoDisplay display = eventData.pointerDrag.GetComponent<MementoDisplay>();
    //    if (display)
    //        display.OnDropPageField(this);
    //}
}
