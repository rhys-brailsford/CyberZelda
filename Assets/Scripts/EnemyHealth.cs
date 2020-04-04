using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float healthPoints = 100;
    public float invDuration = 1;

    private bool isInvuln = false;
    private float curInvDuration = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", Color.green);
    }

    // Update is called once per frame
    void Update()
    {
        if (curInvDuration >= invDuration)
        {
            isInvuln = false;
            gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", Color.green);

        }
        if (isInvuln)
        {
            gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", Color.red);
            curInvDuration += Time.deltaTime;
        }
    }

    public void Hit(float damagePoints)
    {
        if (isInvuln) return;

        Debug.Log("ENEMY TAKE DAMAGE");
        isInvuln = true;
        curInvDuration = 0;
        healthPoints -= damagePoints;

        if (healthPoints < 0)
        {
            // die
        }
    }
}
