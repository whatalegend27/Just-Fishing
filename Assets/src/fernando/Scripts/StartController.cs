using UnityEngine;
using UnityEngine.SceneManagement;
public class StartController : MonoBehaviour
{
    // Loads the Home scene when called
    public void LoadGameMenu()
    {
        SceneManager.LoadScene("Home");
    }
}
