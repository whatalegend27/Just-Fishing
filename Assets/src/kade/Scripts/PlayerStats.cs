using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
   public bool gameOver;

   // Persists this GameObject across scene loads
   void Awake()
   {
      DontDestroyOnLoad( gameObject );
   }

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
