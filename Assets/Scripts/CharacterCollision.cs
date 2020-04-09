using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollision : MonoBehaviour
{
    public List<GameObject> overlappedObjs;

    private void Start()
    {
        overlappedObjs = new List<GameObject>();
    }

    void OnTriggerEnter(Collider collider)
    {
        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.Blocking))
        {
            overlappedObjs.Add(collider.gameObject);
            //Debug.Log("Blocking entered");
        }
    }
    

    void OnTriggerExit(Collider collider)
    {
        List<Tags> tags = collider.GetComponent<CustomTags>().tags;
        if (tags.Contains(Tags.Blocking))
        {
            overlappedObjs.Remove(collider.gameObject);
            //Debug.Log("Blocking exited");
        }
    }

    void FixedUpdate()
    {
        //CleanupNorms();
    }

    public void CleanupNorms()
    {
        overlappedObjs.RemoveAll(item => item == null);
        overlappedObjs.RemoveAll(item => item.GetComponent<Collision>().collide == false);
    }

    public Vector3 CalculateAdjustedMovement(Vector3 movementVec)
    {
        CleanupNorms();
        foreach (GameObject obj in overlappedObjs)
        {
            //if (!obj.GetComponent<Collision>().collide)
            //{
            //    continue;
            //}

            // Check if cross is needed
            // Hit normal impact DOT normalised inputVec
            // if dot > -1 => continue
            Vector3 moveNorm = Vector3.Normalize(movementVec);
            Vector3 norm = obj.GetComponent<Collision>().Normal(transform.position);
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
