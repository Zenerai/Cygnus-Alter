using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemObject : ScriptableObject
{
    public string itemName;
    public Sprite icon;           // For UI usage
    public string description;    // For UI usage

    // Tag to identify what this item unlocks
    public string unlockTag;      // "Door_1"
}