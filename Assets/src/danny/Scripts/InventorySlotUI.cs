using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI quantityText;
   [SerializeField] private Image image;

   //Displays the item picture in a inventory slot
   public void SetUp(InventorySlotData slots)
    {
        if (slots.item == null)
        {
            quantityText.enabled = false;
            quantityText.text = "";
            
        } else
        {
            quantityText.enabled = true;
            quantityText.text = slots.quantity.ToString();
            image.sprite = slots.item.icon;
        }
    }
}
