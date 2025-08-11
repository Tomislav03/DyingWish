using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int scoreToGive;
    [SerializeField] private float floatHeight;
    [SerializeField] private float floatSpeed;

    private float startYPosition;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        startYPosition = transform.position.y;
    }

    private void Update()
    {
        float newY = startYPosition + (Mathf.Sin(Time.time * floatSpeed)* floatHeight);
        transform.position = new Vector3(transform.position.x, newY, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().AddScore(scoreToGive);
            Destroy(gameObject);
            audioManager.PlaySFX(audioManager.coin);
        }
    }
}
