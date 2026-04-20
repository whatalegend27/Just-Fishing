using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
   public bool gameOver;

   public static PlayerStats Instance { get; private set; }

   private bool mLoading = false;

   // Persists this GameObject across scene loads; destroys duplicates
   void Awake()
   {
      if ( Instance != null && Instance != this )
      {
         Destroy( gameObject );
         return;
      }

      Instance = this;
      DontDestroyOnLoad( gameObject );
      SceneManager.sceneLoaded += OnSceneLoaded;
   }

   void OnDestroy()
   {
      SceneManager.sceneLoaded -= OnSceneLoaded;
   }

   // Initializes game over state to false
   void Start()
   {
      gameOver  = false;
      mLoading  = false;
   }

   // Loads the game over scene once; guard prevents re-entry while scene is transitioning
   void Update()
   {
      if ( gameOver && !mLoading )
      {
         mLoading  = true;
         gameOver  = false;
         SceneManager.LoadScene( "GameOver" );
      }
   }

   // Clears the game over state and loading guard so a new game can trigger it again
   public void ResetForNewGame()
   {
      gameOver = false;
      mLoading = false;
   }

   // Resets loading guard only when returning to a non-GameOver scene
   private void OnSceneLoaded( Scene scene, LoadSceneMode mode )
   {
      if ( scene.name != "GameOver" )
      {
         mLoading = false;
         gameOver = false;
      }
   }
}
