using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [Header("Referencje")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider volumeSliderSounds;
    [SerializeField] Slider volumeSliderMusic;

    [Header("Parametry w Audio Mixerze")]
    public string exposedParameterSounds = "Sounds";
    public string exposedParameterMusic = "Music";

    readonly float minDb = -40f;
    readonly float maxDb = 0f;
    readonly float muteDb = -80f;

    private void OnEnable()
    {
        // Wczytaj i ustaw g³oœnoœæ dla efektów
        if (volumeSliderSounds)
        {
            float soundsValue = PlayerPrefs.GetFloat(exposedParameterSounds, 1f);
            volumeSliderSounds.SetValueWithoutNotify(soundsValue);
            ApplyToMixer(exposedParameterSounds, soundsValue);
            volumeSliderSounds.onValueChanged.AddListener(OnSoundsChanged);
        }

        // Wczytaj i ustaw g³oœnoœæ dla muzyki
        if (volumeSliderMusic)
        {
            float musicValue = PlayerPrefs.GetFloat(exposedParameterMusic, 1f);
            volumeSliderMusic.SetValueWithoutNotify(musicValue);
            ApplyToMixer(exposedParameterMusic, musicValue);
            volumeSliderMusic.onValueChanged.AddListener(OnMusicChanged);
        }
    }

    private void OnDisable()
    {
        // Od³¹cz listenery, aby unikn¹æ dublowania
        if (volumeSliderSounds)
            volumeSliderSounds.onValueChanged.RemoveListener(OnSoundsChanged);

        if (volumeSliderMusic)
            volumeSliderMusic.onValueChanged.RemoveListener(OnMusicChanged);
    }

    private void OnSoundsChanged(float value)
    {
        ApplyToMixer(exposedParameterSounds, value);
        PlayerPrefs.SetFloat(exposedParameterSounds, value);
        PlayerPrefs.Save();
    }

    private void OnMusicChanged(float value)
    {
        ApplyToMixer(exposedParameterMusic, value);
        PlayerPrefs.SetFloat(exposedParameterMusic, value);
        PlayerPrefs.Save();
    }

    private void ApplyToMixer(string parameter, float value01)
    {
        float db = value01 <= 0.0001f ? muteDb : Mathf.Lerp(minDb, maxDb, value01);
        audioMixer.SetFloat(parameter, db);
    }
}