using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartItem : Item
{
    private float healthValue = 20;

    public override void PickupUse()
    {
        //:TODO:
        // Add functionality of heart use. Should increase hp of player
        Debug.Log("Heart picked up and used");
        PlayerStats.PS.IncreaseHealth(healthValue);
    }
}
