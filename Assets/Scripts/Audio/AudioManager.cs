using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [SerializeField] AudioSource sFXPlayer;

    const float MIN_PITCH = 0.9f;
    const float MAX_PITCH = 1.1f; 

    //used for UI SFX
    public void PlaySFX(AudioData audioData){
        // sFXPlayer.clip = audioClip;
        // sFXPlayer.volume = volume;
        sFXPlayer.PlayOneShot(audioData.audioClip, audioData.volume);
    }

    //used for repeat play SFX
    public void PlayRandomSFX(AudioData audioData){
        sFXPlayer.pitch = Random.Range(MIN_PITCH, MAX_PITCH);
        PlaySFX(audioData);
    }

    public void PlayRandomSFX(AudioData[] audioData){
        PlayRandomSFX(audioData[Random.Range(0, audioData.Length)]);
    }
}

[System.Serializable] public class AudioData{
    public AudioClip audioClip;
    public float volume;
}