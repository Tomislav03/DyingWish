using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3f;
    public float currentHealth { get; private set; }

    private void Awake()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            // Optional: flash effect or hurt animation
            Debug.Log("Player took damage. Remaining: " + currentHealth);
        }
        else
        {
            // Player dead
            Debug.Log("Player died!");

            PlayerController controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.ShowDeathMessage();
                /*controller.ResetScore(); // optional
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);*/
            }
        }
    }

    public void AddHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, startingHealth);
        Debug.Log("Gained health. Current: " + currentHealth);
    }

}