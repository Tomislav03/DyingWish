using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;   // put this script on the Player
    [SerializeField] private float speed = 4f;

    private float vertical;
    private bool isLadder;
    private bool isClimbing;
    private float originalGravity;
    [SerializeField] private float deadzone = 0.1f; // how much input to start climbing

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        originalGravity = rb != null ? rb.gravityScale : 3f;
    }

    void Update()
    {
        vertical = Input.GetAxisRaw("Vertical");

        // Start climbing only when inside ladder AND you press up/down
        if (isLadder && Mathf.Abs(vertical) > deadzone)
        {
            isClimbing = true;
        }
        // Optional: if you leave the ladder zone, stop climbing (handled in OnTriggerExit too)
        if (!isLadder) isClimbing = false;

        if (isLadder && Input.GetButtonDown("Jump"))
        {
            isClimbing = false;
            rb.gravityScale = originalGravity; // let your PlayerController apply the jump
        }
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, vertical * speed);
        }
        else
        {
            // If standing in ladder but not actively climbing, don't force Y—just restore gravity
            rb.gravityScale = originalGravity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
            // Do not set isClimbing yet; wait for input so we don't snap upward
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
            rb.gravityScale = originalGravity;
        }
    }
}
