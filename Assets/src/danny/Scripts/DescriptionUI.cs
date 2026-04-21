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
    [SerializeField] private Button buyButton;
    private ItemScript currentItem;

    //Displays scriptable objects info
    public void ShowDetails(ItemScript item)
    {
        nameText.text = item.ItemName;
        descriptionText.text = item.ItemDescription;
        priceText.text = "Cost: $" + item.Price;
        itemIcon.sprite = item.Icon;
        descriptionPanel.SetActive(true);
        currentItem = item;

        //if non-stackable in inventory already, disable buy button to prevent multiple buys
        if (!item.CanStack())
        {
            bool hasItem = false;
            foreach (var Slot in InventoryManager.Instance.slots)
            {
                if (Slot.item == item)
                {
                    hasItem = true;
                    break;
                }
            }
            buyButton.interactable = !hasItem;
        } else
        {
            buyButton.interactable = true;
        }
    }

    public void ExitButtonClicked()
    {
        descriptionPanel.SetActive(false);
    }

    public void BuyButtonClicked()
    {
        bool success = GoldManager.Instance.BuyItem(currentItem); 

        //grays out buy button immediatley after non-stackable item is bought
        if (success && !currentItem.CanStack())
        {
            buyButton.interactable = false;
        }
    }
}


/*Class Diagrams:

Singleton:
singleton has static instane with arrow pointing to itself

--------
| singleton |
| ______   |
| instance|
---------

Private Data Class:
main class diamonds to line arrow to data class
private variable is of data class type

private variables are in data class
getters in data class

*/