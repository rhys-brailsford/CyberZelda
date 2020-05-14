using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyHitbox : MonoBehaviour
{
    public EnemyHealth healthScr;
    public EnemyMovement movementScr;

    void OnTriggerEnter(Collider collider)
    {
        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.PlayerWeapon))
        {
            bool invuln = healthScr.IsInvulnerable();

            if (!invuln)
            {
                healthScr.TakeDamage(collider.GetComponent<Weapon>().damage);
                Vector3 diff = gameObject.transform.position - GameManager.GM.GetPlayer().transform.position;
                diff.y = 0;
                Vector3 knockbackDir = Vector3.Normalize(diff);
                movementScr.Knockback(knockbackDir);


                // Perform knockback
                // I tried putting the knockback movement into a coroutine,
                // but it had unexpected jerky behaviour. Considering it needs to update
                // every physics update anyway, might as well put it in FixedUpdate()
            }

            movementScr.Aggro();
        }
    }
}

