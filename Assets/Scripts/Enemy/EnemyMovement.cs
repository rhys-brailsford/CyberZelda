using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent agent;

    // This shouldn't be called playerLayer as this is a layerMask and therefore actually uses multiple layers
    // and expects multiple layers to be selected in the inspector
    public LayerMask sightLayers;
    public float staticSightRange = 10;
    public float followSightRange = 15;
    private float curSightRange;
    public bool debug = false;

    private bool following = false;
    private RaycastHit hitInfo;

    public EnemyState enemyState { get; private set; }

    public float knockbackDuration = 1;
    public float knockbackStrength = 10;
    private bool knockedBack = false;

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Color myColor = Color.blue;
            myColor.a = 0.5f;
            Gizmos.color = myColor;
            Gizmos.DrawWireSphere(gameObject.transform.position, curSightRange);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyState == EnemyState.Aggro && !knockedBack)
        {
            curSightRange = following ? followSightRange : staticSightRange;

            Vector3 playerPos = GameManager.GM.GetPlayer().transform.position;

            Ray ray = new Ray(gameObject.transform.position, playerPos - gameObject.transform.position);
            bool result = Physics.Raycast(ray, out hitInfo, curSightRange, sightLayers);

            if (debug)
            {
                Debug.DrawLine(gameObject.transform.position, agent.destination, Color.red);
            }

            if (result)
            {
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    agent.SetDestination(playerPos);
                    following = true;
                }
            }
            else
            {
                following = false;
            }
        }
    }

    public void Aggro()
    {
        enemyState = EnemyState.Aggro;
    }

    public void Knockback(Vector3 direction)
    {
        StartCoroutine("KnockbackCoroutine", direction);
    }

    private IEnumerator KnockbackCoroutine(Vector3 direction)
    {
        float duration = 0;
        agent.isStopped = true;
        knockedBack = true;

        while (duration < knockbackDuration)
        {
            gameObject.transform.position = gameObject.transform.position + (direction * knockbackStrength * Time.deltaTime);
            duration += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        knockedBack = false;
        agent.isStopped = false;
    }
}
