using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PlayerMovement : MonoBehaviour
{
    public float speed = 1;

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
    private Vector3 pickedUpOffsetRotation;
    private Vector3 pickedUpSrcPosition;
    private Vector3 pickedUpDestPosition;

    public float throwStrength = 10;
    public float throwDur = 1;
    private float curThrowDur = 0;
    private Vector3 throwVec;
    private float floorCoordinate = 1.6f;

    public InteractState interactState = InteractState.Idle;
    public MovementState movState = MovementState.Idle;

    public float height = 2;
    private float halfHeight;
    public float heightPadding = 0.5f;
    public float inGroundAdjustSpeed = 5;
    public LayerMask ground;
    RaycastHit hitInfo;
    bool grounded;


    public bool debug = false;



    // Start is called before the first frame update
    void Start()
    {
        scaledSpeed = speed / 10;
        movementVec = new Vector3(0, 0, 0);
        rb = gameObject.GetComponent<Rigidbody>();
        halfHeight = height / 2.0f;

        //currentDirection = Direction.South;
        float rotation = gameObject.transform.rotation.eulerAngles.y;
        int intRotation = Mathf.RoundToInt((rotation % 360)/90f)%4;

        switch (intRotation)
        {
            case (0):
                currentDirection = Direction.North;
                break;
            case (1):
                currentDirection = Direction.East;
                break;
            case (2):
                currentDirection = Direction.South;
                break;
            case (3):
                currentDirection = Direction.West;
                break;
            default:
                currentDirection = Direction.North;
                break;
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

    // Get direction of player in degrees relative to north (Z-forward)
    public float GetDirectionDeg()
    {
        switch(currentDirection)
        {
            case (Direction.North):
                return 0;
            case (Direction.East):
                return 90;
            case (Direction.South):
                return 180;
            case (Direction.West):
                return 270;
            default:
                return 0;
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
        if (movState != MovementState.KnockedBack)
        {
            return;
        }

        if (curKnockbackRemaining > 0)
        {
            // Apply some knockback movement
            movementVec = knockbackVec;
            speedToUse = knockbackStrength;

            curKnockbackRemaining -= Time.fixedDeltaTime;
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
        pickedUpSrcPosition = pickedUpObj.transform.position;
        pickedUpDestPosition = gameObject.transform.position;
        //objDestPosition.y += 4.5f;
        pickedUpDestPosition.y += height + heightPadding;

        // Used to rotate object relative to players rotation when picked up
        pickedUpOffsetRotation = gameObject.transform.rotation.eulerAngles;

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

        pickedUpObj.transform.position = Vector3.Lerp(pickedUpSrcPosition, pickedUpDestPosition, curPickingUpDuration / pickingUpDuration);

        curPickingUpDuration += Time.deltaTime;
    }
    private void HoldingObjUpdate()
    {
        Vector3 temp = pickedUpObj.transform.position;
        temp.x = gameObject.transform.position.x;
        temp.z = gameObject.transform.position.z;
        pickedUpObj.transform.position = temp;

        // Rotate object given initial offset based on player rotation
        pickedUpObj.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles - pickedUpOffsetRotation);
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

    public void UseEquipped()
    {
        Inventory inv = PlayerStats.PS.Inventory();
        if (inv.equippedItem == ItemName.Undefined)
        {
            Debug.Log("Attempting to use item when nothing is equipped");
            return;
        }

        Debug.Log("UseEquipped called!");
        InventoryItem equippedItem = (InventoryItem)ItemList.IL.GetItem(inv.equippedItem);
        equippedItem.UseItem();
    }

    void CheckGround()
    {
        bool hitSuccess = Physics.Raycast(transform.position,       // src position
                                          -Vector3.up,              // raycast direction
                                          out hitInfo,              // output info
                                          halfHeight + heightPadding,   // max distance
                                          ground);                  // layer
        if (hitSuccess)
        {
            if (hitInfo.distance < halfHeight)
            {
                transform.position = Vector3.Lerp(transform.position,
                                                  transform.position + Vector3.up * halfHeight,
                                                  inGroundAdjustSpeed * Time.fixedDeltaTime);
            }
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }
    void ApplyGravity()
    {
        if (!grounded)
        {
            // Different alternatives for applying gravity.
            //movementVec = Vector3.down;
            //movementVec += Vector3.down;
            // Works well with gravity ~-2.5y
            movementVec += Physics.gravity;
        }
    }
    void ApplyIncline()
    {
        // Project movement vector to plane of incline
        Vector3 projToPlane = Vector3.ProjectOnPlane(movementVec, hitInfo.normal);

        if (debug)
        {
            // draw debug line
            Vector3 inclineVec = Vector3.Cross(transform.right, hitInfo.normal);
            Debug.DrawLine(transform.position, transform.position + inclineVec * height, Color.yellow);
            Debug.DrawLine(transform.position, transform.position + projToPlane * height, Color.magenta);
        }

        movementVec = projToPlane;
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
        
        if (!inputMoveable)
        {
            movementVec = Vector3.zero;
        }
        else
        {
            speedToUse = scaledSpeed;
            movementVec = CalculateMovementVec();
            currentDirection = CalculateDirection();
            RotateToCurrentDirection();
        }

        KnockbackUpdate();
        CheckGround();
        ApplyIncline();
        ApplyGravity();

        rb.MovePosition(rb.position + (movementVec * speedToUse * Time.fixedDeltaTime));
    }
}
