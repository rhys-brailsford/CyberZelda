using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 1;

    public CharacterCollision collisionScript;

    private Direction currentDirection;
    private int[] inputAge = { 0, 0, 0, 0 };
    private float[] inputs = { 0, 0 };
    private float scaledSpeed;
    private Vector3 movementVec;
    private bool moveable = true;

    // Start is called before the first frame update
    void Start()
    {
        scaledSpeed = speed / 10;
        currentDirection = Direction.South;
        movementVec = new Vector3(0, 0, 0);
    }

    void FixedUpdate()
    {
        //Debug.Log("fixed update");
    }

    public void DisableMovement()
    {
        moveable = false;
    }
    public void EnableMovement()
    {
        moveable = true;
    }

    Vector3 CalculateMovementVec()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        Vector3 result = Vector3.Normalize(new Vector3(inputX, 0, inputZ));
        return result;
    }

    Vector3 CalculateNewPosition(Vector3 movementVec)
    {
        float curX = transform.position.x;
        float curZ = transform.position.z;

        float newX = curX + (movementVec.x * scaledSpeed) * Time.deltaTime;
        float newZ = curZ + (movementVec.z * scaledSpeed) * Time.deltaTime;
        Vector3 newpositionVec = new Vector3(newX, transform.position.y, newZ);

        return newpositionVec;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveable)
        {
            movementVec = CalculateMovementVec();
            movementVec = collisionScript.CalculateAdjustedMovement(movementVec);

            gameObject.transform.position = CalculateNewPosition(movementVec);
            currentDirection = CalculateDirection();
            RotateToCurrentDirection();
        }
    }

    void UpdateInputAge()
    {
        float horiIn = Input.GetAxisRaw("Horizontal");
        float vertIn = Input.GetAxisRaw("Vertical");

        // Increase East count 
        if (horiIn > 0)
        {
            inputAge[1]++;
        }
        // Increase West count
        else if (horiIn < 0)
        {
            inputAge[3]++;
        }
        // Reset East and West counts
        else
        {
            inputAge[1] = 0;
            inputAge[3] = 0;
        }

        // Increase North count
        if (vertIn > 0)
        {
            inputAge[0]++;
        }
        // Increase South count
        else if (vertIn < 0)
        {
            inputAge[2]++;
        }
        // Reset North and South counts
        else
        {
            inputAge[0] = 0;
            inputAge[2] = 0;
        }

    }

    Direction CalculateDirection()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        UpdateInputAge();

        int maxAge = inputAge.Max();

        if (maxAge <= 0)
        {
            return currentDirection;
        }

        int index = inputAge.ToList().IndexOf(maxAge);

        switch(index)
        {
            case (0):
                return Direction.North;
            case (1):
                return Direction.East;
            case (2):
                return Direction.South;
            case (3):
                return Direction.West;
            default:
                return currentDirection;
        }
    }

    void RotateToCurrentDirection()
    {
        // North: 0,0,0
        // South: 0,180,0
        // East: 0,90,0
        // West: 0,-90,0
        Vector3 newRotation = new Vector3(0, 0, 0);
        switch(currentDirection)
        {
            case (Direction.North):
                newRotation = new Vector3(0, 0, 0);
                break;
            case (Direction.East):
                newRotation = new Vector3(0, 90, 0);
                break;
            case (Direction.South):
                newRotation = new Vector3(0, 180, 0);
                break;
            case (Direction.West):
                newRotation = new Vector3(0, -90, 0);
                break;
            default:
                Debug.Log("Invalid Direction used.");
                break;
        }

        gameObject.transform.rotation = Quaternion.Euler(newRotation);
    }

    
    void OnCollisionEnter()
    {
        //Debug.Log("~~~~~~collision");
    }
    

}
