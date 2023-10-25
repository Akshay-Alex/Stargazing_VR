using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXSoundsManager : MonoBehaviour
{
    public AudioSource audioSource;
    public static SFXSoundsManager sFXSoundsManager;
    public AudioClip buttonClickSFX;
    public AudioClip timeSkipSFX;
    private void Start()
    {
        sFXSoundsManager = this;
    }
    public void PlayButtonClickSFX()
    {
        audioSource.clip = buttonClickSFX;
        audioSource.Play();
    }
    public void PlayTimeSkipSFX()
    {
        audioSource.clip = timeSkipSFX;
        audioSource.Play();
    }
}
