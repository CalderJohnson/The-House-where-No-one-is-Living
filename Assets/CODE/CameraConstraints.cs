using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConstraints : MonoBehaviour
{
    public Transform player;
    public float positiveX = 20f;
    public float negativeX = -20f;
    public float positiveZ = 20f;
    public float negativeZ = -20f;

    public float offsetX = 0f;
    public float offsetY = 15f;
    public float offsetz = -17f;

    private Vector3 MinCamPosition;
    private Vector3 MaxCamPosition;

    void Start()
    {
        MinCamPosition = new Vector3(negativeX, -20, negativeZ);
        MaxCamPosition = new Vector3(positiveX, 20, positiveX);
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("CameraConstraints: Player is missing! Skipping camera update.");
            return;
        }

        transform.position = new Vector3(
            Mathf.Clamp(player.transform.position.x + offsetX, MinCamPosition.x, MaxCamPosition.x),
            Mathf.Clamp(player.transform.position.y + offsetY, MinCamPosition.y, MaxCamPosition.y),
            Mathf.Clamp(player.transform.position.z + offsetz, MinCamPosition.z, MaxCamPosition.z)
        );
    }
}
