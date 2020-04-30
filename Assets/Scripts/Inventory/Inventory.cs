using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // A temporary list of inventory items to populate hashset on start
    public List<ItemName> tempInvItems = new List<ItemName>();

    public HashSet<ItemName> inventoryItems;
    public ItemName equippedItem = ItemName.Undefined;

    // Start is called before the first frame update
    void Start()
    {
        inventoryItems = new HashSet<ItemName>();

        // :TODO:
        // Delete, this is only here to allow me to populate inventoryItems on load
        foreach (ItemName name in tempInvItems)
        {
            inventoryItems.Add(name);
        }

        equippedItem = ItemName.Undefined;
    }

    public void AddItem(ItemName item)
    {
        // Assert that item is unique in list of inventory items
        Debug.Assert(!inventoryItems.Contains(item), "Duplicate item added to player inventory: " + item.ToString());
        int sizeBefore = inventoryItems.Count;
        inventoryItems.Add(item);

        // If this is the first inventory item picked up, equip it.
        if (equippedItem == ItemName.Undefined)
        {
            equippedItem = item;
        }
    }

    public bool EquipItem(ItemName itemName)
    {
        // Equip fails if item is not in inventory
        if (!inventoryItems.Contains(itemName))
        {
            Debug.LogError("Cannot equip " + itemName.ToString() + ", not in inventory");
            return false;
        }

        try
        {
            // Try cast item to an InventoryItem to ensure it is equipable
            InventoryItem invItem = (InventoryItem)ItemList.IL.GetItem(itemName);
            equippedItem = itemName;
        }
        catch (System.InvalidCastException e)
        {
            Debug.LogError("Only InventoryItems can be equipped. Trying to equip " + itemName.ToString() + " when it is not an InventoryItem object");
            Debug.LogError(e.StackTrace);
            return false;
        }


        // Succesful equip
        return true;
    }

    public ItemName GetEquipped()
    {
        return equippedItem;
    }
}
