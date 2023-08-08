using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UI;
using Unity.VisualScripting;

public class StationComputerLogin : MonoBehaviour
{

    public List<int> enteredPassword;
    public int[] correctPassword;
    public ElementTransition[] inputFieldDigits;
    private bool enteredCorrectPassword;

    public ElementTransition wrongPasswordNotification;
    public ElementTransition logoAnimation;

    [Header("Interactables")]
    public Button[] interactables;

    public GameObject numPadPanel;
    public GameObject passwordPanel;




    void Start()
    {
        StartCoroutine(BootSequence());
    }

    public IEnumerator BootSequence()
    {
        passwordPanel.SetActive(false);
        numPadPanel.SetActive(false);
        logoAnimation.JumpToPosition(2);
        logoAnimation.PlayAnimation(0);
        yield return new WaitForSeconds(2f);
        logoAnimation.PlayAnimation(1);
        yield return new WaitForSeconds(1f);
        passwordPanel.SetActive(true);
        numPadPanel.SetActive(true);
    }
    public void AddDigit(int digit)
    {
        if (enteredPassword.Count < correctPassword.Length)
        {
            enteredPassword.Add(digit);
            inputFieldDigits[enteredPassword.Count - 1].PlayAnimation(1);
        }
    }

    public void RemoveDigit()
    {
        if (enteredPassword.Count > 0)
        {
            enteredPassword.RemoveAt(enteredPassword.Count - 1);
            inputFieldDigits[enteredPassword.Count].PlayAnimation(0);
        }
    }

    public void CheckPassword()
    {
        foreach (Button item in interactables)
        {
            item.interactable = false;
        }

        enteredCorrectPassword = true;

        for (int i = 0; i < enteredPassword.Count; i++)
        {
            if (enteredPassword[i] != correctPassword[i])
            {
                enteredCorrectPassword = false;
            }
        }

        if (enteredCorrectPassword)
        {
            print("Password Correct");

        }
        else
        {
            print("Password Wrong");
            StartCoroutine(EnteredWrongPassword());
        }
    }



    IEnumerator EnteredWrongPassword()
    {
        numPadPanel.SetActive(false);
        wrongPasswordNotification.PlayAnimation(1);
        yield return new WaitForSeconds(1);
        wrongPasswordNotification.PlayAnimation(0);
        yield return new WaitForSeconds(.3f);

        foreach(ElementTransition item in inputFieldDigits)
        {
            item.PlayAnimation(0);
        }

        numPadPanel.SetActive(true);

        foreach (Button item in interactables)
        {
            item.interactable = true;
        }

        enteredPassword.Clear();
    }
}



