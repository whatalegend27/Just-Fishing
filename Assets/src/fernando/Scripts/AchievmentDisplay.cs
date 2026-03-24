using UnityEngine;

public class AchievmentDisplay : MonoBehaviour
{
    public GameObject achievementUI;

    void Start()
    {
        achievementUI.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Lure"))
        {
            achievementUI.SetActive(true);
        }
    }
}