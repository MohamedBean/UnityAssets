using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class InventoryItemObject : ScriptableObject
{
    public Sprite icon;
    public Weapon weapon;
    public string itemName;
    public int maxStackAmount;
}
