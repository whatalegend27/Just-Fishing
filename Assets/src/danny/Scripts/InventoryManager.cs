using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryTB;
    [SerializeField] private GameObject inventoryDescription;


    //prevents other scripts from writing into the inventory - public to allow other scripts to access
    public static InventoryManager Instance { get; private set; }
    public event System.Action inventoryChanged;
    private const int INVENTORY_SIZE = 9;
    public InventorySlotData[] slots = new InventorySlotData[INVENTORY_SIZE];




    //public static void ResetInstance() => Instance = null;

    //Initalizes each inventory slot to be empty
    private void Awake()
    {
        //Creation of singleton so only one inventory exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            slots[i] = new InventorySlotData();
        }
    }

    //adds item to inventory 
    public bool AddItem(ItemScript item)
    {
        //checks to see if no item
        if (item == null)
        {
            return false;
        }

        ItemScript currentItem = item;

        //if item can stack, see if its in inventory already and increase
        if (currentItem.CanStack())
        {
            for (int i = 0; i < INVENTORY_SIZE; i++)
            {
                if (slots[i].item != null && slots[i].item.name == currentItem.name)
                {
                    slots[i].quantity++;
                    inventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        //if item isn't in inventory or can't stack, add first into inventory where empty
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = currentItem;
                slots[i].quantity = 1;
                inventoryChanged?.Invoke();  //gives out signal for other methods to use. ? means to only give out signal if something is listening
                return true;
            }
        }
        return false;
    }

    //Removes item from inventory
    public void RemoveItem(ItemScript item)
    {
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {

            if (item.CanStack())
            {
                //finds stackable item in inventory and not last quantity
                if (slots[i].item == item && slots[i].quantity > 1)
                {
                    slots[i].quantity--;
                    inventoryChanged?.Invoke();
                    return;
                }
            }
            //removes non-stackable/last quantity out of inventory
            if (slots[i].item == item)
            {
                slots[i] = new InventorySlotData();
                inventoryChanged?.Invoke();
                inventoryDescription.SetActive(false);
                return;
            }
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


