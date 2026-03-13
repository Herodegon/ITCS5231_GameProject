using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public Slider healthBar;
    public Slider fuelBar;

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
