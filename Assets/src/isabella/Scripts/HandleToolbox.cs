using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleToolbox : MonoBehaviour
{
    public GameObject[] Toolboxes;
    public GameObject TbToShow;

    public bool TBActive = false; // 👈 make public for testing

    void Start()
    {
        foreach(GameObject tb in Toolboxes)
        {
            tb.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleToolbox(); // 👈 use new method
        }
    }

    // ✅ NEW METHOD (this is the key)
    public void ToggleToolbox()
    {
        if (!TBActive)
        {
            foreach(GameObject tb in Toolboxes)
            {
                tb.SetActive(false);
            }

            if (TbToShow != null)
                TbToShow.SetActive(true);

            TBActive = true;
        }
        else
        {
            foreach(GameObject tb in Toolboxes)
            {
                tb.SetActive(false);
            }

            TBActive = false;
        }
    }
}