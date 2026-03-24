using UnityEngine;

public class SpriteSwitcher : MonoBehaviour
{
    public GameObject[] Toolboxes;
    public GameObject TbToShow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        foreach(GameObject tb in Toolboxes)
        {
            tb.SetActive(false);
        }
        TbToShow.SetActive(true);
    }
}
