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

    private bool isSelected = false;
    private bool isFacingFront = false;

    private float displayDuration = 3;

    [SerializeField]
    private Renderer renderer;
    [SerializeField]
    private MaterialPropertyBlock propBlock;
    
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

        if (isFacingFront)
        {
            // Open item box
            Debug.Log("ITEM BOX INTERACTION SUCCESS");
            opened = true;
            // Turn off highlight effect
            propBlock.SetInt("_IsActive", 0);
            renderer.SetPropertyBlock(propBlock);

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
    }

    public override void Selected()
    {
        Debug.Log("SELECTED!");
        isSelected = true;


        propBlock.SetInt("_IsSelected", 1);
        renderer.SetPropertyBlock(propBlock);

        //gameObject.GetComponent<MeshRenderer>().sharedMaterial.SetInt("_IsSelected", 1);
        //throw new System.NotImplementedException();
    }
    public override void Deselected()
    {
        Debug.Log("DESELECTED!");
        isSelected = false;

        propBlock.SetInt("_IsSelected", 0);
        renderer.SetPropertyBlock(propBlock);

        //gameObject.GetComponent<MeshRenderer>().sharedMaterial.SetInt("IsSelected", 0);
        //throw new System.NotImplementedException();
    }


    // Start is called before the first frame update
    private void Start()
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

        propBlock = new MaterialPropertyBlock();
        renderer = gameObject.GetComponent<Renderer>();
        renderer.GetPropertyBlock(propBlock);
    }

    private void SetHighlight(bool isSelected)
    {
        bool curSelectionValue = propBlock.GetInt("_IsSelected") > 0.5f ? true : false;

        // If there is no change, we don't need to do anything
        if (isSelected == curSelectionValue)
        {
            return;
        }

        // Because there is a change, we need to toggle the value
        int newSelectionValue = isSelected ? 1 : 0;
        propBlock.SetInt("_IsSelected", newSelectionValue);
        renderer.SetPropertyBlock(propBlock);
    }

    private void Update()
    {
        if (isSelected && !opened)
        {
            float playerAngle = GameManager.GM.GetPlayer().GetComponent<PlayerMovement>().GetDirectionDeg();

            isFacingFront = Mathf.Approximately((playerAngle + 180) % 360, forwardDirDeg);

            SetHighlight(isFacingFront);
        }
            
    }
}
