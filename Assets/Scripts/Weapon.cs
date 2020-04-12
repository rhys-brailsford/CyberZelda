using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 0;
    public List<Mesh> weaponMorphs;
    public float attackSpeedSec = 0;
    private int attackStages = 0;
    private bool attacking = false;
    private float curAttackDuration = 0;
    private bool attackHeld = false;

    public PlayerMovement movementController;



    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(weaponMorphs.Count > 0, "List of weapon morphs must be > 0");
        Debug.Assert(attackSpeedSec > 0, "Attack duration must be > 0");
        Debug.Assert(damage > 0, "Attack damage must be > 0");

        List<Tags> tags = gameObject.GetComponent<CustomTags>().tags;
        if (!tags.Contains(Tags.PlayerWeapon))
        {
            Debug.LogError(Tags.PlayerWeapon + " tag expected on GameObject: " + gameObject.name + ". Original value: " + gameObject.tag);
        }

        ResetWeapon();
        attackStages = weaponMorphs.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if (attacking)
        {
            UpdateAttack();
        }
    }

    public void StartAttack()
    {
        // Get ready to start attacking animation
        attacking = true;
        curAttackDuration = 0;
        gameObject.GetComponent<MeshCollider>().enabled = true;
        gameObject.GetComponent<MeshRenderer>().enabled = true;

        movementController.DisableInputMovement();
    }

    void UpdateAttack()
    {
        if (curAttackDuration > attackSpeedSec)
        {
            // Turn off
            ResetWeapon();
            movementController.EnableInputMovement();
            return;
        }

        // Play animation
        int morphIndex = Mathf.FloorToInt(attackStages * (curAttackDuration / attackSpeedSec));
        Mesh curMesh = weaponMorphs[morphIndex];

        // Only update weapon mesh/collision if it changes
        if (gameObject.GetComponent<MeshFilter>().sharedMesh.name != curMesh.name)
        {
            gameObject.GetComponent<MeshFilter>().sharedMesh = curMesh;
            gameObject.GetComponent<MeshCollider>().sharedMesh = curMesh;
        }

        curAttackDuration += Time.deltaTime;
    }

    void ResetWeapon()
    {
        Debug.Assert(weaponMorphs.Count > 0, "List of weapon morphs must be > 0");
        gameObject.GetComponent<MeshCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<MeshFilter>().sharedMesh = weaponMorphs[0];
        gameObject.GetComponent<MeshCollider>().sharedMesh = weaponMorphs[0];

        attacking = false;
    }

}
