using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public ItemName itemName;
    public Mesh staticMesh;
    public Material mat;

    public void Init(ItemName name, Mesh sm, Material m)
    {
        itemName = name;
        staticMesh = sm;
        mat = m;
    }

    public abstract void PickupUse();
}
