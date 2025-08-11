using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerup : MonoBehaviour
{
    [Header("Effect")]
    [SerializeField] private float speedMultiplier = 1.5f;
    [SerializeField] private float duration = 5f;

    [Header("Pickup")]
    [SerializeField] private float pickupDelay = 1f;     // grace period
    [SerializeField] private Collider2D pickupTrigger;   // <- assign the TRIGGER collider here

    private bool canPickup = false;

    private void Awake()
    {
        // Make sure the trigger starts OFF (no pickup during grace period)
        if (pickupTrigger != null) pickupTrigger.enabled = false;
        StartCoroutine(EnablePickupAfterDelay());
    }

    private IEnumerator EnablePickupAfterDelay()
    {
        yield return new WaitForSeconds(pickupDelay);
        canPickup = true;
        if (pickupTrigger != null) pickupTrigger.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canPickup) return;
        if (!other.CompareTag("Player")) return;

        var pc = other.GetComponent<PlayerController>();
        if (pc != null)
            StartCoroutine(ApplySpeed(pc));

        Destroy(gameObject);
    }

    private IEnumerator ApplySpeed(PlayerController pc)
    {
        // Access your private moveSpeed safely via reflection (quickest drop-in for your current codebase)
        var field = typeof(PlayerController).GetField("moveSpeed",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field == null) yield break;

        float original = (float)field.GetValue(pc);
        field.SetValue(pc, original * speedMultiplier);

        yield return new WaitForSeconds(duration);

        if (pc != null)
            field.SetValue(pc, original);
    }
}
