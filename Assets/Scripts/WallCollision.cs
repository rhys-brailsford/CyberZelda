using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    private Vector3 norm3d;
    public Vector3 normal;

    // Start is called before the first frame update
    void Start()
    {
        norm3d = Vector3.Normalize(new Vector3(normal.x, 0, normal.z));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 Normal()
    {
        return norm3d;
    }
}
