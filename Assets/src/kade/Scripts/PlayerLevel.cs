using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
   public int level = 1;
   public int currentXP = 0;

   public event System.Action<int> OnLevelUp;
   public event System.Action<int> OnXPGained;

   // XP required to reach the next level, increases by 100 each level
   public int xpToNextLevel => level * 100;


   // Runs the binding demo once on startup so it appears in the Unity console
   void Start()
   {
      FishingRewardBindingDemo.Run();
   }

   // Resets level and XP to their starting values
   public void resetStats()
   {
      level     = 1;
      currentXP = 0;
   }

   // Subscribes to the fish registered event
   void OnEnable()
   {
      FishDatabaseManager.OnFishRegistered += handleFishCaught;
      Debug.Log( "[PlayerLevel] Subscribed to OnFishRegistered" );
   }

   // Unsubscribes from the fish registered event
   void OnDisable()
   {
      FishDatabaseManager.OnFishRegistered -= handleFishCaught;
   }

   // Returns the reward object matching the fish — static type is FishingReward (dynamic type varies)
   private FishingReward getFishingReward( string fishName )
   {
      switch ( fishName )
      {
         case "OrangeFish":                          return new CommonFishReward();
         case "Nemo": case "ButterflyFish":
         case "SilverFish": case "SkellyFish":       return new RareFishReward();
         case "BigBruce":                            return new LegendaryFishReward();
         default:                                    return new FishingReward();
      }
   }

   // Awards XP based on which fish was caught — uses dynamic binding via FishingReward
   private void handleFishCaught( string fishName )
   {
      // Static type = FishingReward, dynamic type = whichever subclass getFishingReward returns
      FishingReward reward = getFishingReward( fishName );

      // Virtual dispatch — calls the overriding getXP() on the actual runtime type
      int xp = reward.award();
      Debug.Log( $"[PlayerLevel] Caught {fishName} — +{xp} XP" );
      OnXPGained?.Invoke( xp );
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
         if ( OnLevelUp != null )
            OnLevelUp.Invoke( level );
         Debug.Log( $"[PlayerLevel] Leveled up! Now level {level}" );
      }
   }
}
