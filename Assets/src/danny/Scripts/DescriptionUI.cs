using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DescriptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private Image itemIcon;
    [SerializeField] private ItemScript currentItem;

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
        bool canBuy = GoldManager.Instance.BuyItem(currentItem);
    }
}
