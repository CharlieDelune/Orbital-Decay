 using UnityEngine;
 using System.Collections;
 
 public class Playlist : MonoBehaviour
 {
    [SerializeField] private AudioClip[] soundtrack;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private int currentTrack;

    void Start ()
    {
        currentTrack = 0;
        if (!audioSource.playOnAwake)
        {
            audioSource.clip = soundtrack[currentTrack];
            audioSource.Play();
        }
    }

    void Update ()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = soundtrack[currentTrack];
            audioSource.Play();
            currentTrack++;

            if (currentTrack >= soundtrack.Length)
            {
                currentTrack = 0;
            }
        }
    }
 }