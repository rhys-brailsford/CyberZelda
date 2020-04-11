using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEnums : MonoBehaviour
{
}

public enum Direction { North, East, South, West };
public enum BlockDirection { X, Z };
public enum Tags { Undefined, Blocking, CamBlocking, Enemy, PlayerWeapon, PlayerHitbox, Interactive, Grabbable, Draggable };
public enum InteractState { Idle, Grabbing, PickingUp, Holding, Throwing, Interacting };
public enum MovementState { Idle, Walking, KnockedBack }