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
    // Water stream
    public AudioClip waterStream;

    /// <summary>
    /// Jukebox Songs
    /// </summary>
    public AudioClip evad_myopic_horses;
    public AudioClip evad_death_washing_machine;
    public AudioClip eval_dope_life;
    public AudioClip evad_stars;
    public AudioClip evad_valley_of_winds;
    public AudioClip ericd_is_it_better;

    // Poems/Recitals/Lyrics
    public AudioClip ashleyd_summer_stream;
    public AudioClip ericd_death_or_glory;

    // All songs
    private AudioClip[] jukebox;

    // The audio source to play the clips
    public AudioSource AudioSource;
    // The volume
    private float volume = 1.0f;

    // Attach the event listeners
    public void OnEnable()
    {
        CharacterCreationView._OnSexChanged += MakeGenderSounds;
    }
    // Detach the event listeners
    public void OnDisable()
    {
        CharacterCreationView._OnSexChanged -= MakeGenderSounds;
    }

    private void Start()
    {
        jukebox = new AudioClip[] { evad_myopic_horses,
                                    evad_death_washing_machine,
                                    eval_dope_life,
                                    evad_stars,
                                    evad_valley_of_winds,
                                    ericd_is_it_better,
                                    ashleyd_summer_stream,
                                    ericd_death_or_glory,
                                    waterStream
                                  };
        PlayJukeboxSong(2);
        AudioSource.loop = true;
    }

    public void PlayJukeboxSong(int songIndex)
    {
        if (AudioSource.isPlaying)
        {
            AudioSource.Stop();
        }
        AudioSource.loop = false;
        AudioSource.PlayOneShot(jukebox[songIndex], volume);
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
