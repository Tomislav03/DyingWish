using UnityEngine;
using TMPro;                 // remove this and use UnityEngine.UI.Text if you use legacy Text

public class MainMenuCoinsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;   // drag your TMP text here
    [SerializeField] private string prefix = "Total Coins: ";

    private void OnEnable()
    {
        // Subscribe so it live-updates (e.g., after pressing your Reset button)
        if (SaveManager.I != null)
            SaveManager.I.OnTotalCoinsChanged += OnTotalCoinsChanged;

        // Also refresh immediately when the menu opens
        RefreshNow();
    }

    private void OnDisable()
    {
        if (SaveManager.I != null)
            SaveManager.I.OnTotalCoinsChanged -= OnTotalCoinsChanged;
    }

    private void OnTotalCoinsChanged(int newTotal) => SetText(newTotal);

    private void RefreshNow()
    {
        int total = (SaveManager.I != null) ? SaveManager.I.Data.totalCoins : 0;
        SetText(total);
    }

    private void SetText(int value)
    {
        if (coinsText != null)
            coinsText.text = $"{prefix}{value:N0}"; // N0 adds thousands separators (1,234)
    }
}