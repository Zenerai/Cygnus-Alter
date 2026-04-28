using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private List<ItemObject> inventory = new List<ItemObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool AddItem(ItemObject newItem)
    {
        if (newItem == null) return false;

        inventory.Add(newItem);
        Debug.Log($"Collected: {newItem.itemName}");

        // Automatically remove/unlock doors that this item opens
        RemoveDoorsForItem(newItem);

        return true;
    }

    // Update Door System
    private void RemoveDoorsForItem(ItemObject item)
    {
        if (string.IsNullOrEmpty(item.unlockTag))
            return;

        GameObject[] doors = GameObject.FindGameObjectsWithTag(item.unlockTag);

        foreach (GameObject door in doors)
        {
            Debug.Log($"Unlocking door: {door.name}");
            Destroy(door);           // Removes the door completely
            // door.SetActive(false); // Alternative: just disable instead of destroy
        }
    }

    // Check if player has item
    public bool HasItem(string itemName)
    {
        return inventory.Exists(item => item.itemName == itemName);
    }

    // For future UI
    public List<ItemObject> GetInventory()
    {
        return inventory;
    }
}