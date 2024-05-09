using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource p1SFXSource;
    [SerializeField] AudioSource p2SFXSource;

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

    public void PlaySFXP1(AudioClip clip)
    {
        p1SFXSource.PlayOneShot(clip);
    }

    public void PlaySFXP2(AudioClip clip)
    {
        p2SFXSource.PlayOneShot(clip);
    }
}
