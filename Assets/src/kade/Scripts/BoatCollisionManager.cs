using UnityEngine;

public class BoatCollisionHandler : MonoBehaviour
{
   public HealthStats healthStats;

   // Detects when the boat enters a trigger collider tagged as Rock
   void OnTriggerEnter2D( Collider2D other )
   {
      if ( other.CompareTag( "Rock" ) )
      {
         if ( healthStats == null )
         {
            healthStats = FindAnyObjectByType<HealthStats>();
         }

         int damage = Random.Range( 5, 11 );
         healthStats.takeDamage( damage );
         Debug.Log( $"[BoatCollisionHandler] Hit a rock — -{damage} HP, health now {healthStats.healthVal}" );
      }
   }
}
