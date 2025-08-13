using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpBoostPickup : MonoBehaviour
{
    [Header("Effect")]
    [SerializeField] private float jumpMultiplier = 1.5f;
    [SerializeField] private float duration = 5f;

    [Header("Pickup")]
    [SerializeField] private float pickupDelay = 1f;     
    [SerializeField] private Collider2D pickupTrigger;   

    private bool canPickup = false;

    private void Awake()
    {
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
        if (!canPickup || !other.CompareTag("Player")) return;

        var pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            // start the boost
            pc.StartCoroutine(pc.TemporaryJumpBoost(jumpMultiplier, duration));
        }

        Destroy(gameObject);
    }
}