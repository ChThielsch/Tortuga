using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Febucci.UI;

public class PageDisplay : MonoBehaviour
{
    public bool isRightPage;

    public PageData page;
    public MementoBoxDisplay mementoBox;
    public Image memento;

    //Disclamer: Mind that order of texts and typewriter list goes against reading order to mimic a standart coordinate system and make assignment of mementos dimensions more intuitive.
    public TMP_Text[] texts;
    public TypewriterByCharacter[] typewriters;
    public PageField[] fields;
    public Dictionary<Vector2Int, PageField> grid;

    [HideInInspector] public bool isLoaded, isLoading, hasMergedTextBox;

    [SerializeField] float mementoLerpDuration;

    private void Awake()
    {
        memento.gameObject.SetActive(false);
        grid = new Dictionary<Vector2Int, PageField>();
        int i = 0;
        for (int y = 2; y >=0; y--)
            for (int x = 0; x < 3; x++)
            {
                PageField f = fields[i];
                f.pos = new Vector2Int(x, y);
                f.page = this;
                grid.Add(f.pos, f);
                i++;
            }

        foreach (TypewriterByCharacter typewriter in typewriters)
            typewriter.onTextShowed.AddListener(OnEndWaitTypewriter);
        WrapText();
    }
    public void DisplayPage(PageData page)
    {
        this.page = page;

        StartCoroutine(ExecuteDisplayPage());
    }
    public IEnumerator ExecuteDisplayPage()
    {
        isLoaded = false;
        isLoading = true;
        bool isResolved = page.isResolved;

        mementoBox.Close();
        for (int y = 0; y < 2; y++)
            for (int x = 0; x < 3; x++)
            grid[new Vector2Int(x,y)].isBlocked = false;
        memento.gameObject.SetActive(false);

        if (isResolved) // if resolved: Show mementos
            DisplayMemento(page.PickedMemento, grid[page.mementoPos]);
        WrapText();

        //Show Start Story
        if (isResolved)
        {
            texts[2].text = page.story;
            typewriters[2].SkipTypewriter();
        }
        else
        {
            typewriters[2].ShowText(page.story);
            typewriters[2].StartShowingText();
            waitTypewriter = true;
            yield return new WaitUntil(() => !waitTypewriter);
        }

        //If unresolved: pick memento
        if (!isResolved)
        {
            mementoBox.Display(this);
            yield return new WaitUntil(() => page.PickedMemento != null);
            yield return new WaitForSeconds(mementoLerpDuration);
            mementoBox.Close();
            WrapText();
        }

        //Show Memento Story
        if (isResolved)
        {
            texts[1].text = page.PickedMemento.story;
            typewriters[1].SkipTypewriter();
            typewriters[0].SkipTypewriter();
        }
        else
        {
            typewriters[1].ShowText(page.PickedMemento.story);
            typewriters[1].StartShowingText();
            waitTypewriter = true;
            yield return new WaitUntil(() => !waitTypewriter);

            if (!hasMergedTextBox)
            {
                string overflow = texts[1].text;
                overflow = overflow.Substring(texts[1].firstOverflowCharacterIndex);
                typewriters[0].ShowText(overflow);
                typewriters[0].StartShowingText();
                waitTypewriter = true;
                yield return new WaitUntil(() => !waitTypewriter);
            }           
        }

        yield return new WaitForSeconds(0.25f);

        isLoading = false;
        isLoaded = true;
    }
    public void Blank()
    {
        StopAllCoroutines();
        foreach (TMP_Text t in texts)
            t.text = "";
        memento.gameObject.SetActive(false);
        isLoaded = true;
    }

    public void WrapText()
    {
        List<RectTransform> textRects= new List<RectTransform>();
        for (int i = 0; i < 3; i++)
            textRects.Add(texts[i].GetComponent<RectTransform>());

        for (int y = 0; y < 3; y++)
        {
            TMP_Text text = texts[y];
            List<RectTransform> rects = new List<RectTransform>();
            Vector2
                middlePos = Vector2.up * grid[new Vector2Int(0, y)].rectTrans.position.y,
                combinedSize = Vector2.up * 250;
            for (int x = 0; x < 3; x++)
            {
                PageField f = grid[new Vector2Int(x, y)];
                if (!f.isBlocked || y == 2)
                {
                    rects.Add(f.rectTrans);
                    middlePos += Vector2.right * f.rectTrans.position.x;
                    combinedSize += Vector2.right * (f.rectTrans.sizeDelta.x);
                }
                else if (rects.Count != 0) break;
            }
            if (rects.Count != 0)
            {
                middlePos = new Vector2(middlePos.x / rects.Count, rects[0].position.y);
            }

            RectTransform txtRect = textRects[y];
            txtRect.position = middlePos;
            txtRect.sizeDelta = combinedSize;
        }

        if (textRects[0].position.x == textRects[1].position.x) //Texts are the same and can be merged
        {
            Debug.Log("Merge");
            textRects[1].sizeDelta += Vector2.up * textRects[0].sizeDelta;
            textRects[1].position = (textRects[0].position + textRects[1].position) * 0.5f;
            textRects[0].position = Vector2.one * -textRects[0].sizeDelta;
            hasMergedTextBox = true;
        }
        else hasMergedTextBox = false;
    }

