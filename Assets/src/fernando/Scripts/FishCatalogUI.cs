using UnityEngine;

public class FishCatalogUI : MonoBehaviour
{
    public GameObject RT;
    public GameObject RM;
    public GameObject RB;
    public GameObject LT;

    public GameObject LM;
    public GameObject LB;


    void Start()
    {
        RT.SetActive(false);
        RM.SetActive(false);
        RM.SetActive(false);
        LT.SetActive(false);
        LM.SetActive(false);
        LB.SetActive(false);

        for (int i = 0; i < FishDatabaseManager.Instance.fishDatabase.Count; i++)
        {
            FishData fish = FishDatabaseManager.Instance.fishDatabase[i];

            if (fish.fishKnown)
            {
                RT.SetActive(true);
                RM.SetActive(true);
                RM.SetActive(true);
                LT.SetActive(true);
                LM.SetActive(true);
                LB.SetActive(true);
            }
        }
    }
}