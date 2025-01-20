using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using JetBrains.Annotations;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    public InventorySystem inventorySystem;
    // UI ONLY VARIABLES
    public GameObject slotPrefab;
    [HideInInspector] public GameObject[] slots;
    public GameObject inventoryPanel;
    public bool isInventoryOpen;

    private void Awake()
    {
        isInventoryOpen = true;
        inventoryPanel.SetActive(true);
        InitializeUI();
        isInventoryOpen = false;
        inventoryPanel.SetActive(false);
        Debug.Log("Inventory UI activated.");
    }

    private void Update()
    {
        // Reinitialize number of UI slots if the inventory changes
        if (inventorySystem.inventorySlots.Length != slots.Length)
        {
            ReinitializeUI();
        }
        // Otherwise, update the slots normally
        else
        {
            // UPDATE THE UI OF THE INVENTORY
            if (isInventoryOpen)
            {
                UpdateUI();
            }
        }
    }

    //----------------------------
    //------HELPER FUNCTIONS------
    //----------------------------

    public void ToggleInventoryDisplay()
    {
        if (isInventoryOpen)
        {
            inventoryPanel.SetActive(false);
            isInventoryOpen = false;
        }
        else
        {
            inventoryPanel.SetActive(true);
            isInventoryOpen = true;
            UpdateUI();
        }
    }

    //-----------------------------
    //------UI INITILIZATIONS------
    //-----------------------------

    public void ReinitializeUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Destroy(slots[i]);
        }
        InitializeUI();
    }

    public void InitializeUI()
    {
        slots = new GameObject[inventorySystem.inventorySlots.Length];
        for (int i = 0; i < inventorySystem.inventorySlots.Length; i++)
        {
            slots[i] = Instantiate(slotPrefab, gameObject.transform);
            int j = i;
            slots[i].GetComponent<InventorySlotUI>().slot = j;
        }
        UpdateUI();
    }

    //-----------------------------
    //-----UI UPDATE FUNCTIONS-----
    //-----------------------------

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (inventorySystem.inventorySlots[i] == null)
            {
                slots[i].GetComponentsInChildren<Image>()[1].color = new Color(slots[i].GetComponentsInChildren<Image>()[1].color.r, slots[i].GetComponentsInChildren<Image>()[1].color.g, slots[i].GetComponentsInChildren<Image>()[1].color.b, 0);
                slots[i].GetComponentsInChildren<Image>()[1].overrideSprite = null;
                slots[i].GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            }
            else
            {
                if (inventorySystem.inventorySlots[i].weapon != null)
                {
                    slots[i].GetComponentInChildren<TextMeshProUGUI>().enabled = false;
                }
                else
                {
                    slots[i].GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                }
                slots[i].GetComponentsInChildren<Image>()[1].color = new Color(slots[i].GetComponentsInChildren<Image>()[1].color.r, slots[i].GetComponentsInChildren<Image>()[1].color.g, slots[i].GetComponentsInChildren<Image>()[1].color.b, 255);
                slots[i].GetComponentsInChildren<Image>()[1].overrideSprite = inventorySystem.inventorySlots[i].icon;
                slots[i].GetComponentInChildren<TextMeshProUGUI>().text = inventorySystem.amountsPerSlot[i].ToString();
            }
        }
    }
}
