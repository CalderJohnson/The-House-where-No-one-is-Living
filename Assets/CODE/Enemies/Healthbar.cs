using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public float health = 10;
    private float lastDamageTime; // Timestamp of when damage was last taken (for regeneration control)
    private bool died; // Did this entity die? Used in adaptive enemy training
    public bool active;
    public event Action OnDeath; // Event triggered when health reaches zero
    public UnityEvent Ragdoll;
    public Slider healthBarUI;

    public void Initialize(float initialHealth)
    {
        active = true;
        health = initialHealth;
        lastDamageTime = -1f;
    }

    public void TakeDamage(int damage)
    {
        if (active) // If enemy is not blocking
        {
            health -= damage;
            lastDamageTime = Time.time;
        }
        //Debug.Log($"Health remaining: {health}");
        if (health <= 0)
        {
            Die();
        }
        if(healthBarUI != null && healthBarUI.value != health){
            healthBarUI.value = health;
        }
    }

    void FixedUpdate(){
        if(health<0){
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
        //Debug.Log($"{gameObject.name} has died.");
        died = true;
        OnDeath?.Invoke(); // Invoke the death event
        GetComponent<UpdateNode>()?.TryUpdateDecisionNode();
        Ragdoll.Invoke();
    }
}

