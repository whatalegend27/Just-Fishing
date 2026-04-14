using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image image;
    [SerializeField] private Button sellButton;
    [SerializeField] private TextMeshProUGUI sellPrice;
    private ItemScript currentItem;

    //Inventory description items
    [SerializeField] private GameObject slotDescription;
    [SerializeField] private Image slotImage;

    //Displays the item picture in a inventory slot
    public void SetUp(InventorySlotData slots)
    {

        if (slots.item == null)
        {
            currentItem = null;
            quantityText.enabled = false;
            quantityText.text = "";
            image.sprite = null;
            image.color = new Color(0.8f, 0.8f, 0.8f);
        }
        else
        {
            quantityText.enabled = false;
            currentItem = slots.item;
          //  quantityText.enabled = true;
          //  quantityText.text = slots.quantity.ToString();
            image.sprite = currentItem.Icon;
            

            //activates sell button panel when player clicks on time
            Button btn = GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnClick);
            }
        }
    }

    //sets sell button panel to be true
    void OnClick()
    {
        if (currentItem == null)
        {
            return;
        }
        slotImage.sprite = currentItem.Icon;
        slotDescription.SetActive(true);
        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => GoldManager.Instance.SellItem(currentItem));
        sellPrice.text = "Sells for: $" + currentItem.Price;
    }
}
