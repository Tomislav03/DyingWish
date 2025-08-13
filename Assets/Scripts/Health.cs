using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3f;
    [SerializeField] private float hitStopDuration = 0.08f;
    [SerializeField] private float iFramesDuration = 1f;

    [SerializeField] private float spawnProtectionDuration = 0.75f;

    AudioManager audioManager;
    public float currentHealth { get; private set; }
    private bool isHitStopping = false;
    private bool isInvincible = false;

    private void Awake()
    {
        currentHealth = startingHealth;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        if (spawnProtectionDuration > 0f)
        {
            StartCoroutine(SpawnProtection());
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible)
        {
            return;
        }
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            StartCoroutine(InvincibilityRoutine());
            // brief freeze when hurt
            if (!isHitStopping) StartCoroutine(HitStopRoutine(hitStopDuration));
            var camShake = Camera.main != null ? Camera.main.GetComponent<CameraShake>() : null;
            if (camShake != null)
                StartCoroutine(camShake.Shake(0.15f, 0.1f));
            Debug.Log("Player took damage. Remaining: " + currentHealth);

            audioManager.PlaySFX(audioManager.hit);
        }
        else
        {
            Debug.Log("Player died!");
            var audio = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
            if (audio != null) audio.StopMusic();

            PlayerController controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.ShowDeathMessage();
                audioManager.PlaySFX(audioManager.death);
            }
        }
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        isHitStopping = true;
        float prevScale = Time.timeScale;

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = prevScale;

        isHitStopping = false;
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(iFramesDuration);

        isInvincible = false;
    }

    private IEnumerator SpawnProtection()
    {
        isInvincible = true;
        yield return new WaitForSeconds(spawnProtectionDuration);
        isInvincible = false;
    }


    public void AddHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, startingHealth);
        audioManager.PlaySFX(audioManager.GetHeart);
        Debug.Log("Gained health. Current: " + currentHealth);
    }
}