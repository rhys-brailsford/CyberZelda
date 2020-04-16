using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incline : MonoBehaviour
{
    private float incline = 1;
    public float inclineHeight = 0;
    private float playerHeight;
    public GameObject player;
    public Direction inclineDirection = Direction.North;

    // Start is called before the first frame update
    void Start()
    {
        // Calculate inclineFactor

        playerHeight = player.GetComponent<Collider>().bounds.size.y;
        Collider collider = GetComponent<Collider>();


        // Rotate back to local space, use bounds and rotate back to world space
        Vector3 origRotation = gameObject.transform.rotation.eulerAngles;
        Debug.Assert(origRotation.x == 0 && origRotation.z == 0, gameObject.name + ": Invalid X or Z rotations, must be zero.");

        float yRot = origRotation.y;
        gameObject.transform.Rotate(0, -yRot, 0);

        // Recalculate bounds
        Physics.SyncTransforms();

        // Elevation of incline
        if (inclineHeight == 0)
        {
            inclineHeight = collider.bounds.size.y - playerHeight;
        }


        // :TODO: This is returning inaccurate values due to floating point precision
        // e.g. if you manually rotate an object (with 15deg snapping) to 90deg, the result is 90.00001 which results in 89?
        int yRotRounded = Mathf.RoundToInt(Mathf.Repeat(yRot, 360));

        switch (yRotRounded)
        {
            case (0):
                inclineDirection = Direction.North;
                break;
            case (90):
                inclineDirection = Direction.East;
                break;
            case (180):
                inclineDirection = Direction.South;
                break;
            case (270):
                inclineDirection = Direction.West;
                break;
            default:
                Debug.LogError(gameObject.name + ": Invalid Y rotation (" + yRot + "), must be in 90deg increments.");
                break;
        }

        // Depth bounds is length of bounds in direction of incline.
        // e.g. if moving in X elevates player, then depthBounds is size of X bounds
        float depthBounds;
        switch (inclineDirection)
        {
            case (Direction.North):
                depthBounds = collider.bounds.size.z;
                break;
            case (Direction.East):
                depthBounds = collider.bounds.size.x;
                break;
            case (Direction.South):
                depthBounds = collider.bounds.size.z;
                break;
            case (Direction.West):
                depthBounds = collider.bounds.size.x;
                break;
            default:
                Debug.LogError("Invalid incline direction");
                depthBounds = collider.bounds.size.z;
                break;
        }
        incline = inclineHeight / depthBounds;

        // Reset back to world space
        gameObject.transform.rotation = Quaternion.Euler(origRotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        List<Tags> tags = collider.GetComponent<CustomTags>().tags;

        if (tags.Contains(Tags.PlayerHitbox))
        {
            collider.GetComponent<PlayerMovement>().StartIncline(inclineDirection, incline);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        List<Tags> tags = collider.GetComponent<CustomTags>().tags;

        if (tags.Contains(Tags.PlayerHitbox))
        {
            collider.GetComponent<PlayerMovement>().EndIncline();
        }
    }
}
