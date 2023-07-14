using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;

public class WorldTypoBatchTester : MonoBehaviour
{
    public List<TypewriterByCharacter> typewriters = new List<TypewriterByCharacter>();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Show();
        if (Input.GetKeyDown(KeyCode.E))
            Hide();

    }

    public void Show()
    {
        foreach (TypewriterByCharacter t in typewriters)
        {
            if (t.isShowingText) return;
            if (t.isHidingText) t.SkipTypewriter();
            t.StartShowingText();
        }
    }
    public void Hide()
    {
        foreach (TypewriterByCharacter t in typewriters)
        {
            if (t.isHidingText) return;
            if (t.isShowingText) t.SkipTypewriter();
            t.StartDisappearingText();
        }
    }
}
