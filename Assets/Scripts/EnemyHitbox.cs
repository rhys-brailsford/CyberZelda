﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public EnemyHealth healthScr;

    void OnTriggerEnter(Collider collider)
    {
        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.PlayerWeapon))
        {
            healthScr.TakeDamage(collider.GetComponent<Weapon>().damage);
        }
    }
}

