using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollision : MonoBehaviour
{
    private List<Vector3> overlappedNorms;

    private void Start()
    {
        overlappedNorms = new List<Vector3>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Blocking")
        {
            Vector3 wallNormal = collider.GetComponent<WallCollision>().Normal();
            overlappedNorms.Add(wallNormal);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Blocking")
        {
            Vector3 wallNormal = collider.GetComponent<WallCollision>().Normal();
            overlappedNorms.Remove(wallNormal);
        }
    }

    public Vector3 CalculateAdjustedMovement(Vector3 movementVec)
    {
        foreach (Vector3 norm in overlappedNorms)
        {
            // Check if cross is needed
            // Hit normal impact DOT normalised inputVec
            // if dot > -1 => continue
            Vector3 moveNorm = Vector3.Normalize(movementVec);
            float dot = Vector3.Dot(norm, moveNorm);
            if (dot <= -1)
            {
                // Wall norm is positive x, block -x movement
                if (norm.x > 0)
                {
                    movementVec.x = Mathf.Clamp(movementVec.x, 0, 1);
                }
                // Wall norm is negative x, block +x movement
                else if (norm.x < 0)
                {
                    movementVec.x = Mathf.Clamp(movementVec.x, -1, 0);
                }
                // Wall norm is positive z, block -z movement
                if (norm.z > 0)
                {
                    movementVec.z = Mathf.Clamp(movementVec.z, 0, 1);
                }
                // Wall norm is negative z, block +z movement
                else if (norm.z < 0)
                {
                    movementVec.z = Mathf.Clamp(movementVec.z, -1, 0);
                }
                continue;
            }
            else
            {
                if (Vector3.Dot(movementVec, norm) < 0)
                {
                    Vector3 proj = Vector3.Project(moveNorm, norm);
                    Vector3 orth = movementVec - proj;
                    movementVec = orth;
                }
            }
        }

        return movementVec;
    }
}
