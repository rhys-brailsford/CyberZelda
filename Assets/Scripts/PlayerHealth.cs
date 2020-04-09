using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float healthPoints = 100;
    public float invDuration = 1;

    private bool isInvuln = false;
    private float curInvDuration = 0;

    public List<Collider> bufferedColliders = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(gameObject.GetComponent<PlayerMovement>().knockbackDuration < invDuration, "Player knockback duration should be less than invulnerability duration.");
    }

    // Update is called once per frame
    void Update()
    {
        if (curInvDuration >= invDuration && isInvuln)
        {
            isInvuln = false;
            gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", Color.green);

            // Check buffered colliders
            Bounds playerBounds = gameObject.GetComponent<BoxCollider>().bounds;
            // :TODO:
            // Sort colliders by distance from player so shortest distance (deepest intersect) takes priority
            foreach (Collider col in bufferedColliders)
            {
                if (col.bounds.Intersects(playerBounds))
                {
                    TakeDamage(col);
                    gameObject.GetComponent<PlayerMovement>().Knockback(col.transform.position);
                    break;
                }
            }
            bufferedColliders.Clear();

        }
        if (isInvuln)
        {
            gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", Color.red);
            curInvDuration += Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.Enemy)) 
        {
            TakeDamage(collider);
        }
    }

    void TakeDamage(Collider src)
    {
        float damagePoints = src.GetComponent<EnemyHurtbox>().damage;
        if (isInvuln)
        {
            // Buffer
            bufferedColliders.Add(src);
            return;
        }
        //Debug.Log("Player TAKE DAMAGE");
        healthPoints -= damagePoints;

        if (healthPoints < 0)
        {
            // die
        }

        if (gameObject.GetComponent<PlayerMovement>().Moveable())
        {
            // Knockback
            Vector3 srcPosition = src.transform.position;
            gameObject.GetComponent<PlayerMovement>().Knockback(srcPosition);
        }

        isInvuln = true;
        curInvDuration = 0;


    }
}
