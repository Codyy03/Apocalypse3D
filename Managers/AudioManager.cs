using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] AudioClip clickSound;
    AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }
        
    public void PlayClip(AudioClip clip) => audioSource.PlayOneShot(clip);
    public void PlayClickSound() => PlayClip(clickSound);
}
