using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    private float maxHealth = 200f;
    private Healthbar healthbar;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        healthbar = GetComponent<Healthbar>();
        animator = GetComponent<Animator>();

        if (healthbar != null)
        {
            healthbar.Initialize(maxHealth); // Set initial health
            healthbar.OnDeath += HandleDeath; // Subscribe to the death event
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleDeath()
    {
        Debug.Log("Dummy had been killed!");

        // Trigger the death animation
        if (animator != null)
        {
            Debug.Log("Playing dummy death animation...");
            animator.SetTrigger("Death");
        }
    }
}
