using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThunderController : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> thunderSounds;
    [SerializeField] private float minVolume = 0.4f;
    [SerializeField] private float maxVolume = 0.8f;

    [Header("Timing Settings")]
    [SerializeField] private float minTimeBetweenStrikes = 10f;
    [SerializeField] private float maxTimeBetweenStrikes = 45f;

    private WeatherController weatherScript;
    private bool isThunderRoutineRunning = false;

    void Start()
    {
        GameObject weatherObject = GameObject.Find("WeatherController");
        if (weatherObject != null)
        {
            weatherScript = weatherObject.GetComponent<WeatherController>();
        }

        StartCoroutine(ThunderRoutine());
    }

    IEnumerator ThunderRoutine()
    {
        isThunderRoutineRunning = true;

        while (true)
        {
            if (weatherScript != null && (weatherScript.GetCurrentWeather() == "Stormy"))
            {
                float waitTime = Random.Range(minTimeBetweenStrikes, maxTimeBetweenStrikes);
                yield return new WaitForSeconds(waitTime);

                PlayRandomThunder();
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    void PlayRandomThunder()
    {
        if (thunderSounds.Count > 0 && audioSource != null)
        {
            AudioClip clip = thunderSounds[Random.Range(0, thunderSounds.Count)];
            
            audioSource.volume = Random.Range(minVolume, maxVolume);
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            
            audioSource.PlayOneShot(clip);
        }
    }
}