using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleToolbox : MonoBehaviour
{
    public GameObject[] Toolboxes;
    public GameObject TbToShow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool TBActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(GameObject tb in Toolboxes)
        {
            tb.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleHUD();
    }

    void HandleHUD()
    {
        if (!TBActive && Input.GetKeyDown(KeyCode.T)){
            foreach(GameObject tb in Toolboxes)
                {
                    tb.SetActive(false);
                }
            TbToShow.SetActive(true);
            TBActive = true;
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            foreach(GameObject tb in Toolboxes)
            {
                tb.SetActive(false);
            }
            TBActive = false;
        }
    }
}