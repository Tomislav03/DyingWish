using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    AudioManager audioManager;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            PlayerController playerController = collision.GetComponent<PlayerController>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
                audioManager.PlaySFX(audioManager.spikes);
            }

            if (playerController != null)
            {
                playerController.Respawn();
            }
        }
    }
}
