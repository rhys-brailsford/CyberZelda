using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public GameObject weaponObject;
    private Weapon weaponScript;
    private bool atkHeld = false;

    public GameObject interactObject;
    private InteractHandler interactHandler;
    private bool intHeld = false;

    private bool useHeld = false;

    public Inventory inventoryHandler;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(weaponObject != null, "InputHandler: Weapon Object cannot be empty!");
        Debug.Assert(interactObject != null, "InputHandler: Interact Object cannot be empty!");

        weaponScript = weaponObject.GetComponent<Weapon>();
        interactHandler = interactObject.GetComponent<InteractHandler>();

        Debug.Assert(weaponScript != null, "InputHandler: Weapon Object must contain Weapon script!");
        Debug.Assert(interactHandler != null, "InputHandler: Interact Object must contain InteractHandler script!");
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

        float useEquipped = Input.GetAxisRaw("UseEquipped");
        HandleUseEquipped(useEquipped);

        // Temporary equip inputs
        float equip1 = Input.GetAxisRaw("EquipSlot1");
        HandleEquip(equip1);
    }

    // Temporary item equip handler
    private void HandleEquip(float input)
    {
        if (input > 0)
        {
            ItemName itemEquipped = inventoryHandler.equippedItem;
            switch(itemEquipped)
            {
                case (ItemName.TempInvItem1):
                    inventoryHandler.EquipItem(ItemName.Gun);
                    break;
                case (ItemName.Gun):
                    inventoryHandler.EquipItem(ItemName.TempInvItem1);
                    break;
                default:
                    inventoryHandler.EquipItem(ItemName.Gun);
                    break;
            }
        }
    }

    private void HandleUseEquipped(float input)
    {
        if (input > 0)
        {
            if (!useHeld)
            {
                GameManager.GM.GetPlayer().GetComponent<PlayerMovement>().UseEquipped();
            }
            else
            {
                // :TODO: UseEquipped held behaviour
            }

            useHeld = true;
        }
        else
        {
            useHeld = false;
        }
    }

    private void HandleAttack(float input)
    {
        if (input > 0)
        {
            if (!atkHeld)
            {
                Debug.Log("Attack Input");
                weaponScript.StartAttack();
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
                interactHandler.StartInteract();
            }
            intHeld = true;
        }
        else
        {
            if (intHeld)
            {
                interactHandler.EndInteract();
            }
            intHeld = false;
        }
    }
}
