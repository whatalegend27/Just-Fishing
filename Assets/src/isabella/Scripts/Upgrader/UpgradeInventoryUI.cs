using UnityEngine;

// Manages the Inventory UI for the upgrader.
public class UpgradeInventoryUI : MonoBehaviour
{
    [SerializeField] private UpgradeItemSlotUI[] slots;
    public static UpgradeInventoryUI Instance { get; private set; }

    // Ensure only one instance of the UI manager exists.
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        // Start happens AFTER all Awake calls are finished.
        // This ensures InventoryManager.Instance is definitely set.
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.inventoryChanged += Refresh;
            Refresh();
        }
    }

    // Keep OnEnable but add a null check so it doesn't crash on the first frame
    void OnEnable()
    {
        if (InventoryManager.Instance != null)
        {
            Refresh();
        }
    }

    void OnDisable()
    {
        InventoryManager.Instance.inventoryChanged -= Refresh;
    }

    // Refresh the UI slots based on the current inventory, filtering for relevant item types and clearing unused slots.
    public void Refresh()
    {
        
        int index = 0;

        foreach (var invSlot in InventoryManager.Instance.slots)
        {
            if (invSlot.item != null &&
                (invSlot.item.Type == ItemScript.ItemType.Lure ||   
                 invSlot.item.Type == ItemScript.ItemType.Bait ||
                 invSlot.item.Type == ItemScript.ItemType.Weight))
            {
                if (index < slots.Length)
                {
                    slots[index].SetUp(invSlot);
                    index++;
                }
            }
        }

        // clear remaining slots
        for (int i = index; i < slots.Length; i++)
        {
            slots[i].Clear();
        }
    }
}