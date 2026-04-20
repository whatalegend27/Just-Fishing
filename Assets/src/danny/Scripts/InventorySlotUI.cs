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
    private int currentItemQuantity;

    //Inventory description items
    [SerializeField] private GameObject slotDescription;
    [SerializeField] private Image slotImage;
    [SerializeField] private GameObject eatButton;

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
            return;
        }
        currentItem = slots.item;
        currentItemQuantity = slots.quantity;

        if (!currentItem.CanStack())
        {
            quantityText.enabled = false;
        }

        if (currentItem.CanStack())
        {
            quantityText.enabled = true;
            quantityText.text = slots.quantity.ToString();
        }

        image.sprite = currentItem.Icon;
        //activates invent description panel when item clicked
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }

    }

    //sets invent description panel to be true
    void OnClick()
    {
        if (currentItem == null)
        {
            return;
        }
        
        if (currentItem.Type == ItemScript.ItemType.Food)
        {
            eatButton.SetActive(true);
        }
        else
        {
            eatButton.SetActive(false);
        }

        slotImage.sprite = currentItem.Icon;
        slotDescription.SetActive(true);
        sellButton.onClick.RemoveAllListeners();
        sellPrice.text = "Sells for: $" + currentItem.Price;

        //calls sellitem function in goldmanager and disables invent description if last
        sellButton.onClick.AddListener(() =>
        {
            if (currentItemQuantity <= 1) // last one
            {
                slotDescription.SetActive(false);
            }
            GoldManager.Instance.SellItem(currentItem);
        });
    }
}
