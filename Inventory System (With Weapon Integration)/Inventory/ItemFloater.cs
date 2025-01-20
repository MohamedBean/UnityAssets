using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemFloater : MonoBehaviour
{
    public InventoryItemObject item;
    public int amount;
    private SpriteRenderer sprite;
    public float itemLife = 0f;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = item.icon;
    }

    private void Update()
    {
        itemLife += Time.deltaTime;
        if (itemLife > 60f)
        {
            Destroy(gameObject);
        }
    }
}
