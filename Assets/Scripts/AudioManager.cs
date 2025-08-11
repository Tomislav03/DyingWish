using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip background;
    public AudioClip jump;
    public AudioClip hit;
    public AudioClip defeatEnemy;
    public AudioClip death;
    public AudioClip coin;
    public AudioClip levelCompleted;
    public AudioClip GetHeart;
    public AudioClip checkpoint;
    public AudioClip spikes;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
