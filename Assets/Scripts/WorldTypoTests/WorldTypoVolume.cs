using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;

public class WorldTypoVolume : MonoBehaviour
{
    public TypewriterByCharacter typewriter;

    public bool repeat, shown, visible;

    private void Awake()
    {
        typewriter.onTextShowed.AddListener(OnShow);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((repeat||!shown)&&other.tag=="Player") //Check if its the player here
        {
            Show();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ( other.tag == "Player") //Check if its the player here
        {
            Hide();
        }
    }

    public void Show()
    {
        visible = true;
        typewriter.StopAllCoroutines();
        typewriter.StartShowingText();
    }
    public void Hide()
    {
        visible = false;
        typewriter.StopAllCoroutines();
        typewriter.StartDisappearingText();
    }

    public void OnShow() => shown = true;
}
