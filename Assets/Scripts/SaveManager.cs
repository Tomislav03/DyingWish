using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager I { get; private set; }

    [System.Serializable]
    public class SaveData
    {
        public int totalCoins = 0;
        // add more later if you want (e.g., crownUnlocked, bestTimes, etc.)
    }

    public SaveData Data { get; private set; } = new SaveData();

    // Fire this whenever totalCoins changes so listeners (like PlayerCrown) can react.
    public System.Action<int> OnTotalCoinsChanged;

    string PathFile => Path.Combine(Application.persistentDataPath, "save.json");

    private void Awake()
    {
        // singleton that survives scene loads
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        Data.totalCoins += amount;
        Save();
        OnTotalCoinsChanged?.Invoke(Data.totalCoins);
    }

    public void ResetCoins()
    {
        Data.totalCoins = 0;
        Save();
        OnTotalCoinsChanged?.Invoke(Data.totalCoins); // notifies PlayerCrown to hide the crown
    }

    public void Save()
    {
        var json = JsonUtility.ToJson(Data, prettyPrint: true);
        File.WriteAllText(PathFile, json);
#if UNITY_EDITOR
        Debug.Log($"[Save] {PathFile}  totalCoins={Data.totalCoins}");
#endif
    }

    public void Load()
    {
        if (File.Exists(PathFile))
        {
            var json = File.ReadAllText(PathFile);
            Data = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
        }
        else
        {
            Data = new SaveData();
        }
    }

    [ContextMenu("Reset Save (Editor Only)")]
    public void ResetAll()
    {
        Data = new SaveData();
        Save();
#if UNITY_EDITOR
        Debug.Log("[Save] Reset to defaults.");
#endif
    }
}