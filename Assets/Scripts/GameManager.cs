using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton Game manager class.
// Doesn't get destroyed on loading of new level.
public class GameManager : MonoBehaviour
{
    // Singleton ItemList variable
    public static GameManager GM;

    public GameObject playerObj;

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
    }
    
    public GameObject GetPlayer()
    {
        return playerObj;
    }
}
