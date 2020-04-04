using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraControl : MonoBehaviour
{
    // :TODO:
    // Add tag for camera blockers
    // Do actual blocking
    // Make fixed/on rail camera option
    // Set up version control

    public GameObject mainCamera;
    public float baseHeight;

    private bool blockX = false;
    private bool blockZ = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float playerX = gameObject.transform.position.x;
        float playerZ = gameObject.transform.position.z;
        float camX = mainCamera.transform.position.x;
        float camZ = mainCamera.transform.position.z;

        Vector3 newPosition = new Vector3(playerX, baseHeight, playerZ);

        if (blockX)
        {
            newPosition.x = camX;
        }
        if (blockZ)
        {
            newPosition.z = camZ;
        }
        mainCamera.transform.position = newPosition;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag != "CamBlocking")
        {
            return;
        }
        BlockDirection blockDir = collider.GetComponent<CameraBlockCollision>().blockDir;

        SwitchBlockFlag(blockDir);
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag != "CamBlocking")
        {
            return;
        }
        BlockDirection blockDir = collider.GetComponent<CameraBlockCollision>().blockDir;

        SwitchBlockFlag(blockDir);
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
