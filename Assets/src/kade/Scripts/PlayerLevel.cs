using UnityEngine;
using System.Collections.Generic;

public class PlayerLevel : MonoBehaviour
{
   public int level = 1;
   public int currentXP = 0;

   public event System.Action<int> OnLevelUp;

   // XP required to reach the next level, increases by 100 each level
   public int xpToNextLevel => level * 100;

   // XP awarded per fish type
   private static readonly Dictionary<string, int> sFishXP = new()
   {
      { "CatFish",       10 },
      { "Nemo",          20 },
      { "OrangeFish",    15 },
      { "ButterflyFish", 25 },
      { "SilverFish",    20 },
      { "SkellyFish",    30 },
      { "BigBruce",      50 },
   };

   // Subscribes to the fish registered event
   void OnEnable()
   {
      FishDatabaseManager.OnFishRegistered += handleFishCaught;
   }

   // Unsubscribes from the fish registered event
   void OnDisable()
   {
      FishDatabaseManager.OnFishRegistered -= handleFishCaught;
   }

   // Awards XP based on which fish was caught
   private void handleFishCaught( string fishName )
   {
      int xp = sFishXP.TryGetValue( fishName, out int val ) ? val : 10;
      Debug.Log( $"[PlayerLevel] Caught {fishName} — +{xp} XP" );
      addXP( xp );
   }

   // Adds XP and handles leveling up
   public void addXP( int amount )
   {
      currentXP += amount;

      while ( currentXP >= xpToNextLevel )
      {
         currentXP -= xpToNextLevel;
         level++;
         OnLevelUp?.Invoke( level );
         Debug.Log( $"[PlayerLevel] Leveled up! Now level {level}" );
      }
   }
}
