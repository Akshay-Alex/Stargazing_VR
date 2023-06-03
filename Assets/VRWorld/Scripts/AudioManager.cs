using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip[] audioClips;
    public AudioSource audioSource;
    void PlayOceanSFX()
    {
        var audio = Array.Find(audioClips, audioclip => audioclip.name == "oceanSFX");
        if(audio == null)
        {
            Debug.Log("Audio not found");
        }
        else
        {
            audioSource.clip = audio;
            audioSource.Play();
        }

    }
    private void Start()
    {
        PlayOceanSFX();
    }
}
