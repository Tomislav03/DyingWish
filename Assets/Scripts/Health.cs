using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // <- make sure this is here for IEnumerator

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3f;
    [SerializeField] private float hitStopDuration = 0.08f; // tweak: 0.05–0.12 feels good
    [SerializeField] private float iFramesDuration = 1f;
    public float currentHealth { get; private set; }
    private bool isHitStopping = false;
    private bool isInvincible = false;

    private void Awake()
    {
        currentHealth = startingHealth;
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
                StartCoroutine(camShake.Shake(0.15f, 0.1f)); // duration, magnitude

            Debug.Log("Player took damage. Remaining: " + currentHealth);
        }
        else
        {
            // Player dead — show death screen (it does its own freeze)
            Debug.Log("Player died!");
            PlayerController controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.ShowDeathMessage();
            }
        }
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        isHitStopping = true;
        float prevScale = Time.timeScale;

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration); // unaffected by timeScale = 0
        Time.timeScale = prevScale;

        isHitStopping = false;
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(iFramesDuration);

        isInvincible = false;
    }


    public void AddHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, startingHealth);
        Debug.Log("Gained health. Current: " + currentHealth);
    }
}