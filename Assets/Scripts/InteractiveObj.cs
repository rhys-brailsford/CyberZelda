using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveObj : MonoBehaviour
{
    public abstract void Interact(GameObject player);
    public abstract void Selected();
    public abstract void Deselected();
}
