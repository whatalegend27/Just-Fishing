using UnityEngine;

public class BoatCollisionHandler : MonoBehaviour
{
   public HealthStats healthStats;

   void OnCollisionEnter2D( Collision2D collision )
   {
      if ( healthStats == null )
      {
         healthStats = FindAnyObjectByType<HealthStats>();
      }

      if ( collision.gameObject.CompareTag( "Rock" ) )
      {
         int damage = Random.Range( 10, 21 );
         healthStats.takeDamage( damage );
         Debug.Log( $"[BoatCollisionHandler] Hit a rock — -{damage} HP, health now {healthStats.healthVal}" );
      }
      else if ( collision.gameObject.CompareTag( "HeavyRock" ) )
      {
         int damage = Random.Range( 20, 31 );
         healthStats.takeDamage( damage );
         Debug.Log( $"[BoatCollisionHandler] Hit a heavy rock — -{damage} HP, health now {healthStats.healthVal}" );
      }
   }
}
