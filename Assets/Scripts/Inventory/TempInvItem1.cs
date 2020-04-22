using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempInvItem1 : InventoryItem
{
    public override void PickupUse()
    {
        Debug.Log("TEMP ITEM PICKUP");
        throw new System.NotImplementedException();
    }

    public override void UseItem()
    {
        Debug.Log("TEMPITEM USE");
        throw new System.NotImplementedException();
    }
}
