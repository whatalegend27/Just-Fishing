using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour
{
    public ItemScript itemData;
    public Description displayUI;
    public Image iconImage;

    void Start()
    {
        if (itemData != null && iconImage != null)
        {
            iconImage.sprite = itemData.icon;
        }

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        displayUI.ShowDetails(itemData);
    }
}
