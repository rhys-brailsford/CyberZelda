using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public GameObject weaponObject;
    public Weapon wepScript;
    private bool atkHeld = false;

    public GameObject interactObject;
    public InteractHandler intScript;
    private bool intHeld = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(weaponObject != null, "InputHandler: Weapon Object cannot be empty!");
        Debug.Assert(interactObject != null, "InputHandler: Interact Object cannot be empty!");

        wepScript = weaponObject.GetComponent<Weapon>();
        intScript = interactObject.GetComponent<InteractHandler>();

        Debug.Assert(wepScript != null, "InputHandler: Weapon Object must contain Weapon script!");
        Debug.Assert(intScript != null, "InputHandler: Interact Object must contain InteractHandler script!");
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        float atk = Input.GetAxisRaw("Attack");
        float interact = Input.GetAxisRaw("Interact");

        HandleAttack(atk);
        HandleInteract(interact);
    }

    private void HandleAttack(float input)
    {
        if (input > 0)
        {
            if (!atkHeld)
            {
                Debug.Log("Attack Input");
                wepScript.StartAttack();
            }
            else
            {
                // :TODO: Attack held behaviour
            }

            atkHeld = true;
        }
        else
        {
            atkHeld = false;
        }
    }

    private void HandleInteract(float input)
    {
        if (input > 0)
        {
            if (!intHeld)
            {
                Debug.Log("InteractStart Input");
                intScript.StartInteract();
            }
            intHeld = true;
        }
        else
        {
            if (intHeld)
            {
                Debug.Log("InteractStart Input");
                intScript.EndInteract();
            }
            intHeld = false;
        }
    }
}
