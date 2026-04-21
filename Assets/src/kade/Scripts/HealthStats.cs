using UnityEngine;

public class HealthStats : MonoBehaviour, IHealable, IDamageable
{
   public int healthVal;
   public int hungerVal;
   public PlayerStats ps;
   private inGameTime gameTime;

   private int mLastHour;
   private int mHungerTickHour;

   // Initializes health and hunger and subscribes to the new day event
   void Start()
   {
      healthVal = 100;
      hungerVal = 100;
      gameTime = FindAnyObjectByType<inGameTime>();

      if ( gameTime != null )
         mLastHour = gameTime.hours;
      else
         mLastHour = 0;
      mHungerTickHour = mLastHour;

      if ( gameTime != null )
      {
         gameTime.OnNewDay += onNewDay;
      }
   }

   // Unsubscribes from the new day event on destroy
   void OnDestroy()
   {
      if ( gameTime != null )
      {
         gameTime.OnNewDay -= onNewDay;
      }
   }

   // Resets health to full at the start of a new day
   private void onNewDay()
   {
      healthVal = 100;
   }

   // Decreases hunger by 5 every 8 in-game hours and triggers game over if stats hit zero
   void Update()
   {
      if ( gameTime != null && gameTime.hours != mLastHour )
      {
         mLastHour = gameTime.hours;

         int hoursSinceTick = ( mLastHour - mHungerTickHour + 24 ) % 24;
         if ( hoursSinceTick >= 8 )
         {
            calculateHunger( "time" );
            mHungerTickHour = mLastHour;
         }
      }

      if ( hungerVal <= 0 || healthVal <= 0 )
      {
         ps.gameOver = true;
      }
   }

   /* Calculates health based on the given action.
      Valid values: "hurt", "heal" */
   public void calculateHealth( string action )
   {
      IStatCalculator calculator = new BaseStatCalculator();

      switch ( action )
      {
         case "hurt": calculator = new HurtHealthDecorator( calculator ); break;
         case "heal": calculator = new HealHealthDecorator( calculator ); break;
      }

      healthVal = Mathf.Clamp( calculator.calculate( healthVal ), 0, 100 );
   }

   // Heals the player by 25, capped at 100
   public void Heal()
   {
      calculateHealth( "heal" );
   }

   // Resets health and hunger to their starting values
   public void resetStats()
   {
      healthVal = 100;
      hungerVal = 100;
   }

   // IDamageable implementation — forwards to takeDamage
   public void TakeDamage( int amount ) => takeDamage( amount );

   // Applies a specific amount of damage directly to health
   public void takeDamage( int amount )
   {
      IStatCalculator calculator = new TakeDamageDecorator( new BaseStatCalculator(), amount );
      healthVal = Mathf.Clamp( calculator.calculate( healthVal ), 0, 100 );
   }

   /* Calculates hunger based on the given action.
      Valid values: "eat", "time" */
   public void calculateHunger( string action )
   {
      IStatCalculator calculator = new BaseStatCalculator();

      switch ( action )
      {
         case "eat":  calculator = new EatHungerDecorator( calculator ); break;
         case "time": calculator = new TimeHungerDecorator( calculator ); break;
      }

      hungerVal = Mathf.Clamp( calculator.calculate( hungerVal ), 0, 100 );
   }
}
