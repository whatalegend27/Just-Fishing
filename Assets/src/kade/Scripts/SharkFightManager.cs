using UnityEngine;

public class SharkFightManager : MonoBehaviour
{
   public HealthStats healthStats;

   // Finds HealthStats if not assigned, then deals damage
   void Start()
   {
      if ( healthStats == null )
      {
         healthStats = FindAnyObjectByType<HealthStats>();
      }
   }

   // Deals one hit of damage to the player when the shark attacks
   public void onSharkAttack()
   {
      if ( healthStats == null ) return;

      int damage = Random.Range( 10, 21 );
      healthStats.takeDamage( damage );
      Debug.Log( $"[SharkFightManager] Shark attacked — -{damage} HP, health now {healthStats.healthVal}" );
   }
}
