using UnityEngine;
using UnityEngine.SceneManagement;

public class IdleDemoSceneLoader : MonoBehaviour
{
    [Header("Demo Scene")]
    public string demoSceneName = "DemoScene";

    [Header("Idle Time")]
    public float idleTimeBeforeDemo = 120f; // 2 minutes

    private float idleTimer;

    void Update()
    {
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            idleTimer = 0f;
            return;
        }

        idleTimer += Time.deltaTime;

        if (idleTimer >= idleTimeBeforeDemo)
        {
            SceneManager.LoadScene(demoSceneName);
        }
    }
}