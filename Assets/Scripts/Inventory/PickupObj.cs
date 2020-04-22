using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObj : MonoBehaviour
{
    public ItemName item;
    private Item obj;

    // Start is called before the first frame update
    void Start()
    {
        obj = ItemList.IL.GetItem(item);

        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        MeshCollider collider = gameObject.GetComponent<MeshCollider>();
        Debug.Assert(filter != null, gameObject.name + " expected to have a " + filter.GetType());
        Debug.Assert(renderer != null, gameObject.name + " expected to have a " + renderer.GetType());
        Debug.Assert(collider != null, gameObject.name + " expected to have a " + collider.GetType());
        gameObject.GetComponent<MeshFilter>().sharedMesh = obj.staticMesh;
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = obj.mat;
        gameObject.GetComponent<MeshCollider>().sharedMesh = obj.staticMesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print("TRIGGERED by " + other.name);
        List<Tags> tags = other.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.PlayerHitbox))
        {
            obj.PickupUse();
            Destroy(gameObject);
        }
    }
}
