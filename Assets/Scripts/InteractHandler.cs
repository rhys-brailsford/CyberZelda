using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractHandler : MonoBehaviour
{
    public GameObject selectedObj = null;

    private HashSet<GameObject> overlappedObjs;

    private void Start()
    {
        overlappedObjs = new HashSet<GameObject>();
    }

    void OnTriggerEnter(Collider collider)
    {
        CustomTags tagsComp = collider.GetComponent<CustomTags>();
        Debug.Assert(tagsComp != null, collider.name + ": has no custom tags component");
        List<Tags> tags = tagsComp.tags;
        if (tags.Contains(Tags.Interactive) || tags.Contains(Tags.Grabbable))
        {
            overlappedObjs.Add(collider.gameObject);
            //if (selectedObj == null)
            //{
            //    SelectObj(collider.gameObject);
            //}
        }
    }

    void OnTriggerExit(Collider collider)
    {
        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.Interactive) || tags.Contains(Tags.Grabbable))
        {
            overlappedObjs.Remove(collider.gameObject);
            //InteractState state = gameObject.GetComponentInParent<PlayerMovement>().interactState;
            //if (selectedObj == collider.gameObject && state == InteractState.Idle)
            //{
            //    DeselectObj();
            //}
        }
    }

    private GameObject ClosestInteractiveObj()
    {
        GameObject result = null;

        if (overlappedObjs.Count <= 0)
        {
            return null;
        }

        float closestDistance = float.PositiveInfinity;
        Vector3 playerPos = gameObject.transform.position;

        foreach (GameObject obj in overlappedObjs)
        {
            float curDistance = Vector3.Distance(playerPos, obj.transform.position);
            if (curDistance < closestDistance)
            {
                closestDistance = curDistance;
                result = obj;
            }
        }

        return result;
    }

    private void Update()
    {
        GameObject curClosestObj = ClosestInteractiveObj();
        InteractState state = gameObject.GetComponentInParent<PlayerMovement>().interactState;
        if (selectedObj != curClosestObj)
        {
            SetSelectedObj(curClosestObj);
        }        
    }

    void SetSelectedObj(GameObject newSelection)
    {
        // Ignore new selections if picking up or holding object
        InteractState state = gameObject.GetComponentInParent<PlayerMovement>().interactState;
        if (state == InteractState.PickingUp || state == InteractState.Holding)
        {
            return;
        }

        // Deselect current selection
        if (selectedObj != null)
        {
            selectedObj.GetComponent<InteractiveObj>().Deselected();
        }

        // Set new selected object
        selectedObj = newSelection;

        // Select new selection
        if (selectedObj != null)
        {
            selectedObj.GetComponent<InteractiveObj>().Selected();
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
}
