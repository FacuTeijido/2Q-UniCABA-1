/*
 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com
 Creation Date: 2026-10-12 
*/
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;

    // Singleton pattern implementation to ensure only one instance of SoundMixerManager exists
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SetMasterVolume(float volume)
    {
       // audioMixer.SetFloat("MasterVolume", volume);
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20); // Convert linear volume to decibels
    }

    public void SetSoundFXVolume(float volume)
    {
        //audioMixer.SetFloat("SoundFXVolume", volume);
        audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(volume) * 20); // Convert linear volume to decibels
    }

    public void SetMusicVolume(float volume)
    {
       // audioMixer.SetFloat("MusicVolume", volume);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20); // Convert linear volume to decibels
    }
}
