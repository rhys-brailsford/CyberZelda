using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : InteractiveObj
{
    public ItemName itemContents;

    public bool opened = false;
    private Item obj;
    private GameObject displayedItem;

    private Vector3 forwardDirection;
    private float forwardDirDeg;

    private float displayDuration = 3;
    
    private IEnumerator DisplayItemAndDestroy(float displayTime)
    {
        displayedItem.GetComponent<MeshRenderer>().enabled = true;

        yield return new WaitForSeconds(displayTime);

        Destroy(displayedItem);
    }

    public override void Interact(GameObject player)
    {
        // Don't even try interact with it if already opened
        if (opened)
        {
            return;
        }

        // Check if player is facing the front of the chest
        float playerAngle = player.GetComponent<PlayerMovement>().GetDirectionDeg();

        if (Mathf.Approximately((playerAngle+180)%360, forwardDirDeg))
        {
            // Open item box
            Debug.Log("ITEM BOX INTERACTION SUCCESS");
            opened = true;
            // Pickup item
            obj.PickupUse();
            // Display item contents
            StartCoroutine("DisplayItemAndDestroy", displayDuration);
        }
        else
        {
            Debug.Log("ITEM BOX INTERACTION FAIL");

            return;
        }


        //throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        forwardDirDeg = transform.rotation.eulerAngles.y%360;

        Debug.Assert(itemContents != ItemName.Undefined, gameObject.name + " is required to contain an item! Currently undefined.");

        obj = ItemList.IL.GetItem(itemContents);

        displayedItem = new GameObject(gameObject.name + "Item");
        displayedItem.AddComponent<MeshFilter>();
        displayedItem.AddComponent<MeshRenderer>();
        displayedItem.GetComponent<MeshFilter>().sharedMesh = obj.staticMesh;
        displayedItem.GetComponent<MeshRenderer>().sharedMaterial = obj.mat;
        displayedItem.transform.SetParent(gameObject.transform);
        displayedItem.transform.localPosition = new Vector3(0, 1, 0);

        displayedItem.GetComponent<MeshRenderer>().enabled = false;


        Vector3 displayedItemPos = gameObject.transform.position;
        displayedItemPos.y = 6;

        // Chest doesnt use attribute sof object
        //MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        //MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        //MeshCollider collider = gameObject.GetComponent<MeshCollider>();
        //Debug.Assert(filter != null, gameObject.name + " expected to have a " + filter.GetType());
        //Debug.Assert(renderer != null, gameObject.name + " expected to have a " + renderer.GetType());
        //Debug.Assert(collider != null, gameObject.name + " expected to have a " + collider.GetType());
        //gameObject.GetComponent<MeshFilter>().sharedMesh = obj.staticMesh;
        //gameObject.GetComponent<MeshRenderer>().sharedMaterial = obj.mat;
        //gameObject.GetComponent<MeshCollider>().sharedMesh = obj.staticMesh;
    }

}
