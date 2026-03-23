using UnityEngine;

public class InventoryManager : MonoBehaviour
{
   public GameObject inventoryMenu;
   bool menuActive = false;

    void Update()
    {
        toggleMenu();
    }

    void toggleMenu()
    {
        if (Input.GetKeyDown(KeyCode.H) && !menuActive)
        {
            inventoryMenu.SetActive(true);
            menuActive = true;
        } else if (Input.GetKeyDown(KeyCode.E) && menuActive)
        {
            inventoryMenu.SetActive(false);
            menuActive = false;
        } 
    }
}
