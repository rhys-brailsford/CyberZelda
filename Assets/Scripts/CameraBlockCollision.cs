using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBlockCollision : MonoBehaviour
{
    public bool overrideDirection = false;
    public BlockDirection blockDir;

    // Start is called before the first frame update
    void Start()
    {
        if (!overrideDirection) {
            Collider collider = GetComponent<Collider>();
            Vector3 boundsSize = collider.bounds.size;

            blockDir = boundsSize.x < boundsSize.z ? BlockDirection.X : BlockDirection.Z;
        }
    }
}
