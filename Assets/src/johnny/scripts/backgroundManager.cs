using System;
using UnityEngine;

/* This script manages the background visuals, including color shifts based on time of day and weather conditions, as well as sun movement and rain effects. 

It uses a private data class to store all relevant settings, making it easy to adjust parameters. 

It also contains an instance of a singleton pattern*/
public class BackgroundManager : MonoBehaviour
{
    // Serialized private data class instance
    [SerializeField] private BackgroundManagerData settings;
    
    // Reference to the WeatherController script to check current weather conditions
    private WeatherController weatherControllerScript;

    // Time of day starts at 6 AM
    private float timeElapsed = 6f;

    // Singleton instance
    public static BackgroundManager Instance { get; private set; }

    void Awake()
    {
        // Implement singleton pattern. Ensures only one pattern exists
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        // Binds the WeatherController script to check for weather conditions
        GameObject weatherControllerObject = GameObject.Find("WeatherController");
        if (weatherControllerObject != null)
        {
            weatherControllerScript = weatherControllerObject.GetComponent<WeatherController>();
        }
        
        // Start with rain effects disabled
        if (settings.RainSound != null) 
        {
            settings.RainSound.volume = 0f;
        }
    }

    void Update()
    {
        // Update the time of day
        timeElapsed += Time.deltaTime;

        // Calculate values for color shifting and sun movement
        float currentTimeInDay = timeElapsed % settings.DayDuration;
        float timePercentage = currentTimeInDay / settings.DayDuration;

        // Update background colors. Gradient is determined using Unity's built-in editor.
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

        // Adjust visuals based on weather conditions
        if (weatherControllerScript != null)
        {
            if (weatherControllerScript.GetCurrentWeather() == "Sunny")
            {
                // Brighten the colors during sunny weather
                foreach (SpriteRenderer renderer in settings.BackgroundRenderers)
                {
                    if (renderer != null)
                    {
                        renderer.color = currentColor * 1.2f;
                    }
                }
                
                // Disable clouds
                EnableClouds(false);
                
                // Disable rain and rain sounds, if enabled
                if (settings.RainRenderer != null && settings.RainRenderer.emission.enabled == true)
                {
                    var emission = settings.RainRenderer.emission;
                    emission.enabled = false;        
                    if (settings.RainSound != null) settings.RainSound.volume = 0f;    
                }
            } else if (weatherControllerScript.GetCurrentWeather() == "Cloudy")
            {
                // Slightly darken the colors during cloudy weather
                foreach (SpriteRenderer renderer in settings.BackgroundRenderers)
                {
                    if (renderer != null)
                    {
                        renderer.color = currentColor * 0.9f;
                    }
                }
                
                EnableClouds();
                
                // Disable rain and rain sounds, if enabled
                if (settings.RainRenderer != null && settings.RainRenderer.emission.enabled == true)
                {
                    var emission = settings.RainRenderer.emission;
                    emission.enabled = false;
                    settings.RainSound.volume = 0f;
                }
            } else if (weatherControllerScript.GetCurrentWeather() == "Rainy" ||
                        weatherControllerScript.GetCurrentWeather() == "Stormy")
            {
                // Darken the colors during rainy weather
                foreach (SpriteRenderer renderer in settings.BackgroundRenderers)
                {
                    if (renderer != null)
                    {
                        renderer.color = currentColor * 0.8f;
                    }
                }
                
                EnableClouds();
                
                // Enable rain and rain sounds, if disabled
                if (settings.RainSound != null) 
                { 
                    settings.RainSound.volume = 0.5f;
                }

                if (settings.RainRenderer != null && settings.RainRenderer.emission.enabled == false)
                {
                    var emission = settings.RainRenderer.emission;
                    emission.enabled = true;
                    settings.RainSound.volume = 0.5f;
                }

            }

            // Move the sun across the sky based on time of day. The sun's arc height is determined by an AnimationCurve.
            if (settings.SunTransform != null && settings.SunrisePoint != null && 
                settings.SunsetPoint != null && settings.SunArcHeight != null)
            {
                float currentX = Mathf.Lerp(settings.SunrisePoint.position.x, settings.SunsetPoint.position.x, timePercentage);
                float baseY = Mathf.Lerp(settings.SunrisePoint.position.y, settings.SunsetPoint.position.y, timePercentage);
                float heightOffset = settings.SunArcHeight.Evaluate(timePercentage);
                settings.SunTransform.position = new Vector3(currentX, baseY + heightOffset, settings.SunTransform.position.z);
            }
        }
    }

    // Disables or enables clouds depending on the provided boolean
    private void EnableClouds(bool mToEnable = true)
    {
        if (settings.CloudsFront != null) 
        {
            settings.CloudsFront.SetActive(mToEnable);
        }
            
        if (settings.CloudsBack != null) 
        {
            settings.CloudsBack.SetActive(mToEnable);
        }
    }
}

/* This class is used to store all the settings for the BackgroundManager script. 

It is marked as Serializable so that it can be edited in the Unity Inspector. 

It is also an implementation of a Private Data Class */
[Serializable]
public class BackgroundManagerData
{
    // Parameters for the background manager, all serialized
    [Header("Time Settings")]
    [Tooltip("Length of a full in-game day in real-world seconds. Default: 108 (6 sec per in-game hour)")]
    [SerializeField] private float dayDuration = 108f; 

    [Header("Color Shifting")]
    [Tooltip("Sets the colors for midnight (0.0), sunrise (0.25), noon (0.5), sunset (0.75), and midnight (1.0).")]
    [SerializeField] private Gradient timeColors;
    [SerializeField] private Gradient backdropColors;

    [Header("Rendering")]
    [Tooltip("The renderers for the sky, rain, and clouds.")]
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

    // Public getters for all private fields
    // Time settings
    public float DayDuration => dayDuration;

    // Color shifting
    public Gradient TimeColors => timeColors;
    public Gradient BackdropColors => backdropColors;

    // Rendering
    public SpriteRenderer[] BackgroundRenderers => backgroundRenderers;
    public SpriteRenderer BackdropRenderer => backdropRenderer;
    public ParticleSystem RainRenderer => rainRenderer;
    public GameObject CloudsFront => cloudsFront;
    public GameObject CloudsBack => cloudsBack;

    // Sun movement
    public Transform SunTransform => sunTransform;
    public Transform SunrisePoint => sunrisePoint;
    public Transform SunsetPoint => sunsetPoint;
    public AnimationCurve SunArcHeight => sunArcHeight;

    // Rain settings
    public AudioSource RainSound => rainSound;

}