using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class MementoDisplay : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public static MementoDisplay Dragged;
    public bool isDragged
    {
        get => Dragged == this;
        set
        {
            if (value && !isDragged)
            {
                if (Dragged != null) Dragged.isDragged = false;
                Dragged = this;
            }
            else if (!value && isDragged)
            {
                Dragged = null;
            }
        }
    }

    public RectTransform rect;
    public Vector2 restPos;
    public MementoBoxDisplay box;
    public int index;
    public MementoOption option;

    private bool IsAboveBox => box.bounds.Contains(transform.position);


    private void OnEnable()
    {
        rect.anchoredPosition = restPos;
    }
    public void Display(MementoBoxDisplay box,int index)
    {
        this.box = box;
        this.index = index;
        option= box.page.mementos[index];
        GetComponent<RectTransform>().sizeDelta = (Vector2.one * 225)*option.dimensions;
        gameObject.SetActive(true);
    }
    public void Close()
    {
        rect.anchoredPosition = restPos;
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (Dragged)
        {
            if(!IsAboveBox) box.pageDisplay.HintMemento(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragged = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragged = false;

        if (!IsAboveBox)
        {
            PageField closest = box.pageDisplay.GetClosestPageField(this);
            if (closest.page.PickMemento(closest, this))
                return;
        }
        rect.anchoredPosition = restPos;
        box.pageDisplay.memento.gameObject.SetActive(false);
    }

}
