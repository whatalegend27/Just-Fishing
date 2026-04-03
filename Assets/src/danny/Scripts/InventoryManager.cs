using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryMenu;
    bool menuActive = false;

    //prevents other scripts from writing into the inventory
    public static InventoryManager Instance { get; private set; }
    public const int INVENTORY_SIZE = 9;
    public InventorySlotData[] slots = new InventorySlotData[INVENTORY_SIZE];
    public event System.Action inventoryChanged;

    public static void ResetInstance() => Instance = null;

    public void Awake()
    {
        //Creation of singleton so only one inventory exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            slots[i] = new InventorySlotData();
        }
    }

    void Update()
    {
        ToggleMenu();
    }

    public bool AddItem(ItemScript item)
    {
        if (item == null)
        {
            return false;
        }
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = item;
                slots[i].quantity = 1;
                inventoryChanged?.Invoke();  //gives out signal for other methods to use. ? means to only give out signal if something is listening
                return true;
            }
        }
        return false;
    }

    void ToggleMenu()
    {
        if (Input.GetKeyDown(KeyCode.H) && !menuActive)
        {
            inventoryMenu.SetActive(true);
            menuActive = true;
        }
        else if (Input.GetKeyDown(KeyCode.H) && menuActive)
        {
            inventoryMenu.SetActive(false);
            menuActive = false;
        }
    }
}

