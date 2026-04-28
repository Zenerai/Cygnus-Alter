using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class KeyDisplayUI : MonoBehaviour
{
    [Header("Key Slots")]
    public Image slot1;
    public Image slot2;
    public Image slot3;

    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = InventoryManager.Instance;
        ClearSlots();
    }

    void Update()
    {
        if (inventoryManager == null) return;

        List<ItemObject> items = inventoryManager.GetInventory();

        // Clear all slots first
        ClearSlots();

        // Display up to 3 keys
        for (int i = 0; i < items.Count && i < 3; i++)
        {
            if (items[i].icon != null)
            {
                if (i == 0) slot1.sprite = items[i].icon;
                else if (i == 1) slot2.sprite = items[i].icon;
                else if (i == 2) slot3.sprite = items[i].icon;
            }
        }
    }

    private void ClearSlots()
    {
        if (slot1) slot1.sprite = null;
        if (slot2) slot2.sprite = null;
        if (slot3) slot3.sprite = null;
    }
}