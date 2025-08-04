using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public bool finalLevel;
    public string nextLevelName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(finalLevel == true)
            {
                collision.GetComponent<PlayerController>().ResetScore();
                SceneManager.LoadScene(0); // loads Main Menu scene
            }
            else
            {
                SceneManager.LoadScene(nextLevelName);
            }
        }
    }
}
