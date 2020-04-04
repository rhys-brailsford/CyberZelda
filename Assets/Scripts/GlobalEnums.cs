using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEnums : MonoBehaviour
{
}

public enum Direction { North, East, South, West };
public enum BlockDirection { X, Z };
public enum Tags { Blocking, CamBlocking, Enemy, PlayerWeapon };