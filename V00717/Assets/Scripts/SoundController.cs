using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    // Male
    public AudioClip maleSound;
    public AudioClip maleNarrator;
    // Unknown
    public AudioClip unknownSound;
    // Female
    public AudioClip femaleSound;
    public AudioClip femaleNarrator;
    // The audio source to play the clips
    private AudioSource AudioSource;
    // The volume
    private float volume = 1.0f;

    // Attach the event listeners
    public void OnEnable()
    {
        BabyController._OnSexChanged += MakeGenderSounds;
    }
    // Detach the event listeners
    public void OnDisable()
    {
        BabyController._OnSexChanged -= MakeGenderSounds;
    }

    // Get the audio source component
    private void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    // Make sound based on 
    public void MakeGenderSounds(string sex)
    {
        // Stop playing sound if it wasn't done playing
        if(AudioSource.isPlaying)
        {
            AudioSource.Stop();
        }
        // Match which sound to play
        switch(sex)
        {
            case "male":
                AudioSource.PlayOneShot(maleSound, volume);
                AudioSource.PlayOneShot(maleNarrator, volume);
                break;
            case "female":
                AudioSource.PlayOneShot(femaleSound, volume);
                AudioSource.PlayOneShot(femaleNarrator, volume);
                break;
            case "unknown":
                AudioSource.PlayOneShot(unknownSound, volume);
                break;
            case "random":
                break;
            default:
                break;
        }
    }
}
