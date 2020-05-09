using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //public int CurSpawnIndex { get; private set; }
    public int curSpawnIndex = 10;

    public void LoadLevel(LevelName levelName, int spawnIndex)
    {
        curSpawnIndex = spawnIndex;

        // Load level
        SceneManager.LoadScene(levelName.ToString());
    }


}
