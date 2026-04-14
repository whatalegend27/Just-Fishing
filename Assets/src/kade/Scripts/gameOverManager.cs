using UnityEngine;
using UnityEngine.SceneManagement;

public class gameOverManager : MonoBehaviour
{
   // Loads the main menu scene when called
   public void loadMainMenu()
   {
      Debug.Log( "[gameOverManager] loadMainMenu called" );
      SceneManager.LoadScene( "MainMenu" );
   }
}
