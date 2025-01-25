using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    // Slingshot Settings
    public GameObject projectilePrefab;  // The projectile (e.g., a stone)
    public Transform shootPoint;         // The point where the projectile spawns
    public float projectileSpeed = 20f;  // Speed of the projectile
    public float attackCooldown = 1.0f;  // Cooldown between shots

    private float nextAttackTime = 0f;

    // Update is called once per frame
    //void Update()
    //{
    //    // Check for input and cooldown
    //    if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextAttackTime)
    //    {
    //        ShootProjectile();
    //        nextAttackTime = Time.time + attackCooldown; // Set the cooldown
    //    }
    //}

    public void ShootProjectile()
    {
        if (!(Time.time >= nextAttackTime))
        {
            return;
        }
        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

        // Apply velocity to the projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootPoint.forward * projectileSpeed;
        }

        // Destroy the projectile after a set time to prevent memory leaks
        Destroy(projectile, 5f);

        nextAttackTime = Time.time + attackCooldown;
    }
}