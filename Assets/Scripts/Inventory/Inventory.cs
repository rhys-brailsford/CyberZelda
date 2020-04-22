using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemName> inventoryItems;
    public InventoryItem equippedItem = null;
    // Start is called before the first frame update
    void Start()
    {
        inventoryItems = new List<ItemName>();
    }

    public void AddItem(ItemName item)
    {
        // Assert that item is unique in list of inventory items
        Debug.Assert(inventoryItems.FindIndex(f => f == item) == -1, "Duplicate item added to player inventory: " + item.ToString());
        inventoryItems.Add(item);
    }
}
