using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionObj : Collision
{
    public float cornerThresholdX = 4;
    public float cornerThresholdZ = 4;
    public float cornerFactor = 8;
    // Normals of 4 walls in order North, East, South, West
    public Vector3[] norms;

    public Vector2[] angleVals;

    private float width;
    private float length;

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


        norms = new Vector3[8];
        angleVals = new Vector2[8];
        norms[0] = new Vector3(0, 0, 1);    // North
        norms[1] = new Vector3(1, 0, 0);    // East
        norms[2] = new Vector3(0, 0, -1);   // South
        norms[3] = new Vector3(-1, 0, 0);   // West

        norms[4] = Vector3.Normalize(new Vector3(1, 0, 1));
        norms[5] = Vector3.Normalize(new Vector3(1, 0, -1));
        norms[6] = Vector3.Normalize(new Vector3(-1, 0, -1));
        norms[7] = Vector3.Normalize(new Vector3(-1, 0, 1));

        

        width = gameObject.GetComponent<Collider>().bounds.size.x;
        length = gameObject.GetComponent<Collider>().bounds.size.z;

        float northAngle = Mathf.Rad2Deg * Mathf.Atan(width / length);
        float eastAngle = Mathf.Rad2Deg * Mathf.Atan(length / width);

        cornerFactor = (Mod(Mathf.FloorToInt(yRot), 45)) / 45;

        angleVals[0].x = -northAngle;
        angleVals[0].y = northAngle;

        angleVals[1].x = 90 - eastAngle;
        angleVals[1].y = 90 + eastAngle;

        angleVals[2].x = 180 - northAngle;
        angleVals[2].y = 180 + northAngle;

        angleVals[3].x = 270 - eastAngle;
        angleVals[3].y = 270 + eastAngle;


        angleVals[4].x = angleVals[0].y - (cornerFactor * width / length);
        angleVals[4].y = angleVals[0].y + (cornerFactor * length / width);

        angleVals[5].x = angleVals[1].y - (cornerFactor * length / width);
        angleVals[5].y = angleVals[1].y + (cornerFactor * width / length);

        angleVals[6].x = angleVals[2].y - (cornerFactor * width / length);
        angleVals[6].y = angleVals[2].y + (cornerFactor * length / width);

        angleVals[7].x = angleVals[3].y - (cornerFactor * length / width);
        angleVals[7].y = angleVals[3].y + (cornerFactor * width / length);

        for (int i=0; i<8; i++)
        {
            angleVals[i].x += yRot;
            angleVals[i].y += yRot;
            norms[i] = Vector3.Normalize(Quaternion.Euler(0, yRot, 0) * norms[i]);
        }

        gameObject.transform.rotation = Quaternion.Euler(origRotation);
    }

    public override Vector3 Normal(Collider srcCol)
    {
        //if (square)
        //{
        //    Vector3 directionToSrc = srcCol.transform.position - gameObject.transform.position;
        //    bool xDir = Mathf.Abs(directionToSrc.x) > Mathf.Abs(directionToSrc.z);

        //    Vector3 result = new Vector3();

        //    result.x = xDir ? Mathf.Sign(directionToSrc.x) : 0;
        //    result.y = 0;
        //    result.z = xDir ? 0 : Mathf.Sign(directionToSrc.z);

        //    return result;
        //}

        float cornerRange = Mathf.Max(srcCol.bounds.size.x, srcCol.bounds.size.z);
        //print("bounds:" + srcCol.bounds);

        // THE AMOUNT OF THESHOLD YOU WANT TO CHECK FOR IS PROPORTIONAL TO THE SIZE OF THE ANGLE/SIDE IT'S ON

        //cornerThresholdZ = cornerRange * (length / width);
        //cornerThresholdX = cornerRange * (width / length);


        //print("thX:" + cornerThresholdX + ", thZ:" + cornerThresholdZ + "range:" + cornerRange);


        Vector3 closestCornerOnSrc = srcCol.ClosestPointOnBounds(gameObject.transform.position);

        Vector3 tempA = closestCornerOnSrc;
        tempA.y = 0;
        Vector3 tempB = gameObject.GetComponent<Collider>().bounds.center;
        tempB.y = 0;

        Vector3 diff = tempA - tempB;

        //return Vector3.Normalize(diff);

        // take dot product of srcCorner and objCenter
        float angle = Vector3.SignedAngle(new Vector3(0,0,1), Vector3.Normalize(diff), new Vector3(0,1,0));

        if (angle < 0)
        {
            angle = 360 + angle;
        }
        //print(angle);

        //for (int i=0; i<4; i++)
        //{
        //    if (angle < (angleVals[i].y+cornerThresholdZ) && angle > (angleVals[i].y-cornerThresholdX))
        //    {
        //        // find midpoint between norms[i] and norms[i+1]
        //        Vector3 mid = Vector3.Normalize(norms[i] + norms[(i + 1) % 4]);
        //        print("~~~~~~~ CORNER:" + mid);
        //        return mid;
        //    }
        //}

        int index = 0;
        for (int j=0; j<8; j++)
        {
            int i = (4 + j) % 8;
            //if (i==2)
            //{
            //    if ( (angle < angleVals[i].x && angle >= -180) || (angle > angleVals[i].y && angle <= 180) )
            //    {
            //        index = i;
            //        break;
            //    }
            //}
            if (angle >= angleVals[i].x && angle <= angleVals[i].y)
            {
                index = i;
                break;
            }
            else if ((angle+360) >= angleVals[i].x && (angle+360) <= angleVals[i].y)
            {
                index = i;
                break;
            }
        }
        Vector3 ret = norms[index];

        if (index >= 4)
            print(index);

        //if (gameObject.name == "longboi")
        //{
        //    print("test");
        //}

        //float curAngleRange = (angleVals[index].y - angleVals[index].x);
        //float prevAngleRange = (angleVals[Mod(index-1,4)].y - angleVals[Mod(index - 1, 4)].x);
        //cornerThresholdX = cornerRange * (curAngleRange / prevAngleRange);
        //cornerThresholdZ = cornerRange * (prevAngleRange / curAngleRange);


        //// Min angle + threshold, east corner needs more X
        //if (angle < (angleVals[index].x+cornerThresholdZ))
        //{
        //    ret = Vector3.Normalize(norms[index] + norms[Mod(index - 1,4)]);
        //}
        //// Max angle + threshold
        //if (angle > (angleVals[index].y-cornerThresholdX))
        //{
        //    ret = Vector3.Normalize(norms[index] + norms[Mod(index + 1, 4)]);
        //}


        return ret;
    }

    private int Mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }
}
