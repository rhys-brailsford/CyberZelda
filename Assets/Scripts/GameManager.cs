using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton Game manager class.
// Doesn't get destroyed on loading of new level.
public class GameManager : MonoBehaviour
{
    // Singleton ItemList variable
    public static GameManager GM;


    [SerializeField] private GameObject playerObjTest;
    [SerializeField] private GameObject cameraObjTest;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject cameraPrefab;

    [SerializeField] private Inventory inv;

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
        if (playerObjTest == null)
        {
            playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            playerObj = playerObjTest;
        }

        if (cameraObjTest == null)
        {
            cameraObj = Instantiate(cameraPrefab, cameraPrefab.transform.position, cameraPrefab.transform.rotation);
        }
        else
        {
            cameraObj = cameraObjTest;
        }
        if (inv == null)
        {
            inv = gameObject.GetComponent<Inventory>();

            if (inv == null)
            {
                Debug.LogError("Inventory not found on GameManager object");
            }
        }
    }


    public GameObject GetPlayer()
    {
        return playerObj;
    }
    public GameObject GetCamera()
    {
        return cameraObj;
    }
    public Inventory GetInventory()
    {
        return inv;
    }
}
