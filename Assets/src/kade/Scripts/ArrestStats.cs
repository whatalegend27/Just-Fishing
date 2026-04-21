using UnityEngine;

public class ArrestStats : MonoBehaviour, IRiskReducible
{
   public PlayerStats ps;
   private inGameTime gameTime;
   public int riskVal;

   // Initializes risk and subscribes to the nightfall event
   void Start()
   {
      riskVal = 0;
      gameTime = FindAnyObjectByType<inGameTime>();

      if ( gameTime != null )
      {
         gameTime.OnNightfall += onNightfall;
      }
   }

   // Unsubscribes from the nightfall event on destroy
   void OnDestroy()
   {
      if ( gameTime != null )
      {
         gameTime.OnNightfall -= onNightfall;
      }
   }

   // Triggers night fish risk when nightfall event fires
   private void onNightfall()
   {
      calculateRisk( "nightFish" );
   }


   // Reduces risk by a given amount, clamped to 0
   public void ReduceRisk( int amount )
   {
      riskVal = Mathf.Max( 0, riskVal - amount );
   }

   // Resets risk to its starting value
   public void resetStats()
   {
      riskVal = 0;
   }

   /* Calculates risk based on the given action.
      Valid values: "steal", "nightFish", "blackMarket" */
   public void calculateRisk( string action )
   {
      IStatCalculator calculator = new BaseStatCalculator();

      switch ( action )
      {
         case "steal":       calculator = new StealRiskDecorator( calculator ); break;
         case "nightFish":   calculator = new NightFishRiskDecorator( calculator ); break;
         case "blackMarket": calculator = new BlackMarketRiskDecorator( calculator ); break;
      }

      riskVal = calculator.calculate( riskVal );

      if ( riskVal >= 100 )
      {
         if ( ps != null ) ps.gameOver = true;
         riskVal = 0;
      }
   }
}
