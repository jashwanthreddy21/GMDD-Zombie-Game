using UnityEngine;

/// <summary>
/// Detects when the player falls below a threshold and teleports them
/// back to the spawn position. Attach this to the Player GameObject.
/// </summary>
public class PlayerFallSafety : MonoBehaviour
{
    [Tooltip("If the player's Y drops below this value, they will be respawned.")]
    [SerializeField] private float fallThresholdY = 10f;

    [Tooltip("Height above the fall threshold to respawn the player at.")]
    [SerializeField] private float respawnOffsetY = 5f;

    private Vector3 spawnPosition;
    private Rigidbody rb;

    private void Start()
    {
        spawnPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.position.y < fallThresholdY)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        transform.position = new Vector3(spawnPosition.x, spawnPosition.y + respawnOffsetY, spawnPosition.z);
    }
}
