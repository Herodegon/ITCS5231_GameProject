using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool paused;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unpause();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (paused)
            {
                unpause();
            }
            else
            {
                pause();
            }
        }
    }

    void pause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        paused = true;
    }

    void unpause()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        paused = false;
    }
}
