using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    public EnemyHealth healthScr;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == Tags.PlayerWeapon.ToString())
        {
            //Debug.Log("enemy hit!");
            healthScr.Hit(collision.GetComponent<Weapon>().damage);
        }
    }
}
