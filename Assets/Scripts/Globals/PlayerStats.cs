using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats PS;

    [SerializeField]
    private float maxHealth = 100;

    [SerializeField]
    private Inventory inv;

    [SerializeField]
    private float healthPoints = 100;

    void Awake()
    {
        if (PS != null)
        {
            Debug.LogError("Only one PlayerStats allowed.");
            Destroy(gameObject);
            return;
        }
        else
        {
            PS = this;
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        healthPoints = maxHealth;

        if (inv == null)
        {
            inv = gameObject.GetComponent<Inventory>();
        }
    }

    public float TakeDamage(Collider src)
    {
        float damagePoints = src.GetComponent<EnemyHurtbox>().damage;

        return TakeDamage(damagePoints);
    }

    public float TakeDamage(float damagePoints)
    {
        healthPoints -= damagePoints;

        return healthPoints;
    }

    public float IncreaseHealth(float healthIncrease)
    {
        healthPoints = Mathf.Clamp(healthPoints + healthIncrease, 0, maxHealth);

        return healthPoints;
    }

    public float SetHealth(float newHealth)
    {
        healthPoints = newHealth;
        return healthPoints;
    }

    public float Health()
    {
        return healthPoints;
    }

    public Inventory Inventory()
    {
        return inv;
    }
}
