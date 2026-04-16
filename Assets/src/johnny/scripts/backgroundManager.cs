using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("Length of a full in-game day in real-world seconds.")]
    [SerializeField] private float dayDuration = 1440f; 
    
    private float timeElapsed = 6f;

    [Header("Color Shifting")]
    [Tooltip("Sets the colors for midnight (0.0), sunrise (0.25), noon (0.5), sunset (0.75), and midnight (1.0).")]
    [SerializeField] private Gradient timeColors;
    [SerializeField] private Gradient backdropColors;

    [Header("Rendering")]
    [Tooltip("The SpriteRenderer of the sky.")]
    [SerializeField] private SpriteRenderer[] backgroundRenderers;
    [SerializeField] private SpriteRenderer backdropRenderer;
    [SerializeField] private ParticleSystem rainRenderer;

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

    private WeatherController weatherControllerScript;
    private GameObject cloudsFront;
    private GameObject cloudsBack;

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

        cloudsFront = GameObject.Find("CloudsFront");
        cloudsBack = GameObject.Find("CloudsBack");
        
        if (rainSound != null) rainSound.volume = 0f; // Start with rain sound muted
        
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        float currentTimeInDay = timeElapsed % dayDuration;
        float timePercentage = currentTimeInDay / dayDuration;

        Color currentColor = timeColors.Evaluate(timePercentage);
        foreach (SpriteRenderer renderer in backgroundRenderers)
        {
            if (renderer != null)
            {
                renderer.color = currentColor;
            }
        }

        if (backdropRenderer != null)
        {
            backdropRenderer.color = backdropColors.Evaluate(timePercentage);
        }

        if (weatherControllerScript != null && weatherControllerScript.GetCurrentWeather() == "Sunny")
        {
            // Brighten the colors during sunny weather
            foreach (SpriteRenderer renderer in backgroundRenderers)
            {
                if (renderer != null)
                {
                    renderer.color = currentColor * 1.2f;
                }
            }
            
            if (cloudsFront != null) cloudsFront.SetActive(false);
            if (cloudsBack != null) cloudsBack.SetActive(false);

            

            if (rainRenderer != null && rainRenderer.emission.enabled == true)
            {
                var emission = rainRenderer.emission;
                emission.enabled = false;        
                if (rainSound != null) rainSound.volume = 0f;    
            }
        }

        if (weatherControllerScript != null && weatherControllerScript.GetCurrentWeather() == "Cloudy")
        {
            // Slightly darken the colors during cloudy weather
            foreach (SpriteRenderer renderer in backgroundRenderers)
            {
                if (renderer != null)
                {
                    renderer.color = currentColor * 0.9f;
                }
            }
            
            if (cloudsFront != null) cloudsFront.SetActive(true);
            if (cloudsBack != null) cloudsBack.SetActive(true);

            if (rainRenderer != null && rainRenderer.emission.enabled == true)
            {
                var emission = rainRenderer.emission;
                emission.enabled = false;
                rainSound.volume = 0f;
            }
        }

        if (weatherControllerScript != null && (weatherControllerScript.GetCurrentWeather() == "Rainy" || weatherControllerScript.GetCurrentWeather() == "Stormy"))
        {
            // Darken the colors during rainy weather
            foreach (SpriteRenderer renderer in backgroundRenderers)
            {
                if (renderer != null)
                {
                    renderer.color = currentColor * 0.8f;
                }
            }
            
            if (cloudsFront != null) cloudsFront.SetActive(true);
            if (cloudsBack != null) cloudsBack.SetActive(true);
            
            if (rainSound != null) rainSound.volume = 0.5f;

            if (rainRenderer != null && rainRenderer.emission.enabled == false)
            {
                var emission = rainRenderer.emission;
                emission.enabled = true;
                rainSound.volume = 0.5f;

            }

        }

        if (sunTransform != null && sunrisePoint != null && sunsetPoint != null)
        {
            float currentX = Mathf.Lerp(sunrisePoint.position.x, sunsetPoint.position.x, timePercentage);
            float baseY = Mathf.Lerp(sunrisePoint.position.y, sunsetPoint.position.y, timePercentage);
            float heightOffset = sunArcHeight.Evaluate(timePercentage);
            sunTransform.position = new Vector3(currentX, baseY + heightOffset, sunTransform.position.z);
        }
    }
}