using UnityEngine;

public class HealButton : MonoBehaviour
{
   private HealthStats mHealthStats;

   void Awake()
   {
      mHealthStats = FindAnyObjectByType<HealthStats>();
   }

   // Called by the button's OnClick event
   public void onHeal()
   {
      if ( mHealthStats == null ) return;

      mHealthStats.Heal();
   }
}
