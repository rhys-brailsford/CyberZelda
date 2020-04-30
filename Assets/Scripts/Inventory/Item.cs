using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public ItemName itemName;
    public Mesh staticMesh;
    public Material mat;
    public Mesh col;

    public void Init(ItemName name, Mesh sm, Material m, Mesh collider)
    {
        itemName = name;
        staticMesh = sm;
        mat = m;
        col = collider;
    }

    public abstract void PickupUse();
}
