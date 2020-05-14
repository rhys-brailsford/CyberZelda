using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEnums : MonoBehaviour
{
}

public enum Direction { North, East, South, West };
public enum BlockDirection { PosX, PosZ, NegX, NegZ, Null };
public enum Tags { Undefined, Blocking, CamBlocking, Enemy, PlayerWeapon, PlayerHitbox, PlayerTrigger, Interactive, Grabbable, Draggable };
public enum InteractState { Idle, Grabbing, PickingUp, Holding, Throwing, Interacting };
public enum MovementState { Idle, Walking, KnockedBack };
public enum ItemName { Undefined, Heart, Ammo, Gun, TempInvItem1 };
public enum EnemyState { Passive, Aggro };
public enum EnemyMovementState { Idle, Following, Leashing, Resetting, Patrolling };
public enum LevelName { TestLevel1, TestLevel2, TestLevel3 };

