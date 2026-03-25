using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Bar : MonoBehaviour
{
    public Slider healthBar;
    public Slider fuelBar;
    public GameObject healthUI;
    public GameObject fuelUI;

    void Start()
    {
        healthUI.transform.Translate(215.0f, 75.0f, 0.0f);
        fuelUI.transform.Translate(215.0f, 130.0f, 0.0f);
    }

    public void setMaxBar(int value, bool health)
    {
        if(health)
        {
            healthBar.maxValue = value;
            healthBar.value = value;
        }
        else
        {
            fuelBar.maxValue = value;
            fuelBar.value = value;
        }
    }

    public void setBar(int value, bool health)
    {
        if (health)
        {
            healthBar.value = value;
        }
        else
        {
            fuelBar.value = value;
        }
    }
}
