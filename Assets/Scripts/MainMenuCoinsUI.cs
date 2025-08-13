using UnityEngine;
using TMPro;              

public class MainMenuCoinsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText; 
    [SerializeField] private string prefix;

    private void OnEnable()
    {
        if (SaveManager.I != null) 
        { 
            SaveManager.I.OnTotalCoinsChanged += OnTotalCoinsChanged;       
        }
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
        { 
            coinsText.text = $"{prefix}{value:N0}";
        }
    }
}