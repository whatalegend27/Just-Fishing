using UnityEngine;

public class AchievmentDisplay : MonoBehaviour
{
    [SerializeField] private GameObject achievementUI;

    // Hides the achievement UI on startup
    void Start()
    {
        achievementUI.SetActive(false);
    }

    // Shows the achievement UI when the player enters the trigger zone
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            achievementUI.SetActive(true);
        }
    }
}
