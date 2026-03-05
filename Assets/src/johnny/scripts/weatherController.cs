using UnityEngine;

public class weatherController : MonoBehaviour
{
    private readonly string[] weatherConditions = { "Sunny", "Cloudy", "Rainy", "Stormy" };
    public string currentWeather;

    private void Start()
    {
        ChangeWeather();
    }

    public void ChangeWeather()
    {
        // Randomly select a new weather condition
        int randomIndex = Random.Range(0, weatherConditions.Length);
        currentWeather = weatherConditions[randomIndex];

        Debug.Log("Weather changed to: " + currentWeather);
    }
}
