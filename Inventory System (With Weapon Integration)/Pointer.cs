using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Pointer : MonoBehaviour, IPointerClickHandler
{
    // POINTER DATA
    public InventorySystem attachedInventory;
    public int attachedSlot;
    public InventoryItemObject attachedItem;
    public int attachedAmount;
    // CLICKED SLOT DATA
    private InventorySystem clickedInventory;
    private int clickedSlot;
    private InventoryItemObject clickedItem;
    private int clickedAmount;
    // INTERMEDIARY DATA
    private InventorySystem intermediaryInventory;
    private int intermediarySlot;
    private InventoryItemObject intermediaryItem;
    private int intermediaryAmount;
    // UI DATA
    private Image mouseImage;
    private TextMeshProUGUI mouseCount;
    // FLOATER
    public GameObject itemFloater;

    private void Start()
    {
        mouseImage = gameObject.GetComponentInChildren<Image>();
        mouseCount = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        mouseImage.color = new Color(mouseImage.color.r, mouseImage.color.g, mouseImage.color.b, 0);
        mouseCount.enabled = false;
        Debug.Log("Pointer Activated.");
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
        if (!GameObject.Find("InventoryPanel") && attachedItem != null)
        {
            ReturnPointerDataToAttachedInventory();
            UpdatePointerUI();
        }
        if (GameObject.Find("InventoryPanel"))
        {
            UpdatePointerUI();
        }
    }
    //----------------------------
    //-----POINTER UI HANDLER-----
    //----------------------------

    public void UpdatePointerUI()
    {
        if (attachedItem == null)
        {
            // EMPTY ITEM UI
            mouseImage.color = new Color(mouseImage.color.r, mouseImage.color.g, mouseImage.color.b, 0);
            mouseCount.enabled = false;
        }
        else
        {
            // SOME ITEM UI
            mouseImage.enabled = true;
            mouseImage.color = new Color(mouseImage.color.r, mouseImage.color.g, mouseImage.color.b, 255);
            mouseImage.overrideSprite = attachedItem.icon;
            if (attachedItem.weapon != null)
            {
                mouseCount.enabled = false;
            }
            else
            {
                mouseCount.enabled = true;
            }
            mouseCount.text = attachedAmount.ToString();
        }
    }

    //-------------------------------
    //-----POINTER LOGIC HANDLER-----
    //-------------------------------
    public void OnPointerClick(PointerEventData eventData)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        // IF A UI SLOT WAS CLICKED
        PointerDataVerifier();
        if (results.Count > 3) 
        {
            clickedInventory = results[1].gameObject.GetComponentInParent<InventoryUI>().inventorySystem;
            clickedSlot = results[1].gameObject.GetComponentInParent<InventorySlotUI>().slot;
            clickedItem = clickedInventory.inventorySlots[clickedSlot];
            clickedAmount = clickedInventory.amountsPerSlot[clickedSlot];

            if (eventData.button.ToString() == "Left")
            {
                SlotButtonLeftClick();
            }
            else if (eventData.button.ToString() == "Right")
            {
                SlotButtonRightClick();
            }
            EmptyClickedData();
        }
        // IF YOU RIGHT CLICK OUTSIDE UI
        else if (eventData.button.ToString() == "Right" && attachedItem != null && !IsPointerOverUIObject())
        {
            GameObject item = Instantiate(itemFloater, GameObject.Find("Player").transform.position, Quaternion.identity);
            item.GetComponent<ItemFloater>().amount = 1;
            item.GetComponent<ItemFloater>().item = attachedItem;
            SubtractOneFromPointerData();
        }
        // IF YOU LEFT CLICK OUTSIDE UI
        else if (eventData.button.ToString() == "Left" && attachedItem != null && !IsPointerOverUIObject())
        {
            GameObject item = Instantiate(itemFloater, GameObject.Find("Player").transform.position, Quaternion.identity);
            item.GetComponent<ItemFloater>().amount = attachedAmount;
            item.GetComponent<ItemFloater>().item = attachedItem;
            EmptyPointerData();
        }
        PointerDataVerifier();
    }

    public void SlotButtonLeftClick()
    {
        // IF MOUSE IS EMPTY
        if (attachedItem == null)
        {
            // AND THE SLOT HAS AN ITEM
            if (clickedItem != null)
            {
                SaveDataToPointer(clickedInventory, clickedSlot, clickedItem, clickedAmount);
                clickedInventory.ClearSlot(attachedSlot);
            }
        }
        // IF MOUSE HAS AN ITEM
        else
        {
            // AND THE SLOT IS EMPTY
            if (clickedItem == null)
            {
                clickedInventory.AddToInventory(attachedItem, attachedAmount, clickedSlot);
                EmptyPointerData();
            }
            // AND THE SLOT HAS THE SAME ITEM AS THE MOUSE
            else if (attachedItem == clickedItem)
            {
                if (clickedItem.maxStackAmount - clickedAmount >= attachedAmount)
                {
                    clickedInventory.AddToInventory(attachedItem, attachedAmount, clickedSlot);
                    EmptyPointerData();
                }
                else
                {
                    clickedInventory.AddToInventory(attachedItem, attachedAmount, clickedSlot);
                    attachedAmount -= clickedItem.maxStackAmount - clickedAmount;
                }
            }
            // AND THE SLOT ITEM IS DIFFERENT FROM THE MOUSE ITEM
            else
            {
                SaveIntermediaryData(clickedInventory, clickedSlot, clickedItem, clickedAmount);
                clickedInventory.ClearSlot(clickedSlot);
                clickedInventory.AddToInventory(attachedItem, attachedAmount, clickedSlot);
                SaveDataToPointer(intermediaryInventory, intermediarySlot, intermediaryItem, intermediaryAmount);
                EmptyIntermediaryData();
            }
        }
    }

    public void SlotButtonRightClick()
    {
        // IF MOUSE IS EMPTY
        if (attachedItem == null)
        {
            // AND THE SLOT HAS AN ITEM
            if (clickedItem != null)
            {
                if (clickedAmount == 1)
                {
                    SaveDataToPointer(clickedInventory, clickedSlot, clickedItem, clickedAmount);
                    clickedInventory.RemoveFromInventory(clickedItem, clickedAmount, clickedSlot);
                }
                else
                {
                    SaveDataToPointer(clickedInventory, clickedSlot, clickedItem, clickedAmount / 2);
                    clickedInventory.RemoveFromInventory(clickedItem, clickedAmount / 2, clickedSlot);
                }
            }
        }
        // IF MOUSE HAS AN ITEM
        else
        {
            // AND THE SLOT IS EMPTY
            if (clickedItem == null)
            {
                clickedInventory.AddToInventory(attachedItem, 1, clickedSlot);
                SubtractOneFromPointerData();
            }
            // AND THE SLOT HAS THE SAME ITEM AS THE MOUSE
            else if (attachedItem == clickedItem && clickedItem.maxStackAmount != clickedAmount)
            {
                clickedInventory.AddToInventory(attachedItem, 1, clickedSlot);
                SubtractOneFromPointerData();
            }
        }
    }

    //--------------------------
    //-----HELPER FUNCTIONS-----
    //--------------------------

    public void EmptyPointerData()
    {
        attachedInventory = null;
        attachedSlot = -1;
        attachedItem = null;
        attachedAmount = 0;
    }

    public void PointerDataVerifier()
    {
        if (attachedAmount <= 0 || attachedItem == null)
        {
            EmptyPointerData();
        }
    }

    public void SubtractOneFromPointerData()
    {
        if (attachedItem != null)
        {
            attachedAmount--;
            if (attachedAmount <= 0)
            {
                EmptyPointerData();
            }
        }
    }

    public void EmptyIntermediaryData()
    {
        intermediaryInventory = null;
        intermediarySlot = -1;
        intermediaryItem = null;
        intermediaryAmount = 0;
    }

    public void EmptyClickedData()
    {
        clickedInventory = null;
        clickedSlot = -1;
        clickedItem = null;
        clickedAmount = 0;
    }

    public void SaveDataToPointer(InventorySystem inventory, int slot, InventoryItemObject item, int amount)
    {
        attachedInventory = inventory;
        attachedSlot = slot;
        attachedItem = item;
        attachedAmount = amount;
    }

    public void SaveIntermediaryData(InventorySystem system, int slot, InventoryItemObject item, int amount)
    {
        intermediaryInventory = system;
        intermediarySlot = slot;
        intermediaryItem = item;
        intermediaryAmount = amount;
    }

    public void ReturnPointerDataToAttachedInventory()
    {
        if (attachedInventory.DoesInventoryHaveEnoughSpaceToAddItem(attachedItem, attachedAmount))
        {
            attachedInventory.AddToInventory(attachedItem, attachedAmount);
        }
        else
        {
            GameObject item = Instantiate(itemFloater, GameObject.Find("Player").transform.position, Quaternion.identity);
            item.GetComponent<ItemFloater>().amount = attachedAmount;
            item.GetComponent<ItemFloater>().item = attachedItem;
        }
        EmptyPointerData();
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 1;
    }
}
