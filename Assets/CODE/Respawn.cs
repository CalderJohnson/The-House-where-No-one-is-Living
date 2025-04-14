using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    private Healthbar healthbar;

    void Start()
    {
        healthbar = GetComponent<Healthbar>();

        if (healthbar != null)
        {
            healthbar.OnDeath += ReloadScene;
        }
        else
        {
            Debug.LogWarning("Healthbar component not found on Respawn object.");
        }
    }

    private void ReloadScene()
    {
        Debug.Log("Player died. Reloading scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (healthbar != null)
        {
            healthbar.OnDeath -= ReloadScene;
        }
    }
}