using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 moveOffset;
    [SerializeField] private SpriteRenderer sr;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (transform.position == targetPosition)
        {
            if (targetPosition == startPosition)
            {
                targetPosition = startPosition + moveOffset;
            }
            else
            {
                targetPosition = startPosition;
            }
        }
        if ((targetPosition - transform.position).x > 0)
        {
            sr.flipX = false; 
        }
        else
        {
            sr.flipX = true; 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 contactPoint = collision.transform.position - transform.position;

            if (contactPoint.y > 0.5f)
            {
                // DROP POWERUP HERE
                var dropper = GetComponent<LootDropper>();
                if (dropper != null) dropper.TryDrop();

                Destroy(gameObject);

                Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 8f);
                }
            }
            else
            {
                Health playerHealth = collision.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1f);
                }
            }
        }
    }
}
