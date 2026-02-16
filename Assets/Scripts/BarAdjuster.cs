using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxBar(int value)
    {
        slider.maxValue = value;
        slider.value = value;
    }

    public void SetBar(int value)
    {
        slider.value = value;
    }
}
