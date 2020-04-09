using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHurtbox : MonoBehaviour
{
    public float damage = 5;

    void OnTriggerEnter(Collider collider)
    {
        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.PlayerHitbox))
        {
            //collider.
        }
    }
}
