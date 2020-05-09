using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public float invDuration = 1;

    private bool isInvuln = false;

    public List<Collider> bufferedColliders = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(gameObject.GetComponent<PlayerMovement>().knockbackDuration < invDuration, 
            "Player knockback duration should be less than invulnerability duration.");
    }

    private IEnumerator Invulnerability(float invulnDuration)
    {
        Debug.Log("coroutine started");
        isInvuln = true;
        gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", Color.red);

        yield return new WaitForSeconds(invulnDuration);

        // Turn invulnerability off
        isInvuln = false;
        gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", Color.green);

        // Check buffered colliders
        Bounds playerBounds = gameObject.GetComponent<Collider>().bounds;
        // :TODO:
        // Sort colliders by distance from player so shortest distance (deepest intersect) takes priority
        foreach (Collider col in bufferedColliders)
        {
            if (col.bounds.Intersects(playerBounds))
            {
                GetHit(col);
                gameObject.GetComponent<PlayerMovement>().Knockback(col.transform.position);
                break;
            }
        }
        bufferedColliders.Clear();
        Debug.Log("coroutine ended");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.Enemy)) 
        {
            GetHit(collider);
        }
    }

    void GetHit(Collider src)
    {
        float damagePoints = src.GetComponent<EnemyHurtbox>().damage;
        if (isInvuln)
        {
            // Buffer
            bufferedColliders.Add(src);
            return;
        }

        float curHealth = PlayerStats.PS.TakeDamage(damagePoints);
        if (curHealth < 0)
        {
            // Die
            // :TODO:
            // Add death functionality.
        }

        if (gameObject.GetComponent<PlayerMovement>().Moveable())
        {
            // Knockback
            Vector3 srcPosition = src.transform.position;
            gameObject.GetComponent<PlayerMovement>().Knockback(srcPosition);
        }

        StartCoroutine(Invulnerability(invDuration));
    }
}
