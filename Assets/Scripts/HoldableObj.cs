using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableObj : InteractiveObj
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact(GameObject player)
    {
        //Debug.Log("holdable interact!");
        //Destroy(gameObject);
        gameObject.GetComponent<Collision>().collide = false;
        //player.GetComponent<PlayerMovement>().Grab(gameObject);
        player.GetComponent<PlayerMovement>().PickupObject(gameObject);
    }
}
