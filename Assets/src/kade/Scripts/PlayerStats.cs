using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
   public bool gameOver;

   // Initializes game over state to false
   void Start()
   {
      gameOver = false;
   }

   // Loads the game over scene if game over condition is met
   void Update()
   {
      if ( gameOver == true )
      {
         SceneManager.LoadScene( "GameOver" );
      }
   }
}
