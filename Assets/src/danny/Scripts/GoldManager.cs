using UnityEngine;
using TMPro;
using System.Collections;

public class GoldManager : MonoBehaviour
{

    public static GoldManager Instance { get; private set; }
    public TextMeshProUGUI goldText;
    public Animator animator;
    public int playerGold = 10;

    void Awake()
    {
        //singleton creation so only one gold manager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        UpdateUI();
    }

    //checks if player has enough to buy
    bool CanAfford(ItemScript item)
    {
        if (item.price <= playerGold)
        {
            return true;
        } else
        {
            return false;
        }
    }

    //buys item - subtract money and add to inventory
    public bool BuyItem(ItemScript item)
    {
        if (CanAfford(item))
        {
            playerGold -= item.price;
            InventoryManager.Instance.AddItem(item);
            UpdateUI();
            return true;
        } else
        {
            StartCoroutine(TurnRed()); 
            return false;
        }
    }

    //sells item - adds money
    public void SellItem(ItemScript item)
    {
        playerGold += item.price;
        UpdateUI();
    }

    //updates gold amount
    void UpdateUI()
    {
        goldText.text = "$" + playerGold.ToString();
    }

    //turns gold text red for a sec and plays wiggle animation
    IEnumerator TurnRed()
    {
        goldText.color = Color.red;
        animator.Play("TextWiggle", -1, 0f);
        yield return new WaitForSeconds(1f);
        goldText.color = Color.white;
    }
}
