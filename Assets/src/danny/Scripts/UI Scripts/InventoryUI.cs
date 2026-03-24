using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public InventorySlotUI[] inventorySlotUI;

    void OnEnable()
    {
        //calls singleton using Instance and the inventoryChanged signal. Calls Refresh when ui is enabled
        InventoryManager.Instance.inventoryChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
       //-= tells to stop listening for signal since panel isn't active
       InventoryManager.Instance.inventoryChanged -= Refresh;
    }

    void Refresh()
    {
        int i =0;
        foreach (InventorySlotUI slots in inventorySlotUI)
        {
            slots.SetUp(InventoryManager.Instance.slots[i]);
            i++;
        }
    }
}

