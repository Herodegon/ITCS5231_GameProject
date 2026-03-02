using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool paused;
    public fishSlot[] fishSlot;
    public upgradeSlot[] upgradeSlot;

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

    public void addFish(string name, int quantity, Sprite sprite)
    {
        for (int i = 0; i < fishSlot.Length; i++)
        {
            if (fishSlot[i].isFull == false)
            {
                fishSlot[i].addFish(name, quantity, sprite);
                return;
            }
        }
    }

    public void addUpgrade(string name, int quantity, Sprite sprite)
    {
        for (int i = 0; i < upgradeSlot.Length; i++)
        {
            if (upgradeSlot[i].isFull == false)
            {
                upgradeSlot[i].addUpgrade(name, quantity, sprite);
                return;
            }
        }
    }
}
