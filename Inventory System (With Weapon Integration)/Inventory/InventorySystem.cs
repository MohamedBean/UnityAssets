using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Progress;

public class InventorySystem : MonoBehaviour
{
    public int numberOfSlots;
    public InventoryItemObject[] inventorySlots;
    public int[] amountsPerSlot;
    private int oldNumberOfSlots;

    void Awake()
    {
        oldNumberOfSlots = numberOfSlots;
        InitializeInventory();
    }

    private void Update()
    {
        if (oldNumberOfSlots != numberOfSlots)
        {
            ReInitializeInventory();
        }
    }

    //------------------------------------
    //------INVENTORY REINITIALIZATION------
    //------------------------------------
    private void ReInitializeInventory()
    {
        // COPY ITEMS FROM OLD INVENTORY TO NEW ONE
        InventoryItemObject[] newSlots = new InventoryItemObject[numberOfSlots];
        int[] newAmounts = new int[numberOfSlots];
        if (inventorySlots.Length > newSlots.Length)
        {
            for (int i = 0; i < newSlots.Length; i++)
            {
                newSlots[i] = inventorySlots[i];
                newAmounts[i] = amountsPerSlot[i];
            }
        }
        else
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                newSlots[i] = inventorySlots[i];
                newAmounts[i] = amountsPerSlot[i];
            }
        }
        inventorySlots = newSlots;
        amountsPerSlot = newAmounts;
        oldNumberOfSlots = numberOfSlots;
    }

    //------------------------------------
    //------INVENTORY INITIALIZATION------
    //------------------------------------
    private void InitializeInventory()
    {
        inventorySlots = new InventoryItemObject[numberOfSlots];
        amountsPerSlot = new int[numberOfSlots];
    }

    //----------------------------
    //------HELPER FUNCTIONS------
    //----------------------------
    
    public void ClearSlot(int slot)
    {
        inventorySlots[slot] = null;
        amountsPerSlot[slot] = 0;
    }

    public int HowMuchDoesInventoryHaveOfThisItem(InventoryItemObject item)
    {
        int calculatedAmount = 0;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == item)
            {
                calculatedAmount += amountsPerSlot[i];
            }
        }
        return calculatedAmount;
    }

    public bool DoesInventoryHaveAtleastThisMuchOfItem(InventoryItemObject item, int amount)
    {
        int calculatedAmount = 0;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == item)
            {
                calculatedAmount += amountsPerSlot[i];
            }
            if (calculatedAmount >= amount)
            {
                return true;
            }
        }
        return false;
    }

    public bool DoesInventoryHaveEnoughSpaceToAddItem(InventoryItemObject item, int amount)
    {
        int calculatedAmount = 0;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // IF ITEM EXISTS
            if (inventorySlots[i] == item && amountsPerSlot[i] + amount > inventorySlots[i].maxStackAmount)
            {
                calculatedAmount += inventorySlots[i].maxStackAmount - amountsPerSlot[i];
            }
            if (inventorySlots[i] == item && amountsPerSlot[i] + amount <= inventorySlots[i].maxStackAmount)
            {
                return true;
            }
            // IF ITEM DOES NOT EXIST
            if (inventorySlots[i] == null && amount > item.maxStackAmount)
            {
                calculatedAmount += item.maxStackAmount;
            }
            if (inventorySlots[i] == null && amount <= item.maxStackAmount)
            {
                return true;
            }
            // IF THERE IS ENOUGH SPACE
            if (calculatedAmount >= amount)
            {
                return true;
            }
        }
        return false;
    }

    //-------------------------------
    //------ADDING TO INVENTORY------
    //-------------------------------
    public void AddToInventory(InventoryItemObject item, int amount, int slot)
    {
        // CONVERT WEAPONS INTO THEIR OWN OBJECT
        if (item.weapon != null)
        {
            InventoryItemObject instance = ScriptableObject.CreateInstance<InventoryItemObject>();
            Weapon weaponInstance = ScriptableObject.CreateInstance<Weapon>();
            weaponInstance.ammoItem = item.weapon.ammoItem;
            weaponInstance.weaponItem = item.weapon.weaponItem;
            weaponInstance.currentAmmo = item.weapon.currentAmmo;
            weaponInstance.shootingSpeed = item.weapon.shootingSpeed;
            weaponInstance.damage = item.weapon.damage;
            weaponInstance.clipSize = item.weapon.clipSize;
            instance.icon = item.icon;
            instance.itemName = item.itemName;
            instance.maxStackAmount = item.maxStackAmount;
            instance.weapon = weaponInstance;
            instance.name = item.name;
            item = instance;
        }
        // IF ITEM EXISTS
        if (inventorySlots[slot] == item && amountsPerSlot[slot] + amount > inventorySlots[slot].maxStackAmount)
        {
            amountsPerSlot[slot] = inventorySlots[slot].maxStackAmount;
        }
        if (inventorySlots[slot] == item && amountsPerSlot[slot] + amount <= inventorySlots[slot].maxStackAmount)
        {
            amountsPerSlot[slot] = amountsPerSlot[slot] + amount;
        }
        // IF ITEM DOES NOT EXIST
        if (inventorySlots[slot] == null && amount > item.maxStackAmount)
        {
            inventorySlots[slot] = item;
            amountsPerSlot[slot] = inventorySlots[slot].maxStackAmount;
        }
        if (inventorySlots[slot] == null && amount <= item.maxStackAmount)
        {
            inventorySlots[slot] = item;
            amountsPerSlot[slot] = amount;
        }
    }

    public void AddToInventory(InventoryItemObject item, int amount)
    {
        // CONVERT WEAPONS INTO THEIR OWN OBJECT
        if (item.weapon != null)
        {
            InventoryItemObject instance = ScriptableObject.CreateInstance<InventoryItemObject>();
            Weapon weaponInstance = ScriptableObject.CreateInstance<Weapon>();
            weaponInstance.ammoItem = item.weapon.ammoItem;
            weaponInstance.weaponItem = item.weapon.weaponItem;
            weaponInstance.currentAmmo = item.weapon.currentAmmo;
            weaponInstance.shootingSpeed = item.weapon.shootingSpeed;
            weaponInstance.damage = item.weapon.damage;
            weaponInstance.clipSize = item.weapon.clipSize;
            instance.icon = item.icon;
            instance.itemName = item.itemName;
            instance.maxStackAmount = item.maxStackAmount;
            instance.weapon = weaponInstance;
            item = instance;
        }
        if (amount <= 0)
        {
            return;
        }
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // IF ITEM EXISTS
            if (inventorySlots[i] == item && amountsPerSlot[i] + amount > inventorySlots[i].maxStackAmount)
            {
                amount -= inventorySlots[i].maxStackAmount - amountsPerSlot[i];
                amountsPerSlot[i] = inventorySlots[i].maxStackAmount;
            }
            if (inventorySlots[i] == item && amountsPerSlot[i] + amount <= inventorySlots[i].maxStackAmount)
            {
                amountsPerSlot[i] = amountsPerSlot[i] + amount;
                break;
            }
            // IF ITEM DOES NOT EXIST
            if (inventorySlots[i] == null && amount > item.maxStackAmount)
            {
                amount -= item.maxStackAmount;
                inventorySlots[i] = item;
                amountsPerSlot[i] = inventorySlots[i].maxStackAmount;
            }
            if (inventorySlots[i] == null && amount <= item.maxStackAmount)
            {
                inventorySlots[i] = item;
                amountsPerSlot[i] = amount;
                break;
            }
            // WHILE AN AMOUNT CAN STILL BE ADDED:
            if (amount <= 0)
            {
                break;
            }
        }
    }

    //------------------------------------
    //------REMOVING FROM INVENTORY-------
    //------------------------------------

    public void RemoveFromInventory(InventoryItemObject item, int amount)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // IF ITEM EXISTS
            if (inventorySlots[i] == item && amount >= amountsPerSlot[i])
            {
                amount -= amountsPerSlot[i];
                amountsPerSlot[i] = 0;
                inventorySlots[i] = null;
            }
            if (inventorySlots[i] == item && amount < amountsPerSlot[i])
            {
                amountsPerSlot[i] -= amount;
                break;
            }
            // WHILE AN AMOUNT CAN STILL BE REMOVED:
            if (amount <= 0)
            {
                break;
            }
        }
    }

    public void RemoveFromInventory(InventoryItemObject item, int amount, int slot)
    {
        // IF ITEM EXISTS
        if (inventorySlots[slot] == item && amount >= amountsPerSlot[slot])
        {
            amountsPerSlot[slot] = 0;
            inventorySlots[slot] = null;
        }
        if (inventorySlots[slot] == item && amount < amountsPerSlot[slot])
        {
            amountsPerSlot[slot] -= amount;
        }
    }
}
