using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SeekerEnemy : MonoBehaviour
{
    public enum State { Idle, Chasing, Returning }

    [Header("Detection")]
    [SerializeField] private float aggroRadius = 6f;       // start chasing when inside this
    [SerializeField] private float deAggroRadius = 6.5f;   // stop chasing when outside this (slightly larger to avoid flicker)

    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool airMovement = true;      // if false, only move along X (grounded feel)
    [SerializeField] private float arriveThreshold = 0.05f;

    [Header("Player Interaction")]
    [SerializeField] private float stompHeightThreshold = 0.5f; // how much above this enemy the player must be
    [SerializeField] private float bounceVelocity = 8f;
    [SerializeField] private int damage = 1;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer sr; // optional; auto-fills if left empty

    AudioManager audioManager;

    private Transform player;
    private Vector3 startPos;
    private State state = State.Idle;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();

        // Make sure we render above tiles (optional, but avoids “invisible” issues)
        if (sr != null)
        {
            // Adjust these to your project’s layers if needed:
            // sr.sortingLayerName = "Characters";
            // sr.sortingOrder = 5;
        }

        // Make this collider a trigger for easy stomp/damage detection
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        // Kinematic RB helps moving triggers behave consistently (optional but recommended)
        if (GetComponent<Rigidbody2D>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;
            rb.gravityScale = 0f;
        }
    }

    private void Start()
    {
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null) player = playerGO.transform;

        startPos = transform.position;
    }

    private void Update()
    {
        // Keep on 2D plane
        if (Mathf.Abs(transform.position.z) > 0.0001f)
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Idle:
                if (dist <= aggroRadius) state = State.Chasing;
                break;

            case State.Chasing:
                if (dist > deAggroRadius) state = State.Returning;
                break;

            case State.Returning:
                if (Vector2.Distance(transform.position, startPos) <= arriveThreshold)
                    state = State.Idle;
                break;
        }

        // Move based on state
        if (state == State.Chasing)
            MoveTowards(player.position);
        else if (state == State.Returning)
            MoveTowards(startPos);
    }

    private void MoveTowards(Vector3 target)
    {
        Vector3 from = transform.position;
        Vector3 to = target;

        if (!airMovement) // ground-style: follow only on X
            to = new Vector3(target.x, transform.position.y, 0f);

        Vector3 next = Vector2.MoveTowards(from, to, speed * Time.deltaTime);
        transform.position = next;

        // Flip sprite to face movement
        if (sr != null)
        {
            float dx = next.x - from.x;
            if (dx > 0.001f) sr.flipX = true;
            else if (dx < -0.001f) sr.flipX = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only react to the player's main body (ignore child triggers)
        var pc = collision.GetComponentInParent<PlayerController>();
        if (pc == null) return;

        // Use the player's rigidbody for velocity checks/bounce
        Rigidbody2D playerRb = collision.attachedRigidbody;

        // Is the player above us and falling/downwards-ish?
        Vector2 toPlayer = (pc.transform.position - transform.position);
        bool playerAbove = toPlayer.y > stompHeightThreshold;
        bool playerFalling = playerRb != null && playerRb.velocity.y <= 0f;

        if (playerAbove && playerFalling)
        {
            // Stomp kill
            if (playerRb != null)
                playerRb.velocity = new Vector2(playerRb.velocity.x, bounceVelocity);

            audioManager.PlaySFX(audioManager.defeatEnemy);
            Destroy(gameObject);
        }
        else
        {
            // Side/bottom hit -> damage player
            var health = pc.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(damage);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
        Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, deAggroRadius);
    }
#endif
}
