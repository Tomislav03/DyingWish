using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnPlayButton()
    {
        SceneManager.LoadScene("LevelOne");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void OnResetCoinsButton()
    {
        SaveManager.I?.ResetCoins();
        Debug.Log("Coins reset.");
    }

    public void OnResetProgressButton()
    {
        SaveManager.I?.ResetAll();
        Debug.Log("All progress reset.");
    }
}
