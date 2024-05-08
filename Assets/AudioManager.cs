using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip background;
    public AudioClip click;
    public AudioClip start;
    public AudioClip crash_chicken;
    public AudioClip crash_rabbit;
    public AudioClip run;
    public AudioClip jump;
    public AudioClip cheer;
    public AudioClip victory;

    // Start is called before the first frame update
    void Start()
    {
        // musicSource.clip = background;
        // musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        if (SFXSource.isPlaying)
        {
            SFXSource.Stop();
        }
    }
}
