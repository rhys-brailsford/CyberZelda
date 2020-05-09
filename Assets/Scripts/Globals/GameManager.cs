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

    [SerializeField] private LevelManager levelManager;

    private GameObject playerObj;
    private GameObject cameraObj;

    void Awake()
    {
        if (GM != null)
        {
            Debug.LogError("Only one GameManager allowed.");
            Destroy(gameObject);
            return;
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
            playerObj = Instantiate(playerObjTest);
            Destroy(playerObjTest);
        }

        if (cameraObjTest == null)
        {
            cameraObj = Instantiate(cameraPrefab, cameraPrefab.transform.position, cameraPrefab.transform.rotation);
        }
        else
        {
            cameraObj = Instantiate(cameraObjTest);
            Destroy(cameraObjTest);
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

    public void SpawnPlayer(Vector3 position, Direction direction, Vector3 camOffset)
    {
        if (playerObj == null)
        {
            Vector3 playerDirection;
            switch (direction)
            {
                case (Direction.North):
                    playerDirection = new Vector3(0, 0, 0);
                    break;
                case (Direction.East):
                    playerDirection = new Vector3(0, 90, 0);
                    break;
                case (Direction.South):
                    playerDirection = new Vector3(0, 180, 0);
                    break;
                case (Direction.West):
                    playerDirection = new Vector3(0, 270, 0);
                    break;
                default:
                    playerDirection = new Vector3(0, 0, 0);
                    break;
            }

            // Destroy any existing player objects
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject obj in objs)
            {
                Destroy(obj);
            }

            playerObj = Instantiate(playerPrefab, position, Quaternion.Euler(playerDirection));

            // If camera is offset, flag player camera control to block movement in that axis
            if (camOffset.x != 0)
            {
                playerObj.GetComponent<PlayerCameraControl>().SwitchBlockFlag(BlockDirection.PosX, true);
            }
            if (camOffset.z != 0)
            {
                playerObj.GetComponent<PlayerCameraControl>().SwitchBlockFlag(BlockDirection.PosZ, true);
            }
        }

        if (cameraObj == null)
        {
            // Destroy any existing MainCamera objects
            GameObject[] objs = GameObject.FindGameObjectsWithTag("MainCamera");
            foreach (GameObject obj in objs)
            {
                Destroy(obj);
            }

            cameraObj = Instantiate(cameraPrefab, position+camOffset, Quaternion.Euler(90,0,0));
        }
    }

    public GameObject GetPlayerPrefab()
    {
        return playerPrefab;
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
    public LevelManager GetLevelManager()
    {
        return levelManager;
    }
}