    public void HintMemento(MementoDisplay display)
    {
        PageField root;
        List<PageField> coveredFields;
        if (CheckFitAllRotations(display.option.dimensions, GetClosestPageField(display), out root, out coveredFields))
            DisplayMemento(display.option,root,false);
        else memento.gameObject.SetActive(false);
    }
    public bool PickMemento(PageField field, MementoDisplay display)
    {
        int index= display.index;
        if (field.isBlocked) return false;
        MementoOption option = page.mementos[index];

        List<PageField> coveredFields;
        CheckFitAllRotations(option.dimensions, field, out field, out coveredFields);

        foreach (PageField f in coveredFields)
            f.isBlocked = true;
        page.pickedMementoIndex = index;
        page.mementoPos = field.pos;

        StartCoroutine(ExecuteDisplayMemento(display, field));

        return true;
    }

    IEnumerator ExecuteDisplayMemento(MementoDisplay display, PageField field)
    {
        Vector3
            initialPosition = display.transform.position,
            targetPosition = ((Vector2)field.transform.position + (Vector2)(grid[field.pos + (display.option.dimensions - Vector2Int.one)].transform.position)) * 0.5f;

        float lerpDuration = mementoLerpDuration, lerpTimer = 0f;
        while (lerpTimer < lerpDuration)
        {
            lerpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(lerpTimer / lerpDuration);
            display.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        display.Close();
        DisplayMemento(display.option, field, true);
    }
    public void DisplayMemento(MementoOption option, PageField field, bool real=false)
    {
        memento.transform.position = ((Vector2)field.transform.position + (Vector2)(grid[field.pos + (option.dimensions - Vector2Int.one)].transform.position)) * 0.5f;
        memento.sprite = option.picture;
        memento.GetComponent<RectTransform>().sizeDelta = (Vector2.one * 225) * option.dimensions;
        memento.gameObject.SetActive(true);

        Color col = memento.color;
        col.a = real ? 1f : 0.25f;
        memento.color = col;
    }

    public PageField GetClosestPageField(MementoDisplay display)
    {
        Vector2 mementosPosition = display.rect.position;
        PageField closest = fields[3];
        float closestDistance = Vector2.Distance(mementosPosition, fields[0].rectTrans.position);

        for (int i = 4; i < fields.Length; i++)
        {
            if (fields[i].pos.x == 1 && display.option.dimensions.x < 2) continue;
            float distance = Vector2.Distance(mementosPosition, fields[i].rectTrans.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = fields[i];
            }
        }

        return closest;
    }
    public bool CheckFitAllRotations(Vector2Int dimensions, PageField field, out PageField root, out List<PageField> fields)
    {
        Vector2Int position = field.pos;
        fields = new List<PageField>();

        Debug.Log("1");
        if (CheckFit(dimensions, position, out root, out fields))
            return true;

        Vector2Int
            cutDimensions = dimensions - Vector2Int.one,
            flippedCutDimensions;

        Debug.Log("2");
        flippedCutDimensions = -cutDimensions;
        if (CheckFit(dimensions, position + flippedCutDimensions, out root, out fields))
            return true;

        Debug.Log("3");
        flippedCutDimensions = cutDimensions * Vector2Int.left;
        if (CheckFit(dimensions, position + flippedCutDimensions, out root, out fields))
            return true;

        Debug.Log("4");
        flippedCutDimensions = cutDimensions * Vector2Int.down;
        if (CheckFit(dimensions, position + flippedCutDimensions, out root, out fields))
            return true;

        return false;
    }
    public bool CheckFit(Vector2Int dimensions, Vector2Int position, out PageField root, out List<PageField> fields)
    {
        fields = new List<PageField>();
        root = null;

        for (int x = 0; x < dimensions.x; x++)
            for (int y = 0; y < dimensions.y; y++)
            {
                Vector2Int p = position + new Vector2Int(x, y);
                if (!grid.ContainsKey(p) || grid[p].isBlocked) return false;
                PageField f = grid[p];
                if (x + y == 0) root = f;
                fields.Add(f);
            }

        return true;
    }

    bool waitTypewriter;
    public void OnEndWaitTypewriter()
    {
        waitTypewriter = false;
    }
}
