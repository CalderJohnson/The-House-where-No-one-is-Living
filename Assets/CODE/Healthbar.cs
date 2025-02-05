using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    private float health;
    private float lastDamageTime; // Timestamp of when damage was last taken (for regeneration control)
    public event Action OnDeath; // Event triggered when health reaches zero

    public void Initialize(float initialHealth)
    {
        health = initialHealth;
        lastDamageTime = -1f;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        lastDamageTime = Time.time;
        Debug.Log($"Health remaining: {health}");
        if (health <= 0)
        {
            Die();
        }
    }

    public float GetHealth() 
    { 
        return health; 
    }

    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }

    public float GetLastDamageTime()
    {
        return lastDamageTime;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        OnDeath?.Invoke(); // Invoke the death event
    }
}
