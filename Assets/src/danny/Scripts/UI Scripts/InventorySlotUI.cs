using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
   public TextMeshProUGUI quantityText;
   public Image image;

   public void SetUp(InventorySlotData slots)
    {
        if (slots.item == null)
        {
            quantityText.enabled = false;
            quantityText.text = "";
            
        } else
        {
            quantityText.enabled = true;
            image.enabled = true;
            quantityText.text = slots.quantity.ToString();
            image.sprite = slots.item.icon;
        }
    }
}
