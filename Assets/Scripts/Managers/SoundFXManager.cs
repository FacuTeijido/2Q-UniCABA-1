/*
 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com
 Creation Date: 2026-10-12 
*/using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance { get; private set; }

    [SerializeField] private AudioSource soundFXSource;

    // Singleton pattern implementation to ensure only one instance of SoundFXManager exists
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
    
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        AudioSource audioSource = Instantiate(soundFXSource, position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

     public void PlayRandomSound(AudioClip[] clip, Vector3 position, float volume = 1f)
    {
        int randomIndex = Random.Range(0, clip.Length);
        AudioSource audioSource = Instantiate(soundFXSource, position, Quaternion.identity);
        audioSource.clip = clip[randomIndex];
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = clip[randomIndex].length;
        Destroy(audioSource.gameObject, clipLength);
    }
}
