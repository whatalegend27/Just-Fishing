using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image image;
    [SerializeField] private GameObject slotDescription;
    [SerializeField] private Button sellButton;
    [SerializeField] private TextMeshProUGUI sellPrice;
    private ItemScript currentItem;

    //Displays the item picture in a inventory slot
    public void SetUp(InventorySlotData slots)
    {

        if (slots.item == null)
        {
            quantityText.enabled = false;
            quantityText.text = "";
            image.sprite = null;
            image.color = new Color(0.8f, 0.8f, 0.8f);
        }
        else
        {
            currentItem = slots.item;
            quantityText.enabled = true;
            quantityText.text = slots.quantity.ToString();
            image.sprite = slots.item.Icon;

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
        slotDescription.SetActive(true);
        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => GoldManager.Instance.SellItem(currentItem));
        sellPrice.text = "Sells for: $" + currentItem.Price;
    }
}
