using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public ItemObject item;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager inventory = FindObjectOfType<InventoryManager>();
            if (inventory != null && inventory.AddItem(item))
            {
                // Plays a sound or particles with code that would go here
                Destroy(gameObject);
            }
        }
    }
}