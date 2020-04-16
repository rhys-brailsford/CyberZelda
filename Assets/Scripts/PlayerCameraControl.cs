using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraControl : MonoBehaviour
{
    public GameObject mainCamera;
    public float heightDiff;

    private bool blockX = false;
    private bool blockZ = false;

    public float offsetZ = 0;

    private void Start()
    {
        //mainCamera.transform.RotateAround(gameObject.transform.position, new Vector3(1, 0, 0), rotX);
    }
    // Update is called once per frame
    void Update()
    {
        float playerX = gameObject.transform.position.x;
        float playerY = gameObject.transform.position.y;
        float playerZ = gameObject.transform.position.z;
        float camX = mainCamera.transform.position.x;
        float camZ = mainCamera.transform.position.z;

        Vector3 newPosition = new Vector3(playerX, playerY + heightDiff, playerZ-offsetZ);

        if (blockX)
        {
            newPosition.x = camX;
        }
        if (blockZ)
        {
            newPosition.z = camZ;
        }
        mainCamera.transform.position = newPosition;

        mainCamera.transform.LookAt(gameObject.transform, new Vector3(0,0,1));
    }

    void OnTriggerEnter(Collider collider)
    {
        //print(collider.name);

        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.CamBlocking))
        {
            BlockDirection blockDir = collider.GetComponent<CameraBlockCollision>().blockDir;

            SwitchBlockFlag(blockDir);

            //Debug.Log("CamBlocking entered");
        }
        
    }

    void OnTriggerExit(Collider collider)
    {
        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.CamBlocking))
        {
            BlockDirection blockDir = collider.GetComponent<CameraBlockCollision>().blockDir;

            SwitchBlockFlag(blockDir);
            //Debug.Log("CamBlocking exited");
        }


    }

    void SwitchBlockFlag(BlockDirection direction)
    {
        switch (direction)
        {
            case (BlockDirection.X):
                blockX = !blockX;
                break;
            case (BlockDirection.Z):
                blockZ = !blockZ;
                break;
            default:
                break;
        }
    }
}
