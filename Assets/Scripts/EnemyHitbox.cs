using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyHitbox : MonoBehaviour
{
    public EnemyHealth healthScr;

    public float knockbackDuration = 1;
    public float knockbackStrength = 5;
    private float curKnockbackDur = 0;
    private bool knockedBack = false;

    public Vector3 knockbackDirection;

    public Rigidbody rb;
    public NavMeshAgent agent;

    void OnTriggerEnter(Collider collider)
    {
        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.PlayerWeapon))
        {
            bool invuln = healthScr.IsInvulnerable();

            if (!invuln)
            {
                healthScr.TakeDamage(collider.GetComponent<Weapon>().damage);
                knockbackDirection = Vector3.Normalize(
                    gameObject.transform.position -
                    GameManager.GM.GetPlayer().transform.position);

                // Perform knockback
                // I tried putting the knockback movement into a coroutine,
                // but it had unexpected jerky behaviour. Considering it needs to update
                // every physics update anyway, might as well put it in FixedUpdate()
                curKnockbackDur = 0;
                agent.isStopped = true;
                knockedBack = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (knockedBack)
        {
            curKnockbackDur += Time.fixedDeltaTime;

            rb.MovePosition(rb.position + (knockbackDirection * knockbackStrength * Time.fixedDeltaTime));

            if (curKnockbackDur > knockbackDuration)
            {
                knockedBack = false;
                agent.isStopped = false;
            }
        }
    }
}

