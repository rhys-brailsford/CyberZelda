using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attached to trigger volume that defines an exit/entrance to a level
public class LevelExit : MonoBehaviour
{
    // Level to load when entering this exit
    public LevelName levelNameToLoad;
    // Index of spawn point in next level
    public int nextSpawnIndex;

    // Position when spawning in from this exit
    [HideInInspector]
    public Vector3 spawnPointPosition { get; private set; }
    public Direction spawnDirection;
    public GameObject spawnPositionObj;
    public GameObject[] cameraBlockingVolumes;

    private void Awake()
    {
        spawnPointPosition = spawnPositionObj.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // :TODO:
        // Check if collider is player
        
        GameManager.GM.GetLevelManager().LoadLevel(levelNameToLoad, nextSpawnIndex);
    }

    public Vector3 GetSpawnPoint()
    {
        return spawnPositionObj.transform.position;
    }

    public Direction GetSpawnDirection()
    {
        return spawnDirection;
    }
}
