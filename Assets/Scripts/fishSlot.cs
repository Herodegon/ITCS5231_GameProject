using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class fishSlot : MonoBehaviour, IPointerClickHandler
{
    //Item Data
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;

    //Item Slot
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    public GameObject selectedImage;
    public bool selected;
    private PauseMenu inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("PauseMenu").GetComponent<PauseMenu>();
    }

    public void addFish(string name, int quantity, Sprite sprite)
    {
        this.name = name;
        this.quantity = quantity;
        this.itemSprite = sprite;
        isFull = true;

        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
        itemImage.sprite = sprite;
    }

    public void removeFish()
    {
        this.itemName = null;
        this.quantity = 0;
        this.itemSprite = null;
        isFull = false;

        quantityText.enabled = false;
        itemImage.sprite = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left){
            inventoryManager.deselectFish();
            inventoryManager.deselectUpgrade();
            selectedImage.SetActive(true);
            selected = true;
        }
    }
}
