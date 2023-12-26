using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventorySystem))]
public class CanPickUpItems : MonoBehaviour
{
    public float CanPickUpItemWithLifeTime;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PickUp") && collision.gameObject.GetComponent<ItemFloater>().itemLife >= CanPickUpItemWithLifeTime && GetComponent<InventorySystem>().DoesInventoryHaveEnoughSpaceToAddItem(collision.gameObject.GetComponent<ItemFloater>().item, collision.gameObject.GetComponent<ItemFloater>().amount))
        {
            GetComponent<InventorySystem>().AddToInventory(collision.gameObject.GetComponent<ItemFloater>().item, collision.gameObject.GetComponent<ItemFloater>().amount);
            Destroy(collision.gameObject);
        }
    }
}
