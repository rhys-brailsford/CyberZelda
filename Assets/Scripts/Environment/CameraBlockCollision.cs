using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBlockCollision : MonoBehaviour
{
    public bool overrideDirection = false;
    public BlockDirection blockDir;

    [SerializeField]
    private GameObject playerObj;

    // Start is called before the first frame update
    void Start()
    {
        //if (!overrideDirection) {
        //    Collider collider = GetComponent<Collider>();
        //    Vector3 boundsSize = collider.bounds.size;

        //    blockDir = boundsSize.x < boundsSize.z ? BlockDirection.X : BlockDirection.Z;
        //}

        playerObj = GameManager.GM.GetPlayer();
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Assert(playerObj != null, "PlayerObj cannot be null on " + gameObject.name + ", entered by " + collider.name);
        if (playerObj == null) return;

        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.PlayerTrigger))
        {
            // Offset is the difference between where player IS and edge of blocking bounds
            //Vector3 offset;

            //Bounds blockingBounds = gameObject.GetComponent<Collider>().bounds;
            //Vector3 edge = blockingBounds.center - blockingBounds.extents;

            //Bounds playerTriggerBounds = playerObj.GetComponent<PlayerCameraControl>().triggerBoundsObj.GetComponent<Collider>().bounds;

            //float playerSizePadding = playerTriggerBounds.extents.z;

            //// Only do Z for now
            //float offsetZ = playerObj.transform.position.z - edge.z + playerSizePadding;
            //Debug.Log("offsetZ:" + offsetZ);
            //offset = new Vector3(0, 0, offsetZ);



            playerObj.GetComponent<PlayerCameraControl>().SwitchBlockFlag(blockDir, true);
            //playerObj.GetComponent<PlayerCameraControl>().OffsetCam(offset);
            Debug.Log(collider.name);
            Debug.Log("CamBlocking entered");
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        Debug.Assert(playerObj != null, "PlayerObj cannot be null on " + gameObject.name);

        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.PlayerTrigger))
        {
            playerObj.GetComponent<PlayerCameraControl>().SwitchBlockFlag(blockDir, false);
            Debug.Log(collider.name);
            Debug.Log("CamBlocking entered");
        }
    }
}
