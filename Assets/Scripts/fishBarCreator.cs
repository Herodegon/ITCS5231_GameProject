using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fishBarCreator : MonoBehaviour
{
    public bool isBarOn;
    public GameObject fishBar;
    [SerializeField] private Image redSect;
    [SerializeField] private Image greenSect;
    [SerializeField] private Image playerIndicator;

    // Start is called before the first frame update
    void Start()
    {
        isBarOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBarOn == false)
        {
            fishBar.SetActive(false);
        }
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
}
