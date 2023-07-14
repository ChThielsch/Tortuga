using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapbookManager : MonoBehaviour
{
    public PageData p1, p2;
    public PageDisplay leftPage, rightPage;

    [HideInInspector] public bool isLoading, isLoaded;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            p1.pickedMementoIndex = -1;
            p2.pickedMementoIndex = -1;

            DisplayPages(p1,p2);
        }
    }
    public void DisplayPages(PageData page1=null, PageData page2=null)
    {
        StartCoroutine(ExecuteDisplayPages(page1,page2));
    }
    IEnumerator ExecuteDisplayPages(PageData page1 = null, PageData page2 = null)
    {
        isLoaded = false;
        isLoading = true;

        leftPage.Blank();
        rightPage.Blank();

        DisplayPage(leftPage,page1);
        yield return new WaitUntil(() => leftPage.isLoaded);

        DisplayPage(rightPage,page2);
        yield return new WaitUntil(() => rightPage.isLoaded);

        isLoading = false;
        isLoaded = true;
    }
    public void DisplayPage(PageDisplay display,PageData page)
    {
        if (page) display.DisplayPage(page);
        else display.Blank();
    }

}
