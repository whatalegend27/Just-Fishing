using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour
{
    [SerializeField] private ItemScript itemData;
    [SerializeField] private DescriptionUI displayUI;
    [SerializeField] private Image iconImage;

    //Displays the image for the Scriptable Object
    void Start()
    {
        if (itemData != null && iconImage != null)
        {
            iconImage.sprite = itemData.Icon;
        }

        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnClick);
    }

    //Displays Scriptable Objects item descriptions when clicked
    void OnClick()
    {
        displayUI.ShowDetails(itemData); //from DescriptionUI
    }
}
