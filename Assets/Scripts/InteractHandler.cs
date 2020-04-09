using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractHandler : MonoBehaviour
{
    public GameObject selectedObj = null;

    void OnTriggerEnter(Collider collider)
    {
        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.Interactive) || tags.Contains(Tags.Grabbable)) {
            if (selectedObj == null)
            {
                selectedObj = collider.gameObject;
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.Interactive) || tags.Contains(Tags.Grabbable))
        {
            if (selectedObj == collider.gameObject)
            {
                selectedObj = null;
            }
        }
    }

    void Update()
    {
        if (selectedObj == null)
        {
            return;
        }

        float interactInput = Input.GetAxisRaw("Interact");
        List<Tags> tags = selectedObj.GetComponent<CustomTags>().tags;

        if (interactInput > 0 && gameObject.GetComponentInParent<PlayerMovement>().InputMoveable())
        {
            if (tags.Contains(Tags.Grabbable) && tags.Contains(Tags.Interactive))
            {
                Debug.Log("Picking up: " + selectedObj);
                selectedObj.GetComponent<InteractiveObj>().Interact(gameObject.transform.parent.gameObject);
                //gameObject.GetComponentInParent<PlayerMovement>().Grab(selectedObj);
                //gameObject.GetComponentInParent<PlayerMovement>().PickupObject();
            }
            else if (tags.Contains(Tags.Grabbable))
            {
                // If something is grabbable but not interactive, behaviour is responsibility of PC
                gameObject.GetComponentInParent<PlayerMovement>().Grab(selectedObj);
            }
            else if (tags.Contains(Tags.Interactive))
            {
                // If something is interactive but not grabbable, behaviour is responsibility of object
                selectedObj.GetComponent<InteractiveObj>().Interact(gameObject.transform.parent.gameObject);
            }
        }
        if (interactInput == 0)
        {
            if (tags.Contains(Tags.Grabbable))
            {
                gameObject.GetComponentInParent<PlayerMovement>().Ungrab();
            }
        }
        
    }
}
