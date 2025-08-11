using UnityEngine;

public class PlayerCrown : MonoBehaviour
{
    [Header("Unlock")]
    [SerializeField] private int coinThreshold = 1;  // set your target here

    [Header("Visuals")]
    [SerializeField] private GameObject crownObject;  // child object with a SpriteRenderer
    [SerializeField] private Animator animator;       // optional
    [SerializeField] private string hasCrownBool = "HasCrown"; // optional animator bool

    private void OnEnable()
    {
        if (SaveManager.I != null)
            SaveManager.I.OnTotalCoinsChanged += OnTotalCoinsChanged;
    }

    private void Start()
    {
        // Apply initial state on scene load
        int total = SaveManager.I != null ? SaveManager.I.Data.totalCoins : 0;
        ApplyCrown(total >= coinThreshold);
    }

    private void OnDisable()
    {
        if (SaveManager.I != null)
            SaveManager.I.OnTotalCoinsChanged -= OnTotalCoinsChanged;
    }

    private void OnTotalCoinsChanged(int newTotal)
    {
        ApplyCrown(newTotal >= coinThreshold);
    }

    private void ApplyCrown(bool hasCrown)
    {
        if (crownObject != null) crownObject.SetActive(hasCrown);
        if (animator != null && !string.IsNullOrEmpty(hasCrownBool))
            animator.SetBool(hasCrownBool, hasCrown);
    }
}
