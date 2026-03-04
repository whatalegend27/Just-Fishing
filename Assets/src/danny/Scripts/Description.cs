using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Description : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;
    public GameObject descriptionPanel;
    public Image itemIcon;

    public void ShowDetails(ItemScript item)
    {
        nameText.text = item.itemName;
        descriptionText.text = item.itemDescription;
        priceText.text = "Cost: $" + item.price;
        itemIcon.sprite = item.icon;
        descriptionPanel.SetActive(true);
    }

    public void exitButton()
    {
        descriptionPanel.SetActive(false);
    }
}
