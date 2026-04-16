using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class GoldManager : MonoBehaviour
{

    public static GoldManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private int playerGold = 100;
    private bool canAddIn;

    private void Awake()
    {
        //singleton creation so only one gold manager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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
           // StartCoroutine(FlashColor(Color.red));
            return false;
        }
    }

    //sells item - adds money
    public void SellItem(ItemScript item)
    {
        InventoryManager.Instance.RemoveItem(item);
        playerGold += item.Price;
      //  StartCoroutine(FlashColor(Color.green));
        UpdateUI();

    }

    //updates gold amount
    void UpdateUI()
    {
        goldText.text = playerGold.ToString();
    }

    //turns gold text red for a sec and plays wiggle animation
    IEnumerator FlashColor(Color color)
    {
        goldText.color = color;

        yield return new WaitForSeconds(1f);
        goldText.color = Color.white;
    }
}
