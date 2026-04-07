using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public bool gameOver;

    void Start()
    {
        gameOver = false;
    }

    void Update()
    {
        if (gameOver == true)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
