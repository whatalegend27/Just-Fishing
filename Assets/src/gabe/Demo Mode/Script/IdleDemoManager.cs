using UnityEngine;
using UnityEngine.SceneManagement;

public class IdleDemoManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameplaySceneName = "FishingScene";
    public string demoSceneName = "DemoScene";

    [Header("Idle Settings")]
    public float idleTimeBeforeDemo = 120f;

    private float idleTimer;

    void Update()
    {
        bool playerInput =
            Input.anyKeyDown ||
            Input.GetMouseButtonDown(0) ||
            Input.GetMouseButtonDown(1);

        string currentScene = SceneManager.GetActiveScene().name;

        // =========================
        // DEMO SCENE LOGIC
        // =========================
        if (currentScene == demoSceneName)
        {
            if (playerInput)
            {
                SceneManager.LoadScene(gameplaySceneName);
            }

            return;
        }

        // =========================
        // NORMAL GAMEPLAY LOGIC
        // =========================
        if (playerInput)
        {
            idleTimer = 0f;
        }
        else
        {
            idleTimer += Time.deltaTime;
        }

        if (idleTimer >= idleTimeBeforeDemo)
        {
            SceneManager.LoadScene(demoSceneName);
        }
    }
}