using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collision : MonoBehaviour
{
    public bool collide = true;
    public abstract Vector3 Normal(Vector3 srcPosition);
}
