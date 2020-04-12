using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractHandler : MonoBehaviour
{
    public GameObject selectedObj = null;

    void OnTriggerEnter(Collider collider)
    {
        CustomTags tagsComp = collider.GetComponent<CustomTags>();
        Debug.Assert(tagsComp != null, collider.name + ": has no custom tags component");
        List<Tags> tags = tagsComp.tags;
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
            InteractState state = gameObject.GetComponentInParent<PlayerMovement>().interactState;
            if (selectedObj == collider.gameObject && state == InteractState.Idle)
            {
                selectedObj = null;
            }
        }
    }

    public void StartInteract()
    {
        if (selectedObj == null)
        {
            return;
        }

        List<Tags> tags = selectedObj.GetComponent<CustomTags>().tags;
        InteractState state = gameObject.GetComponentInParent<PlayerMovement>().interactState;

        if (state == InteractState.Holding)
        {
            gameObject.GetComponentInParent<PlayerMovement>().ThrowObj();
        }
        else if (state == InteractState.Throwing)
        {
            return;
        }
        // Grabbable needs to take precedence over Interactive as objects that are both
        // need to be grabbed (e.g. draggable objects)
        else if (tags.Contains(Tags.Grabbable))
        {
            // Grab
            gameObject.GetComponentInParent<PlayerMovement>().Grab(selectedObj);
        }
        else if (tags.Contains(Tags.Interactive))
        {
            // Interact
            selectedObj.GetComponent<InteractiveObj>().Interact(gameObject.transform.parent.gameObject);
        }
    }

    public void EndInteract()
    {
        if (selectedObj == null)
        {
            return;
        }

        InteractState state = gameObject.GetComponentInParent<PlayerMovement>().interactState;
        if (state == InteractState.Grabbing)
        {
            // Ungrab
            gameObject.GetComponentInParent<PlayerMovement>().Ungrab();
        }
    }

    void Update()
    {
        //if (selectedObj == null)
        //{
        //    return;
        //}

        //float interactInput = Input.GetAxisRaw("Interact");
        //List<Tags> tags = selectedObj.GetComponent<CustomTags>().tags;
        //InteractState state = gameObject.GetComponentInParent<PlayerMovement>().interactState;

        //if (interactInput > 0 && state == InteractState.Holding)
        //{
        //    gameObject.GetComponentInParent<PlayerMovement>().ThrowObj();
        //    return;
        //}
        //if (interactInput > 0 && gameObject.GetComponentInParent<PlayerMovement>().InputMoveable())
        //{
        //    if (tags.Contains(Tags.Grabbable) && tags.Contains(Tags.Interactive))
        //    {
        //        selectedObj.GetComponent<InteractiveObj>().Interact(gameObject.transform.parent.gameObject);
        //    }
        //    else if (tags.Contains(Tags.Grabbable))
        //    {
        //        // If something is grabbable but not interactive, behaviour is responsibility of PC
        //        gameObject.GetComponentInParent<PlayerMovement>().Grab(selectedObj);
        //    }
        //    else if (tags.Contains(Tags.Interactive))
        //    {
        //        // If something is interactive but not grabbable, behaviour is responsibility of object
        //        selectedObj.GetComponent<InteractiveObj>().Interact(gameObject.transform.parent.gameObject);
        //    }
        //}
        //if (interactInput == 0 && state == InteractState.Grabbing)
        //{
        //    if (tags.Contains(Tags.Grabbable))
        //    {
        //        gameObject.GetComponentInParent<PlayerMovement>().Ungrab();
        //    }
        //}
    }
}
