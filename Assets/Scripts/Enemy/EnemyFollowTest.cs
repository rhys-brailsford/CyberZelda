using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class EnemyFollowTest : MonoBehaviour
{
    public NavMeshAgent agent;

    public LayerMask playerLayer;
    public float staticSightRange = 10;
    public float followSightRange = 15;
    private float curSightRange;
    public bool debug = false;

    private bool following = false;
    private RaycastHit hitInfo;

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
        curSightRange = following ? followSightRange : staticSightRange;

        Vector3 playerPos = GameManager.GM.GetPlayer().transform.position;

        Ray ray = new Ray(gameObject.transform.position, playerPos - gameObject.transform.position);
        bool result = Physics.Raycast(ray, out hitInfo, curSightRange, playerLayer);

        if (debug)
        {
            Debug.DrawLine(gameObject.transform.position, agent.destination, Color.red);
            Debug.Log("pos:" + gameObject.transform.position);
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
