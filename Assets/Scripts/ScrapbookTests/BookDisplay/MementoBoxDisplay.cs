using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MementoBoxDisplay : MonoBehaviour
{
    [HideInInspector] public PageData page;
    [HideInInspector] public PageDisplay pageDisplay;

     public RectTransform rect;
    public Bounds bounds => new Bounds(transform.position,rect.sizeDelta);

    public MementoDisplay[] mementos;

    [SerializeField]
    float boxLerpDuration;
    [SerializeField]
    Vector2 boxPosLeft, boxPosRight;
    bool isPageRight => pageDisplay ? pageDisplay.isRightPage : true;

    public void Display(PageDisplay display)
    {        
        pageDisplay = display;
        page = display.page;

        for (int i = 0; i < mementos.Length; i++)
        {
            bool on = i < page.mementos.Length;
            if (on) mementos[i].Display(this, i);
            else mementos[i].Close();
        }

        Vector3
            sidePos = isPageRight ? boxPosLeft + Vector2.left * rect.sizeDelta.x: boxPosRight + Vector2.right * rect.sizeDelta.x,
            targetPos = isPageRight ? boxPosLeft : boxPosRight;
        gameObject.SetActive(true);
        StartCoroutine(ExecuteMoveBox(sidePos, targetPos, true));
    }
    public void Close()
    {
        if (!gameObject.activeSelf) return;
        Vector3 sidePos = isPageRight ? boxPosLeft + Vector2.left * rect.sizeDelta.x : boxPosRight + Vector2.right * rect.sizeDelta.x;
        StartCoroutine(ExecuteMoveBox(rect.anchoredPosition, sidePos, false));
    }

    IEnumerator ExecuteMoveBox(Vector3 initialPosition,Vector3 targetPosition, bool activate=false)
    {
        rect.anchoredPosition = initialPosition;
        if(activate) gameObject.SetActive(true);

        float lerpDuration = boxLerpDuration, lerpTimer = 0f;
        while (lerpTimer < lerpDuration)
        {
            lerpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(lerpTimer / lerpDuration);
            rect.anchoredPosition = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        if (!activate) gameObject.SetActive(false);
    }
}
