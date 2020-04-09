using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionObj : Collision
{
    public override Vector3 Normal(Vector3 srcPosition)
    {
        Vector3 directionToSrc = srcPosition - transform.position;
        bool xDir = Mathf.Abs(directionToSrc.x) > Mathf.Abs(directionToSrc.z);

        Vector3 result = new Vector3();

        result.x = xDir ? Mathf.Sign(directionToSrc.x) * 1 : 0;
        result.y = 0;
        result.z = xDir ? 0 : Mathf.Sign(directionToSrc.z) * 1;

        return result;
    }
}
