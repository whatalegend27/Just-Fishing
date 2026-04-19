using UnityEngine;

public class ArrestStats : MonoBehaviour
{
   public PlayerStats ps;
   public inGameTime gameTime;
   public int riskVal;

   // Initializes risk and subscribes to the nightfall event
   void Start()
   {
      riskVal = 0;

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

   // Triggers game over if risk reaches 100
   void Update()
   {
      if ( riskVal >= 100 )
      {
         ps.gameOver = true;
         riskVal = 0;
      }
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
   }
}
