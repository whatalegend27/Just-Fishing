using UnityEngine;

public class SharkFightManager : MonoBehaviour
{
   public HealthStats healthStats;

   // Deals one hit of damage to the player when the shark attacks
   public void onSharkAttack()
   {
      if ( healthStats == null ) return;

      healthStats.calculateHealth( "hurt" );
      Debug.Log( $"[SharkFightManager] Shark attacked — health now {healthStats.healthVal}" );
   }
}
