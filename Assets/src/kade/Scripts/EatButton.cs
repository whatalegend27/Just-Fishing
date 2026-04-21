using System;
using UnityEngine;

public class EatButton : MonoBehaviour
{
   // Assign the Bread ScriptableObject asset in the Inspector
   public ItemScript breadItem;
   [SerializeField] GameObject inventoryDescription;
   private HealthStats mHealthStats;

   void Awake()
   {
      mHealthStats = FindAnyObjectByType<HealthStats>();
   }

   // Called by the button's OnClick event
   public void onEat()
   {
      if (InventoryManager.Instance == null ||  mHealthStats == null)
      {
         Debug.Log("mHealthStat is null");
         return;
      }

      // Check the player actually has bread before consuming it
      bool hasBread = false;
      foreach (var slot in InventoryManager.Instance.slots)
      {
         if (slot.item == breadItem && slot.quantity > 0) { hasBread = true; break; }
      }
      if (!hasBread)
      {
         inventoryDescription.SetActive(false);
         return;
      }

      InventoryManager.Instance.RemoveItem(breadItem);
      mHealthStats.calculateHunger("eat");
   }
}
