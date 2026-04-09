using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image image;
    [SerializeField] private GameObject slotDescription;

    //Displays the item picture in a inventory slot
    public void SetUp(InventorySlotData slots)
    {

        if (slots.item == null)
        {
            quantityText.enabled = false;
            quantityText.text = "";

        }
        else
        {
            quantityText.enabled = true;
            quantityText.text = slots.quantity.ToString();
            image.sprite = slots.item.Icon;

            //activates sell button panel when player clicks on time
            Button btn = GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(OnClick);
            }
        }

        //sets sell button panel to be true
        void OnClick()
        {
            slotDescription.SetActive(true);
        }
    }
}
