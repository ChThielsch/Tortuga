using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public InputActionReference pauseReference;
    public GameObject Panel;
    public Selectable firstSelected;

    private bool m_paused;

    public void Start()
    {
        pauseReference.action.performed += ctx => { TogglePause(); };
    }

    private void OnDestroy()
    {
        pauseReference.action.performed -= ctx => { TogglePause(); };
    }

    public void TogglePause()
    {
        m_paused = !m_paused;

        if (m_paused)
        {
            Time.timeScale = 0f;
            Panel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        }
        else
        {
            Time.timeScale = 1f;
            Panel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
