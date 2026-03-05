using UnityEngine;
using UnityEngine.SceneManagement;

public class gameOverManager : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main-Menu");
    }
}
