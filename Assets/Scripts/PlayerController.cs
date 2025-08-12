using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float jumpForce;
    [SerializeField] private Animator animator;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float climbSpeed = 4f;
    [SerializeField] private float extraLift = 0.1f; // small safety margin
    [SerializeField] private GameObject deathMessageUI;

    AudioManager audioManager;

    public SpriteRenderer sr;
    private bool isGrounded;
    public static int score;
    public TextMeshProUGUI scoreText;
    private int jumpCount = 0;
    private Vector3 respawnPoint;
    private bool isClimbing = false;
    private bool isOnLadder = false;
    private float baseJumpForce;
    private int activeJumpBoost;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        respawnPoint = transform.position;
        scoreText.text = "Coins : " + score;
        deathMessageUI.SetActive(false);
        baseJumpForce = jumpForce;

        // UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0); // reset Y velocity
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
            audioManager.PlaySFX(audioManager.jump);
            
            if(jumpCount == 1)
            {
                animator.SetBool("isJumping", true);
            }
        }

        if (transform.position.y < -10)
        {
            Health health = GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(1f);
                Respawn();
            }
        }

        bool falling = !isGrounded && rb.velocity.y < -0.1f;
        animator.SetBool("isFalling", falling);
    }

    private void FixedUpdate() // runs at a fixed time rate
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if(moveInput != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (rb.velocity.x > 0)
        {
            sr.flipX = false;
        }
        else if (rb.velocity.x < 0)
        {
            sr.flipX = true;
        }

        if (isOnLadder)
        {
            float verticalInput = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(verticalInput) > 0.1f)
            {
                isClimbing = true;
            }

            if (isClimbing)
            {
                rb.gravityScale = 0f; // turn off gravity
                rb.velocity = new Vector2(rb.velocity.x, verticalInput * climbSpeed);
            }
        }
        else
        {
            // Reset when not on ladder
            if (isClimbing)
            {
                rb.gravityScale = 3f; // restore gravity (adjust as needed)
                isClimbing = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Vector2.Dot(collision.GetContact(0).normal, Vector2.up) > 0.8f)
        {
            isGrounded = true;
            jumpCount = 0; // reset jump count
            animator.SetBool("isJumping", false); // optional
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = false;
        }
    }

    public void GameOver() // function is public so you can call it outside of the PlayerController script
    {
        Respawn();
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Coins : " + score;

        if (score % 10 == 0)
        {
            Health health = GetComponent<Health>();
            if (health != null)
            {
                health.AddHealth(1f); // Gain 1 heart every 10 coins
            }
        }
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        respawnPoint = newCheckpoint;
    }

    public void Respawn()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        float halfHeight = 0.5f;
        var col = GetComponent<Collider2D>();
        if (col != null) halfHeight = col.bounds.extents.y;
        transform.position = respawnPoint + new Vector3(0f, halfHeight + extraLift, 0f);

        isOnLadder = false;
        isClimbing = false;
        jumpCount = 0;
        isGrounded = false;

        animator.SetBool("isRunning", false);
        animator.SetBool("isJumping", false);
        animator.SetBool("isFalling", false);
    }

    public void ShowDeathMessage()
    {
        deathMessageUI.SetActive(true);     // Show the message
        Time.timeScale = 0f;
        StartCoroutine(WaitForKeyToRestart());
    }

    private IEnumerator WaitForKeyToRestart()
    {
        yield return new WaitForSecondsRealtime(1f); // short dramatic pause

        // Wait until player presses any key
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        score = 0;

        Time.timeScale = 1f; // Unfreeze the game before restart
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public IEnumerator TemporaryJumpBoost(float multiplier, float seconds)
    {
        if (activeJumpBoost == 0) baseJumpForce = jumpForce;

        activeJumpBoost++;
        jumpForce = baseJumpForce * multiplier;

        yield return new WaitForSeconds(seconds);

        activeJumpBoost = Mathf.Max(0, activeJumpBoost - 1);
        if (activeJumpBoost == 0)
            jumpForce = baseJumpForce;
    }
}
