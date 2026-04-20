using UnityEngine;

public class UpgradeInventoryUI : MonoBehaviour
{
    [SerializeField] private UpgradeItemSlotUI[] slots;

    void Start()
    {
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

    void Refresh()
    {
        int index = 0;

        foreach (var invSlot in InventoryManager.Instance.slots)
        {
            if (invSlot.item != null &&
                (invSlot.item.Type == ItemScript.ItemType.Lure ||   // ✅ FIXED
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