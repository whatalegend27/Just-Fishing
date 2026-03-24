using UnityEngine;
using UnityEngine.SceneManagement;
public class StartController : MonoBehaviour
{
    public void LoadGameMenu()
    {
        SceneManager.LoadScene("Home");
    }
}
