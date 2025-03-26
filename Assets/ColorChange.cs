using System.Collections;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    public Material redMaterial;
    public Material greenMaterial;
    private Material originalMaterial;
    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material; // Store original material
    }

    public void ChangeMaterialRed()
    {
        StartCoroutine(ChangeMaterialCoroutine(redMaterial));
    }

    public void ChangeMaterialGreen()
    {
        StartCoroutine(ChangeMaterialCoroutine(greenMaterial));
    }

    IEnumerator ChangeMaterialCoroutine(Material newMaterial)
    {
        objectRenderer.material = newMaterial; // Change material
        yield return new WaitForSeconds(5); // Wait for 5 seconds
        objectRenderer.material = originalMaterial; // Revert back
    }
}