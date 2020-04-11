using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableObj : InteractiveObj
{
    private bool thrown = false;
    private Vector3 throwDest;
    private Vector3 throwSrc;
    private float flightDur = 1;
    private float curFlightDur = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (thrown)
        {
            ThrowUpdate();
        }
    }

    public override void Interact(GameObject player)
    {
        gameObject.GetComponent<Collision>().collide = false;
        player.GetComponent<PlayerMovement>().PickupObject(gameObject);
    }

    public void Throw(Vector3 destination)
    {
        thrown = true;
        throwDest = destination;
        throwSrc = gameObject.transform.position;
        curFlightDur = 0;
    }
    
    private void ThrowUpdate()
    {
        if (curFlightDur > flightDur)
        {
            // explode and kill
            gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", Color.red);
        }

        Vector3 newPos = Vector3.Lerp(throwSrc, throwDest, curFlightDur / flightDur);
        gameObject.transform.position = newPos;

        curFlightDur += Time.deltaTime;
    }
}
