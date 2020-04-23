using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton Game manager class.
// Doesn't get destroyed on loading of new level.
public class GameManager : MonoBehaviour
{
    // Singleton ItemList variable
    public static GameManager GM;

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject cameraPrefab;

    private GameObject playerObj;
    private GameObject cameraObj;

    void Awake()
    {
        if (GM != null)
        {
            Debug.LogError("Only one ItemList allowed.");
            Destroy(GM);
        }
        else
        {
            GM = this;
        }

        DontDestroyOnLoad(this);

        // If prefabs are not set, use default values
        if (playerPrefab == null)
        {
            playerPrefab = (GameObject)Resources.Load("PF_Player");
        }
        if (cameraPrefab == null)
        {
            cameraPrefab = (GameObject)Resources.Load("PF_DefaultCamera");
        }

        // Create player and camera object
        playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        cameraObj = Instantiate(cameraPrefab, cameraPrefab.transform.position, cameraPrefab.transform.rotation);
    }


    public GameObject GetPlayer()
    {
        return playerObj;
    }
    public GameObject GetCamera()
    {
        return cameraObj;
    }
}
