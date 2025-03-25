using UnityEngine;
using System.Collections;

public class RagdollFreeze : MonoBehaviour
{
    private Rigidbody[] ragdollParts;
    Animator ZombieAnim;

    void Start()
    {
        ZombieAnim = GetComponent<Animator>();
        // Get all rigidbodies from the ragdoll
        ragdollParts = GetComponentsInChildren<Rigidbody>();
    }

    public void FreezeRagdoll()
    {
        foreach (Transform child in transform)
        {
            child.SetParent(null);
        }
    }

    public void CrawlAnim()
    {
        ZombieAnim.SetBool("ZombieHurt",true);
    }
}