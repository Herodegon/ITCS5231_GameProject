using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BarState {Green, Red, Maroon, Off};

public class fishBarCreator : MonoBehaviour
{
    public bool isBarOn;
    public GameObject fishBar;
    [SerializeField] private Image redSect;
    [SerializeField] private Image greenSect;
    [SerializeField] private Image playerIndicator;
    public BarState State;

    // Start is called before the first frame update
    void Start()
    {
        isBarOn = false;
        fishBar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Create the fishing bar based on the stats passed in
    void createBar(int durability, int catchRate)
    {
        isBarOn = true;
        fishBar.SetActive(true);

        redSect.rectTransform.sizeDelta = new Vector2(durability, 90);
        greenSect.rectTransform.sizeDelta = new Vector2(catchRate, 100);
    }

    //update red section and player indicator in real time based on locationa and durability
    void updateBar(int durability, float direction)
    {
        redSect.rectTransform.sizeDelta = new Vector2(durability, 90);
        float pos = direction * 1125;
        playerIndicator.rectTransform.anchoredPosition = new Vector2(pos, 0.0f);
    }

    void destroyBar()
    {
        isBarOn = false;
        fishBar.SetActive(false);
        State = BarState.Off;
    }

    //check if which section the player is in and return 1 for green, 2 for red, and 3 for maroon
    void checkBar(int durability, int catchRate, float direction)
    {
        float pos = direction * 1125;
        if(pos >= ((1125/2) - (catchRate / 2)) && pos <= ((1125/2) + (catchRate / 2)))
        {
            State = BarState.Green;
        }
        if(pos >= ((1125/2) - (durability / 2)) && pos <= ((1125/2) + (durability / 2)))
        {
            State = BarState.Red;
        }
        else
        {
            State = BarState.Maroon;
        }
    }
}