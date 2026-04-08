using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("Length of a full in-game day in real-world seconds.")]
    // Public for testing purposes. Will be made private in a final build.
    // 1440 seconds = 24 minutes = 1 real-world second = 1 in-game minute
    public float dayDuration = 1440f; 
    
    private float timeElapsed = 0f;

    [Header("Color Shifting")]
    [Tooltip("Sets the colors for midnight (0.0), sunrise (0.25), noon (0.5), sunset (0.75), and midnight (1.0).")]
    public Gradient timeColors;

    [Header("Target Rendering")]
    [Tooltip("The SpriteRenderer of your sky/background.")]
    public SpriteRenderer[] backgroundRenderers;

    [Header("Sun Movement")]
    public Transform sunTransform;
    [Tooltip("Where the sun starts.")]
    public Transform sunrisePoint;
    [Tooltip("Where the sun ends.")]
    public Transform sunsetPoint;
    [Tooltip("Controls the height of the sun's arc throughout the day.")]
    public AnimationCurve sunArcHeight;

    [Header("Moon Movement")]
    public Transform moonTransform;
    [Tooltip("Controls the height of the moon's arc throughout the night.")]
    public AnimationCurve moonArcHeight;

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

        if (sunTransform != null && sunrisePoint != null && sunsetPoint != null)
        {
            float currentX = Mathf.Lerp(sunrisePoint.position.x, sunsetPoint.position.x, timePercentage);
            float baseY = Mathf.Lerp(sunrisePoint.position.y, sunsetPoint.position.y, timePercentage);
            float heightOffset = sunArcHeight.Evaluate(timePercentage);
            sunTransform.position = new Vector3(currentX, baseY + heightOffset, sunTransform.position.z);
        }

        if (moonTransform != null && sunrisePoint != null && sunsetPoint != null)
        {
            // Offset the time by half a day so the moon is opposite the sun
            float moonTimePercentage = (timePercentage + 0.5f) % 1f;

            float currentX = Mathf.Lerp(sunrisePoint.position.x, sunsetPoint.position.x, moonTimePercentage);
            float baseY = Mathf.Lerp(sunrisePoint.position.y, sunsetPoint.position.y, moonTimePercentage);
            float heightOffset = moonArcHeight.Evaluate(moonTimePercentage);
            
            moonTransform.position = new Vector3(currentX, baseY + heightOffset, moonTransform.position.z);
        }
    }
}