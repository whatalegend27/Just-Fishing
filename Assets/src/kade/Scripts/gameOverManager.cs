using UnityEngine;
using UnityEngine.SceneManagement;

public class gameOverManager : MonoBehaviour
{
    public void LoadMainMenu()
    {
        Debug.Log("[gameOverManager] LoadMainMenu called");
        SceneManager.LoadScene("MainMenu");
    }
}
