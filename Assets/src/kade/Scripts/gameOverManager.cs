using UnityEngine;
using UnityEngine.SceneManagement;

public class gameOverManager : MonoBehaviour
{
   // Resets all persistent stats and returns to the main menu
   public void loadMainMenu()
   {
      Debug.Log( "[gameOverManager] loadMainMenu called" );

      var healthStats = FindAnyObjectByType<HealthStats>();
      if ( healthStats != null ) healthStats.resetStats();

      var arrestStats = FindAnyObjectByType<ArrestStats>();
      if ( arrestStats != null ) arrestStats.resetStats();

      var playerLevel = FindAnyObjectByType<PlayerLevel>();
      if ( playerLevel != null ) playerLevel.resetStats();

      var playerStats = FindAnyObjectByType<PlayerStats>();
      if ( playerStats != null ) playerStats.ResetForNewGame();

      SceneManager.LoadScene( "MainMenu" );
   }
}
