﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunItem : InventoryItem
{
    public override void PickupUse()
    {
        // Add item to players inventory on pickup
        //GameManager.GM.GetPlayer().GetComponent<Inventory>().AddItem(ItemName.Gun);
        GameManager.GM.GetInventory().AddItem(ItemName.Gun);
    }

    public override void UseItem()
    {
        Debug.Log("GUN USEITEM");
        throw new System.NotImplementedException();
    }
}
