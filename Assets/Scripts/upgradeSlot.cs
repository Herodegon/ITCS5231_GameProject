using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class upgradeSlot : MonoBehaviour, IPointerClickHandler
{
    //Item Data
    public string itemName;
    public Sprite itemSprite;
    public bool isFull;

    //Item Slot
    [SerializeField] private Image itemImage;
    public GameObject selectedImage;
    public bool selected;
    private PauseMenu inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("CanvasUI").GetComponent<PauseMenu>();
    }

    public void addUpgrade(string name, Sprite sprite)
    {
        this.name = name;
        this.itemSprite = sprite;
        isFull = true;

        itemImage.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left){
            inventoryManager.deselectUpgrade();
            inventoryManager.deselectFish();
            selectedImage.SetActive(true);
            selected = true;
        }
    }
}
