using UnityEditor;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventorySlotUI[] inventorySlotUI;
    [SerializeField] private GameObject inventoryTB;
    [SerializeField] private GameObject inventoryDescription;

    //runs when inventory turns on
    void OnEnable()
    {
        //doesn't call refresh if inventory isn't initailized yet
        if (InventoryManager.Instance == null)
        {
            return;
        }
        //calls singleton using Instance and the inventoryChanged signal. Calls Refresh when ui is enabled
        InventoryManager.Instance.inventoryChanged += Refresh;
        Refresh();
    }


    //when inventory turned off
    void OnDisable()
    {
        //-= tells to stop listening for signal since panel isn't active
        InventoryManager.Instance.inventoryChanged -= Refresh;
    }

    // Refreshs inventory slots to update items when something is added
    void Refresh()
    {
        int i = 0;
        foreach (InventorySlotUI slots in inventorySlotUI)
        {
            slots.SetUp(InventoryManager.Instance.slots[i]); //from InventorySlotUI
            i++;
        }
    }

    void Update()
    {
        ToggleMenu();
    }

    void ToggleMenu()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            bool isActive = inventoryTB.activeSelf;
            inventoryTB.SetActive(!isActive); // just flip whatever state it's actually in

            if (isActive) // was open, now closing
            {
                inventoryDescription.SetActive(false);
            }
        }
    }
}

