using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    private float health;
    public event Action OnDeath; // Event triggered when health reaches zero

    public void Initialize(float initialHealth)
    {
        health = initialHealth;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"Health remaining: {health}");
        if (health <= 0)
        {
            Die();
        }
    }

    public float getHealth() { 
        return health; 
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        OnDeath?.Invoke(); // Invoke the death event
    }
}
