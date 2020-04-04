using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float baseHeight;
    public GameObject target = null;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(target != null, "Target cannot be empty!");
    }

    // Update is called once per frame
    void Update()
    {
        float x = target.gameObject.transform.position.x;
        float z = target.gameObject.transform.position.z;
        Vector3 newPosition = new Vector3(x, baseHeight, z);
        gameObject.transform.position = newPosition;
    }
}
