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
    private ItemScript currentItem;

    //Displays scriptable objects info
    public void ShowDetails(ItemScript item)
    {
        nameText.text = item.ItemName;
        descriptionText.text = item.getDescription();
        priceText.text = "Cost: $" + item.Price;
        itemIcon.sprite = item.Icon;
        descriptionPanel.SetActive(true);
        currentItem = item;
    }

    public void exitButton()
    {
        descriptionPanel.SetActive(false);
    }

    public void buyButton()
    {
        GoldManager.Instance.BuyItem(currentItem); 
    }
}
