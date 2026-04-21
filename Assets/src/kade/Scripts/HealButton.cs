using UnityEngine;

public class HealButton : MonoBehaviour
{
   public ItemScript healItem;

   private HealthStats mHealthStats;

   void Awake()
   {
      mHealthStats = FindAnyObjectByType<HealthStats>();
   }

   // Called by the button's OnClick event
   public void onHeal()
   {
      if (mHealthStats == null)
      {
         Debug.Log("mHealthStats is null");
         return;
      }

      bool hasItem = false;
      foreach (var slot in InventoryManager.Instance.slots)
      {
         if (slot.item == healItem && slot.quantity > 0) { hasItem = true; break; }
      }
      if (!hasItem) return;

      InventoryManager.Instance.RemoveItem(healItem);
      mHealthStats.Heal();
   }
}
