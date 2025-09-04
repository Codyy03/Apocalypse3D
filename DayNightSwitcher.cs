using UnityEngine;

public class DayNightSwitcher : MonoBehaviour
{
    public static DayNightSwitcher Instance;
    [Header("Materia³y skybox")]
    [SerializeField] Material daySkybox;
    [SerializeField] Material nightSkybox;

    [Header("Kolory")]
    [SerializeField] Color dayAmbient = new Color(0.576f, 0.576f, 0.576f, 1f);
    [SerializeField] Color nightAmbient = new Color(0.1137f, 0.1098f, 0.1098f, 1f);
    [SerializeField] Light directionalLight;

    [SerializeField] AudioClip yawnSound;

    AudioManager audioManager;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        audioManager = AudioManager.Instance;
    }
    /// <summary>
    /// ustawia noc w œwiecie gry
    /// </summary>
    /// <param name="yawn">czy odgrywaæ dŸwiêk ziewania</param>
    public void SetNight(bool yawn)
    {
        RenderSettings.skybox = nightSkybox;

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = nightAmbient;

        directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();

        directionalLight.intensity = 0;

        if (yawn) PlayYawnSound();

    }
    /// <summary>
    /// ustawia dzieñ w œwiecie gry
    /// </summary>
    /// <param name="yawn">czy odgrywaæ dŸwiêk ziewania</param>
    public void SetDay(bool yawn)
    {
        RenderSettings.skybox = daySkybox;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = dayAmbient;

        directionalLight.intensity = 1;

        if (yawn) PlayYawnSound();
    }

    void PlayYawnSound() => audioManager.PlayClip(yawnSound);
}
