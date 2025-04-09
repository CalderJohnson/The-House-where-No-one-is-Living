using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Healthbar : MonoBehaviour
{
    private float health;
    private float lastDamageTime; // Timestamp of when damage was last taken (for regeneration control)
    private bool died; // Did this entity die? Used in adaptive enemy training
    public event Action OnDeath; // Event triggered when health reaches zero
    public UnityEvent Ragdoll;

    public void Initialize(float initialHealth)
    {
        health = initialHealth;
        lastDamageTime = -1f;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        lastDamageTime = Time.time;
        // Debug.Log($"Health remaining: {health}");
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

    public bool DiedRecently()
    {
        bool temp = died;
        died = false;
        return temp;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        died = true;
        OnDeath?.Invoke(); // Invoke the death event
        Ragdoll.Invoke();
    }
}
