using UnityEngine;

public class HealthStats : MonoBehaviour
{
   public int healthVal;
   public int hungerVal;
   public PlayerStats ps;
   public inGameTime gameTime;

   private int mLastHour;

   // Initializes health and hunger and subscribes to the new day event
   void Start()
   {
      healthVal = 100;
      hungerVal = 100;
      mLastHour = gameTime != null ? gameTime.hours : -1;

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

   // Triggers game over if hunger or health reaches zero
   void Update()
   {
      if ( hungerVal <= 0 || healthVal <= 0 )
      {
         ps.gameOver = true;
      }
   }

   /* Calculates health based on the given action.
      Valid values: "hurt" */
   public void calculateHealth( string action )
   {
      IStatCalculator calculator = new BaseStatCalculator();

      switch ( action )
      {
         case "hurt": calculator = new HurtHealthDecorator( calculator ); break;
      }

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
