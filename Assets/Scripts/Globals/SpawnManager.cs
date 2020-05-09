using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public LevelExit[] exits;
    // Half the size of the player in x/z.
    public float playerSize = 0.5f;

    private void Awake()
    {
        // May need to wait for LevelManager to finish loading level before spawning player

        int spawnIndex = GameManager.GM.GetLevelManager().curSpawnIndex;
        LevelExit curExit = exits[spawnIndex];

        Vector3 spawnPosition = curExit.GetSpawnPoint();
        Direction playerDir = curExit.GetSpawnDirection();

        BlockDirection blockDir = BlockDirection.Null;
        GameObject[] camBlockingVolumes = curExit.cameraBlockingVolumes;
        Vector3 camOffset = Vector3.zero;

        // Apply offset for each camera blocking volume player is spawning into
        foreach (GameObject camBlockingVolume in camBlockingVolumes)
        {
            // Calculate camera offset
            Bounds camBlockBounds = camBlockingVolume.GetComponent<Collider>().bounds;

            blockDir = camBlockingVolume.GetComponent<CameraBlockCollision>().blockDir;

            if (blockDir == BlockDirection.PosX)
            {
                Vector3 edge = camBlockBounds.center + camBlockBounds.extents;
                camOffset.x = edge.x - spawnPosition.x + playerSize;
            }
            else if (blockDir == BlockDirection.NegX)
            {
                Vector3 edge = camBlockBounds.center - camBlockBounds.extents;
                camOffset.x = edge.x - spawnPosition.x - playerSize;
            }
            else if (blockDir == BlockDirection.PosZ)
            {
                Vector3 edge = camBlockBounds.center + camBlockBounds.extents;
                camOffset.z = edge.z - spawnPosition.z + playerSize;
            }
            else if (blockDir == BlockDirection.NegZ)
            {
                Vector3 edge = camBlockBounds.center - camBlockBounds.extents;
                camOffset.z = edge.z - spawnPosition.z - playerSize;
            }
        }

        GameManager.GM.SpawnPlayer(spawnPosition, playerDir, camOffset);
    }

    private void Start()
    {
        // Assert that playerTriggerHalfSize is accurate.
        // This needs to be done after player is spawned because playerTriggerHalfSize is used in the
        // spawning of the player. This is mostly a sanity check to let the user know if the value has
        // changed.
        Vector3 playerBoundsSize = GameManager.GM.GetPlayer().GetComponent<PlayerCameraControl>().
            triggerBoundsObj.GetComponent<Collider>().bounds.extents;
        float actualHalfSize = Mathf.Max(playerBoundsSize.x, playerBoundsSize.z);
        Debug.Assert(Mathf.Approximately(actualHalfSize, playerSize), gameObject.name + 
            ": playerTriggerHalfSize not equal to actual player halfsize. " + 
            "Stored: " + playerSize + ", Actual: " + actualHalfSize);
    }
}
