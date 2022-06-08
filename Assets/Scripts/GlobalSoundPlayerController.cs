using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSoundPlayerController : MonoBehaviour
{
    public AudioClip glassBreakAudioClip;

    private AudioSource audioSource;

    void Awake() {audioSource = GetComponent<AudioSource>();}

    public void PlayGlassBreakSound() {
        audioSource.PlayOneShot(glassBreakAudioClip);
    }
}
