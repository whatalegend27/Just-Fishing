using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DescriptionUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;
    public GameObject descriptionPanel;
    public Image itemIcon;
    private ItemScript currentItem;

    public void ShowDetails(ItemScript item)
    {
        nameText.text = item.itemName;
        descriptionText.text = item.itemDescription;
        priceText.text = "Cost: $" + item.price;
        itemIcon.sprite = item.icon;
        descriptionPanel.SetActive(true);
        currentItem = item;
    }

    public void exitButton()
    {
        descriptionPanel.SetActive(false);
    }

    public void buyButton()
    {
        InventoryManager.Instance.AddItem(currentItem);
    }
}
