using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private BackgroundManagerData settings;
    private WeatherController weatherControllerScript;
    private float timeElapsed = 6f;

    public static BackgroundManager Instance { get; private set; }

    void Awake()
    {
        // Implement singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        GameObject weatherControllerObject = GameObject.Find("WeatherController");
        if (weatherControllerObject != null)
        {
            weatherControllerScript = weatherControllerObject.GetComponent<WeatherController>();
        }
        
        if (settings.RainSound != null) 
        {
            settings.RainSound.volume = 0f; // Start with rain sound muted
        }
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        float currentTimeInDay = timeElapsed % settings.DayDuration;
        float timePercentage = currentTimeInDay / settings.DayDuration;

        Color currentColor = settings.TimeColors.Evaluate(timePercentage);
        foreach (SpriteRenderer renderer in settings.BackgroundRenderers)
        {
            if (renderer != null)
            {
                renderer.color = currentColor;
            }
        }

        if (settings.BackdropRenderer != null)
        {
            settings.BackdropRenderer.color = settings.BackdropColors.Evaluate(timePercentage);
        }

        if (weatherControllerScript != null && weatherControllerScript.GetCurrentWeather() == "Sunny")
        {
            // Brighten the colors during sunny weather
            foreach (SpriteRenderer renderer in settings.BackgroundRenderers)
            {
                if (renderer != null)
                {
                    renderer.color = currentColor * 1.2f;
                }
            }
            
            ModifyClouds(false);
            
            if (settings.RainRenderer != null && settings.RainRenderer.emission.enabled == true)
            {
                var emission = settings.RainRenderer.emission;
                emission.enabled = false;        
                if (settings.RainSound != null) settings.RainSound.volume = 0f;    
            }
        }

        if (weatherControllerScript != null && weatherControllerScript.GetCurrentWeather() == "Cloudy")
        {
            // Slightly darken the colors during cloudy weather
            foreach (SpriteRenderer renderer in settings.BackgroundRenderers)
            {
                if (renderer != null)
                {
                    renderer.color = currentColor * 0.9f;
                }
            }
            
            ModifyClouds(true);

            if (settings.RainRenderer != null && settings.RainRenderer.emission.enabled == true)
            {
                var emission = settings.RainRenderer.emission;
                emission.enabled = false;
                settings.RainSound.volume = 0f;
            }
        }

        if (weatherControllerScript != null && (weatherControllerScript.GetCurrentWeather() == "Rainy" || weatherControllerScript.GetCurrentWeather() == "Stormy"))
        {
            // Darken the colors during rainy weather
            foreach (SpriteRenderer renderer in settings.BackgroundRenderers)
            {
                if (renderer != null)
                {
                    renderer.color = currentColor * 0.8f;
                }
            }
            
            ModifyClouds(true);
            
            if (settings.RainSound != null) settings.RainSound.volume = 0.5f;

            if (settings.RainRenderer != null && settings.RainRenderer.emission.enabled == false)
            {
                var emission = settings.RainRenderer.emission;
                emission.enabled = true;
                settings.RainSound.volume = 0.5f;

            }

        }

        if (settings.SunTransform != null && settings.SunrisePoint != null && 
            settings.SunsetPoint != null && settings.SunArcHeight != null)
        {
            float currentX = Mathf.Lerp(settings.SunrisePoint.position.x, settings.SunsetPoint.position.x, timePercentage);
            float baseY = Mathf.Lerp(settings.SunrisePoint.position.y, settings.SunsetPoint.position.y, timePercentage);
            float heightOffset = settings.SunArcHeight.Evaluate(timePercentage);
            settings.SunTransform.position = new Vector3(currentX, baseY + heightOffset, settings.SunTransform.position.z);
        }

    }

    private void ModifyClouds(bool toEnable)
    {
        if (settings.CloudsFront != null) 
        {
            settings.CloudsFront.SetActive(toEnable);
        }
            
        if (settings.CloudsBack != null) 
        {
            settings.CloudsBack.SetActive(toEnable);
        }
    }
}

// Private Data Class pattern
[Serializable]
public class BackgroundManagerData
{
    [Header("Time Settings")]
    [Tooltip("Length of a full in-game day in real-world seconds. Default: 108 (6 sec per in-game hour)")]
    [SerializeField] private float dayDuration = 108f; 

    [Header("Color Shifting")]
    [Tooltip("Sets the colors for midnight (0.0), sunrise (0.25), noon (0.5), sunset (0.75), and midnight (1.0).")]
    [SerializeField] private Gradient timeColors;
    [SerializeField] private Gradient backdropColors;

    [Header("Rendering")]
    [Tooltip("The SpriteRenderer of the sky.")]
    [SerializeField] private SpriteRenderer[] backgroundRenderers;
    [SerializeField] private SpriteRenderer backdropRenderer;
    [SerializeField] private ParticleSystem rainRenderer;
    [SerializeField] private GameObject cloudsFront;
    [SerializeField] private GameObject cloudsBack;

    [Header("Sun Movement")]
    [SerializeField] private Transform sunTransform;
    [Tooltip("Where the sun starts.")]
    [SerializeField] private Transform sunrisePoint;
    [Tooltip("Where the sun ends.")]
    [SerializeField] private Transform sunsetPoint;
    [Tooltip("Controls the height of the sun's arc throughout the day.")]
    [SerializeField] private AnimationCurve sunArcHeight;

    [Header("Rain Settings")]
    [Tooltip("The rain sound controller.")]
    [SerializeField] private AudioSource rainSound;

    public float DayDuration => dayDuration;

    public Gradient TimeColors => timeColors;
    public Gradient BackdropColors => backdropColors;

    public SpriteRenderer[] BackgroundRenderers => backgroundRenderers;
    public SpriteRenderer BackdropRenderer => backdropRenderer;
    public ParticleSystem RainRenderer => rainRenderer;
    public GameObject CloudsFront => cloudsFront;
    public GameObject CloudsBack => CloudsBack;

    public Transform SunTransform => sunTransform;
    public Transform SunrisePoint => sunrisePoint;
    public Transform SunsetPoint => sunsetPoint;
    public AnimationCurve SunArcHeight => sunArcHeight;

    public AudioSource RainSound => rainSound;

}