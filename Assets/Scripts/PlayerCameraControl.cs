using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraControl : MonoBehaviour
{
    public GameObject mainCamera;
    public float heightDiff;
    public GameObject triggerBoundsObj;

    public bool blockX = false;
    public bool blockZ = false;

    public float offsetZ = 0;

    private void Start()
    {
        mainCamera = GameManager.GM.GetCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = GameManager.GM.GetCamera();
        }

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

        if (!blockX && !blockZ)
        {
            mainCamera.transform.LookAt(gameObject.transform, new Vector3(0, 0, 1));
        }
    }

    public void OffsetCam(Vector3 offset)
    {
        mainCamera.transform.position += offset;
    }

    public void SwitchBlockFlag(BlockDirection direction, bool newValue)
    {
        switch (direction)
        {
            case (BlockDirection.PosX):
                blockX = newValue;
                break;
            case (BlockDirection.NegX):
                blockX = newValue;
                break;
            case (BlockDirection.PosZ):
                blockZ = newValue;
                break;
            case (BlockDirection.NegZ):
                blockZ = newValue;
                break;
            default:
                break;
        }
    }
}
