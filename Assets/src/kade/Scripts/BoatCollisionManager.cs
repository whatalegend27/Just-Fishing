using UnityEngine;

public class BoatCollisionHandler : MonoBehaviour
{
   public HealthStats healthStats;

   void OnCollisionEnter2D( Collision2D collision )
   {
      if ( collision.gameObject.CompareTag( "Rock" ) )
      {
         if ( healthStats == null )
         {
            healthStats = FindAnyObjectByType<HealthStats>();
         }

         rock rockComponent = collision.gameObject.GetComponent<rock>();
         int damage;
         if ( rockComponent != null )
         {
            damage = rockComponent.DamageAmount;
         }
         else
         {
            damage = Random.Range( 5, 11 );
         }
         healthStats.takeDamage( damage );
         Debug.Log( $"[BoatCollisionHandler] Hit a rock — -{damage} HP, health now {healthStats.healthVal}" );
      }
   }
}
