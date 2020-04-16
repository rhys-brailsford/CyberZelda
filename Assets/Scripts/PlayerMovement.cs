using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PlayerMovement : MonoBehaviour
{
    public float speed = 1;

    public CharacterCollision collisionScript;

    private Rigidbody rb;

    public Direction currentDirection;
    public int[] inputAge = { 0, 0, 0, 0 };
    private float[] inputs = { 0, 0 };
    private float scaledSpeed;
    private float speedToUse;
    private Vector3 movementVec;
    private bool inputMoveable = true;
    private bool moveable = true;

    // Knockback variables
    public float knockbackDuration = 0.3f;
    public float knockbackStrength = 25;
    private float curKnockbackRemaining = 0;
    private Vector3 knockbackVec;

    private GameObject grabbedObj = null;
    public bool isGrabbingObj = false;

    private GameObject pickedUpObj = null;
    private float curPickingUpDuration = 0;
    public float pickingUpDuration = 1;
    private Vector3 offsetRotation;
    private Vector3 objSrcPosition;
    private Vector3 objDestPosition;

    public float throwStrength = 10;
    public float throwDur = 1;
    private float curThrowDur = 0;
    private Vector3 throwVec;
    private float floorCoordinate = 1.6f;

    public InteractState interactState = InteractState.Idle;
    public MovementState movState = MovementState.Idle;

    private bool incline = false;
    private float inclineFactor;
    private Direction inclineDirection;
    private float inclineStartCoord;
    private float inclineStartY;

    // Start is called before the first frame update
    void Start()
    {
        scaledSpeed = speed / 10;
        //scaledSpeed = speed;
        currentDirection = Direction.South;
        movementVec = new Vector3(0, 0, 0);
        rb = gameObject.GetComponent<Rigidbody>();
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

        switch (index)
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
        switch (currentDirection)
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

    Vector3 CalculateMovementVec()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        Vector3 result = Vector3.Normalize(new Vector3(inputX, 0, inputZ));

        return result;
    }

    //Vector3 CalculateNewPosition(Vector3 movementVec, float scale)
    //{
    //    float curX = transform.position.x;
    //    float curZ = transform.position.z;

    //    float newX = curX + (movementVec.x * scale) * Time.deltaTime;
    //    float newZ = curZ + (movementVec.z * scale) * Time.deltaTime;

    //    Vector3 newpositionVec = new Vector3(newX, transform.position.y, newZ);

    //    return newpositionVec;
    //}

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

        curKnockbackRemaining = knockbackDuration;

        movState = MovementState.KnockedBack;

    }
    private void KnockbackUpdate()
    {
        if (curKnockbackRemaining > 0)
        {
            // Apply some knockback movement
            movementVec = knockbackVec;
            speedToUse = knockbackStrength;

            curKnockbackRemaining -= Time.deltaTime;
        }
        else
        {
            EnableInputMovement();
            movState = MovementState.Idle;
        }
    }

    public void Grab(GameObject obj)
    {
        grabbedObj = obj;
        DisableInputMovement();

        interactState = InteractState.Grabbing;
    }
    public void Ungrab()
    {
        grabbedObj = null;
        EnableInputMovement();

        interactState = InteractState.Idle;
    }

    public void PickupObject(GameObject obj)
    {
        // Maybe add argument for picked up item
        curPickingUpDuration = 0;
        DisableInputMovement();
        pickedUpObj = obj;
        objSrcPosition = pickedUpObj.transform.position;
        objDestPosition = gameObject.transform.position;
        objDestPosition.y += 4.5f;

        // Used to rotate object relative to players rotation when picked up
        offsetRotation = gameObject.transform.rotation.eulerAngles;

        interactState = InteractState.PickingUp;
    }
    private void PickingUpObjUpdate()
    {
        // Animation finished, swap to "holdingObj" state
        if (curPickingUpDuration > pickingUpDuration)
        {
            interactState = InteractState.Holding;
            EnableInputMovement();
            return;
        }

        pickedUpObj.transform.position = Vector3.Lerp(objSrcPosition, objDestPosition, curPickingUpDuration / pickingUpDuration);

        curPickingUpDuration += Time.deltaTime;
    }
    private void HoldingObjUpdate()
    {
        Vector3 temp = pickedUpObj.transform.position;
        temp.x = gameObject.transform.position.x;
        temp.z = gameObject.transform.position.z;
        pickedUpObj.transform.position = temp;

        // Rotate object given initial offset based on player rotation
        pickedUpObj.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles - offsetRotation);
    }

    public void ThrowObj()
    {
        DisableInputMovement();
        curThrowDur = 0;

        interactState = InteractState.Throwing;
    }
    private void ThrowObjUpdate()
    {
        if (curThrowDur > throwDur)
        {
            Vector3 dest;
            // Calculate destination of thrown object
            switch (currentDirection)
            {
                case (Direction.East):
                    dest = new Vector3(1, 0, 0);
                    break;
                case (Direction.North):
                    dest = new Vector3(0, 0, 1);
                    break;
                case (Direction.South):
                    dest = new Vector3(0, 0, -1);
                    break;
                case (Direction.West):
                    dest = new Vector3(-1, 0, 0);
                    break;
                default:
                    dest = new Vector3(0, 0, 1);
                    break;
            }
            dest = dest * throwStrength;
            dest = pickedUpObj.transform.position + dest;
            dest.y = floorCoordinate;
            pickedUpObj.GetComponent<HoldableObj>().Throw(dest);

            // :Care:
            // Ungrab was not initially designed to handle thrown objects, this could have negative side effects
            Ungrab();

            interactState = InteractState.Idle;
            return;
        }
        curThrowDur += Time.deltaTime;
    }

    public void StartIncline(Direction dir, float factor)
    {
        inclineStartY = transform.position.y;
        inclineFactor = factor;
        inclineDirection = dir;

        switch (inclineDirection)
        {
            case (Direction.North):
                inclineStartCoord = transform.position.z;
                break;
            case (Direction.East):
                inclineStartCoord = transform.position.x;
                break;
            case (Direction.South):
                inclineStartCoord = transform.position.z;
                break;
            case (Direction.West):
                inclineStartCoord = transform.position.x;
                break;
            default:
                Debug.Log("unsupported incline direction, error");
                inclineStartCoord = transform.position.z;
                break;
        }

        incline = true;
    }
    public void EndIncline()
    {
        incline = false;
    }
    private void UpdateIncline()
    {
        if (!incline)
        {
            return;
        }
        float curCoordinate;
        switch (inclineDirection)
        {
            case (Direction.North):
                curCoordinate = movementVec.z;
                break;
            case (Direction.East):
                curCoordinate = movementVec.x;
                break;
            case (Direction.South):
                curCoordinate = -movementVec.z;
                break;
            case (Direction.West):
                curCoordinate = -movementVec.x;
                break;
            default:
                Debug.Log("unsupported incline direction, error");
                curCoordinate = movementVec.z;
                break;
        }
        movementVec.y = curCoordinate * inclineFactor;
        movementVec = Vector3.Normalize(movementVec);

        //float yOffset = (curCoordinate - inclineStartCoord) * inclineFactor;
        //if (inclineDirection == Direction.South || inclineDirection == Direction.West)
        //{
        //    yOffset *= -1;
        //}

        //float y = (inclineStartY + yOffset);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputAge();

        // Increase speed, for testing
        if (Input.GetKey(KeyCode.LeftShift))
        {
            scaledSpeed = speed / 5;
        }
        else
        {
            scaledSpeed = speed / 10;
        }

        // Handle movement first
        if (inputMoveable)
        {
            movementVec = CalculateMovementVec();
            if (incline)
            {
                UpdateIncline();
            }
            
            speedToUse = scaledSpeed;

            currentDirection = CalculateDirection();
            RotateToCurrentDirection();
        }
        else
        {
            movementVec = new Vector3(0, 0, 0);
        }
        if (movState == MovementState.KnockedBack)
        {
            KnockbackUpdate();
        }
        
        // Handle interact behaviours
        if (interactState == InteractState.Holding)
        {
            HoldingObjUpdate();
        }

        if (interactState == InteractState.PickingUp)
        {
            PickingUpObjUpdate();
        }

        if (interactState == InteractState.Throwing)
        {
            ThrowObjUpdate();
        }

        
        else
        {
            // :TODO:
            // Maybe put in someway to prevent player from buffering inputs while being knocked back.
            // At the moment, the instant the knockback animation is done, inputs are being read and immediately being translated into movement. 
            // Could use a delay, or force player to give new inputs.
            //EnableInputMovement();
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + (movementVec * speedToUse * Time.fixedDeltaTime));
    }
}
