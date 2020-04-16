using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionObj : Collision
{
    // Normals of 4 walls in order North, East, South, West
    public Vector3[] norms;

    // Angle ranges for deciding which normal to use, x = min angle, y = max angle
    public Vector2[] angleVals;

    public bool square = false;

    private void Start()
    {
        Vector3 origRotation = gameObject.transform.rotation.eulerAngles;
        float yRot = origRotation.y;
        gameObject.transform.Rotate(0, -yRot, 0);
        // Recalculate bounds
        Physics.SyncTransforms();

        Vector3 bounds = gameObject.GetComponent<Collider>().bounds.size;

        square = Mathf.Approximately(bounds.x, bounds.z) ? true : false;

        if (square)
        {
            gameObject.transform.rotation = Quaternion.Euler(origRotation);
            return;
        }

        norms = new Vector3[4];
        angleVals = new Vector2[4];
        norms[0] = new Vector3(0, 0, 1);    // North
        norms[1] = new Vector3(1, 0, 0);    // East
        norms[2] = new Vector3(0, 0, -1);   // South
        norms[3] = new Vector3(-1, 0, 0);   // West       

        float width = gameObject.GetComponent<Collider>().bounds.size.x;
        float length = gameObject.GetComponent<Collider>().bounds.size.z;

        float northAngle = Mathf.Rad2Deg * Mathf.Atan(width / length);
        float eastAngle = Mathf.Rad2Deg * Mathf.Atan(length / width);

        angleVals[0].x = Mathf.Repeat(-northAngle+360, 360);
        angleVals[0].y = northAngle;

        angleVals[1].x = Mathf.Repeat(90 - eastAngle, 360);
        angleVals[1].y = Mathf.Repeat(90 + eastAngle, 360);

        angleVals[2].x = Mathf.Repeat(180 - northAngle, 360);
        angleVals[2].y = Mathf.Repeat(180 + northAngle, 360);

        angleVals[3].x = Mathf.Repeat(270 - eastAngle, 360);
        angleVals[3].y = Mathf.Repeat(270 + eastAngle, 360);


        for (int i=0; i<4; i++)
        {
            angleVals[i].x = Mathf.Repeat(angleVals[i].x + yRot, 360);
            angleVals[i].y = Mathf.Repeat(angleVals[i].y + yRot, 360);
            norms[i] = Vector3.Normalize(Quaternion.Euler(0, yRot, 0) * norms[i]);
        }

        gameObject.transform.rotation = Quaternion.Euler(origRotation);
    }

    public override Vector3 Normal(Collider srcCol)
    {
        if (square)
        {
            Vector3 directionToSrc = srcCol.transform.position - gameObject.transform.position;
            bool xDir = Mathf.Abs(directionToSrc.x) > Mathf.Abs(directionToSrc.z);

            Vector3 result = new Vector3();

            result.x = xDir ? Mathf.Sign(directionToSrc.x) : 0;
            result.y = 0;
            result.z = xDir ? 0 : Mathf.Sign(directionToSrc.z);

            return result;
        }

        Vector3 closestCornerOnSrc = srcCol.ClosestPointOnBounds(gameObject.transform.position);
        print(closestCornerOnSrc);

        Vector3 tempA = closestCornerOnSrc;
        tempA.y = 0;
        Vector3 tempB = gameObject.GetComponent<Collider>().bounds.center;
        tempB.y = 0;

        Vector3 diff = tempA - tempB;

        // take dot product of srcCorner and objCenter
        float angle = Vector3.SignedAngle(new Vector3(0,0,1), Vector3.Normalize(diff), new Vector3(0,1,0));

        if (angle < 0)
        {
            angle = 360 + angle;
        }

        int index = 0;
        for (int i=0; i<4; i++)
        {
            if (angle > angleVals[i].x && angle <= angleVals[i].y)
            {
                index = i;
                break;
            }

            else if (Mathf.Repeat((angle+180),360) > (Mathf.Repeat(angleVals[i].x+180,360)) &&
                     Mathf.Repeat((angle+180),360) <= (Mathf.Repeat(angleVals[i].y+180,360)) )
            {
                index = i;
                break;
            }
        }
        Vector3 ret = norms[index];

        // if angle is within some threshold of one of the angles for selected norm
        // give average of neighbouring norms
        float threshold = 5;
        if (Mathf.Abs(angleVals[index].x - angle) < threshold)
        {
            // x = minimum angle, therefore look at previous norm
            Vector3 curNorm = norms[index];
            Vector3 nextNorm = norms[Mod(index - 1,4)];
            ret = Vector3.Normalize(curNorm + nextNorm);
        }
        else if (Mathf.Abs(angleVals[index].y - angle) < threshold)
        {
            // y = max angle, therefore look at next norm
            Vector3 curNorm = norms[index];
            Vector3 nextNorm = norms[Mod(index + 1,4)];
            ret = Vector3.Normalize(curNorm + nextNorm);
        }
        
        return ret;
    }

    private int Mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }
}
