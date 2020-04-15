using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : Collision
{
    private Vector3 norm3d;
    public Vector3 normal;
    public bool wall = true;

    // Start is called before the first frame update
    // :TODO:
    // Calculate norm3d based on an origin of (0,0,1) and applying local rotation
    void Start()
    {
        norm3d = Vector3.Normalize(new Vector3(normal.x, 0, normal.z));
    }


    public override Vector3 Normal(Collider srcCol)
    {
        return norm3d;
    }

    //public Vector3 BlockingNormal(Vector3 srcPosition)
    //{
    //    Vector3 directionToSrc = srcPosition - transform.position;
    //    Debug.Log("direction: " + directionToSrc);
    //    bool xDir = Mathf.Abs(directionToSrc.x) > Mathf.Abs(directionToSrc.z);

    //    Vector3 result = new Vector3();

    //    result.x = xDir ? Mathf.Sign(directionToSrc.x)*1 : 0;
    //    result.y = 0;
    //    result.z = xDir ? 0 : Mathf.Sign(directionToSrc.z) * 1;

    //    return result;
    //}

}
