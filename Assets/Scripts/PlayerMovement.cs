using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PlayerMovement : MonoBehaviour
{
    public float speed = 1;

    public CharacterCollision collisionScript;

    public Direction currentDirection;
    public int[] inputAge = { 0, 0, 0, 0 };
    private float[] inputs = { 0, 0 };
    private float scaledSpeed;
    private Vector3 movementVec;
    private bool inputMoveable = true;
    private bool moveable = true;

    // Knockback variables
    public float knockbackDuration = 0.3f;
    public float knockbackStrength = 25;
    private float curKnockbackRemaining = 0;
    private Vector3 knockbackVec;

    private GameObject grabbedObj = null;

    private GameObject pickedUpObj = null;
    private bool pickingUp = false;
    private float curPickingUpDuration = 0;
    public float pickingUpDuration = 1;
    private Vector3 objSrcPosition;
    private Vector3 objDestPosition;

    private bool holdingObj = false;

    // Start is called before the first frame update
    void Start()
    {
        scaledSpeed = speed / 10;
        currentDirection = Direction.South;
        movementVec = new Vector3(0, 0, 0);

    }

    public void DisableInputMovement()
    {
        inputMoveable = false;
    }
    public void EnableInputMovement()
    {
        inputMoveable = true;
    }
    public bool InputMoveable()
    {
        return inputMoveable;
    }

    public void DisableMoveable()
    {
        moveable = false;
    }
    public void EnableMovement()
    {
        moveable = true;
    }
    public bool Moveable()
    {
        return moveable;
    }

    public void Knockback(Vector3 srcPosition)
    {
        DisableInputMovement();

        // Calculate knockback vector
        // A->B = B-A
        Vector3 curPosition = transform.position;
        Vector3 preNormalised = curPosition - srcPosition;
        preNormalised.y = 0;
        knockbackVec = Vector3.Normalize(preNormalised);
        //Debug.Log("knockback vector:" + knockbackVec);

        curKnockbackRemaining = knockbackDuration;
    }

    public void Grab(GameObject obj)
    {
        grabbedObj = obj;
        DisableInputMovement();
    }

    public void Ungrab()
    {
        grabbedObj = null;
        EnableInputMovement();
    }

    public void PickupObject(GameObject obj)
    {
        // Maybe add argument for picked up item
        pickingUp = true;
        curPickingUpDuration = 0;
        DisableInputMovement();
        pickedUpObj = obj;
        objSrcPosition = pickedUpObj.transform.position;
        objDestPosition = gameObject.transform.position;
        objDestPosition.y += 4.5f;
    }

    private void PickingUp()
    {
        if (curPickingUpDuration > pickingUpDuration)
        {
            pickingUp = false;
            holdingObj = true;
            EnableInputMovement();
            return;
        }

        pickedUpObj.transform.position = Vector3.Lerp(objSrcPosition, objDestPosition, curPickingUpDuration / pickingUpDuration);

        curPickingUpDuration += Time.deltaTime;
    }

    private void HoldingObj()
    {
        Vector3 temp = pickedUpObj.transform.position;
        temp.x = gameObject.transform.position.x;
        temp.z = gameObject.transform.position.z;
        pickedUpObj.transform.position = temp;
        // :TODO:
        // do something about rotation
    }

    Vector3 CalculateMovementVec()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        Vector3 result = Vector3.Normalize(new Vector3(inputX, 0, inputZ));
        return result;
    }

    Vector3 CalculateNewPosition(Vector3 movementVec, float scale)
    {
        float curX = transform.position.x;
        float curZ = transform.position.z;

        float newX = curX + (movementVec.x * scale) * Time.deltaTime; 
        float newZ = curZ + (movementVec.z * scale) * Time.deltaTime;
        Vector3 newpositionVec = new Vector3(newX, transform.position.y, newZ);

        return newpositionVec;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputAge();

        if (inputMoveable)
        {
            movementVec = CalculateMovementVec();
            movementVec = collisionScript.CalculateAdjustedMovement(movementVec);

            gameObject.transform.position = CalculateNewPosition(movementVec, scaledSpeed);
            currentDirection = CalculateDirection();
            RotateToCurrentDirection();
        }

        if (holdingObj)
        {
            HoldingObj();
        }

        if (pickingUp)
        {
            PickingUp();
        }

        else if (curKnockbackRemaining > 0)
        {
            movementVec = collisionScript.CalculateAdjustedMovement(knockbackVec);

            // Apply some knockback movement
            gameObject.transform.position = CalculateNewPosition(movementVec, knockbackStrength);

            curKnockbackRemaining -= Time.deltaTime;
        }
        else
        {
            // :TODO:
            // Maybe put in someway to prevent player from buffering inputs while being knocked back.
            // At the moment, the instant the knockback animation is done, inputs are being read and immediately being translated into movement. 
            // Could use a delay, or force player to give new inputs.
            EnableInputMovement();
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

        

}
