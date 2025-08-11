using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    public bool chase = false;
    public Transform startingPoint;

    [Header("Player Interaction")]
    [SerializeField] private float stompHeightThreshold = 0.5f; // how much above the enemy the player must be
    [SerializeField] private float bounceVelocity = 8f;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer sr; // optional (for flip)

    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (startingPoint == null)
        {
            // fall back to current position if not set
            GameObject sp = new GameObject($"{name}_StartPoint");
            sp.transform.position = transform.position;
            startingPoint = sp.transform;
        }
    }

    void Update()
    {
        if (chase)
            Chase();
        else
            ReturnStartPosition();

        Flip();
    }

    private void Chase()
    {
        if (player == null) return;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    private void ReturnStartPosition()
    {
        transform.position = Vector2.MoveTowards(transform.position, startingPoint.position, speed * Time.deltaTime);
    }

    private void Flip()
    {
        if (player == null || sr == null) return;
        // face the player
        sr.flipX = player.transform.position.x < transform.position.x;
    }

    // ===== Player interaction (same behavior as your original Enemy) =====
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // relative position of player vs this enemy
        Vector2 toPlayer = (collision.transform.position - transform.position);
        Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();

        bool playerAbove = toPlayer.y > stompHeightThreshold;
        bool playerFalling = playerRb != null && playerRb.velocity.y <= 0f;

        if (playerAbove && playerFalling)
        {
            // Stomp kill
            Destroy(gameObject);

            // Bounce player up
            if (playerRb != null)
                playerRb.velocity = new Vector2(playerRb.velocity.x, bounceVelocity);
        }
        else
        {
            // Side/bottom hit → damage player
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.TakeDamage(1f);
        }
    }
}
