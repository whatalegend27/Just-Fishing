using UnityEngine;
using TMPro;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class GoldManager : MonoBehaviour
{

    public static GoldManager Instance { get; private set; }
    [SerializeField] private GameObject goldDisplay;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private int playerGold = 10;
    private bool canAddIn;

    public int PlayerGold => playerGold;
    public static void ResetInstance() => Instance = null; //used for testing

    private void Awake()
    {
        //singleton creation so only one gold manager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);  //persist across other scenes so gold stays updated
        SceneManager.sceneLoaded += OnSceneLoaded; //sees if the shop scene is loaded
        UpdateUI();
    }

    //checks if player has enough to buy
    bool CanAfford(ItemScript item)
    {
        if (item.Price <= playerGold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //buys item - subtract money and add to inventory
    public bool BuyItem(ItemScript item)
    {
        if (CanAfford(item))
        {
            canAddIn = InventoryManager.Instance.AddItem(item);    //from InventoryManager
            if (canAddIn == false)
            {
                Debug.Log("Inventory full");
                return false;
            }
            else
            {
                playerGold -= item.Price;
                UpdateUI();
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    //sells item - adds money
    public void SellItem(ItemScript item)
    {
        InventoryManager.Instance.RemoveItem(item);
        playerGold += item.Price;
        UpdateUI();

    }

    //add gold from non-selling sources in other scenes
    public void AddGold(int amount)
    {
        playerGold += amount;
        UpdateUI();
    }

    //updates gold amount
    void UpdateUI()
    {
        if (goldText == null)
        {
            return;
        }
        goldText.text = playerGold.ToString();
    }

    //activates gold amount whenever shop is loaded
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {

        if (scene.name == "Shop")
        {
            goldDisplay.SetActive(true);
        }
        else
        {
            goldDisplay.SetActive(false);
        }
    }


}
