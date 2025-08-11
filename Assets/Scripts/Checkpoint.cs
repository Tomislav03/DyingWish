using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private AudioManager audioManager;
    private bool activated = false;
    private Collider2D col;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;                   // already used

        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<PlayerController>();
            if (player == null) return;

            activated = true;                    // mark as used

            player.SetCheckpoint(transform.position);
            if (audioManager != null)
                audioManager.PlaySFX(audioManager.checkpoint);

            if (col != null) col.enabled = false; // optional: stop future triggers
            // Optional: change sprite to a "used" look here
        }
    }
}