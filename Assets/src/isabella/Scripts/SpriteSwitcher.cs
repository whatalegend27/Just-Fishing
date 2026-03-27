using UnityEngine;

public class SpriteSwitcher : MonoBehaviour
{
    public GameObject[] Toolboxes;
    public GameObject TbToShow;

    public Color hoverColor = Color.gray;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
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

    void OnMouseEnter() => spriteRenderer.color = hoverColor;
    void OnMouseExit() => spriteRenderer.color = originalColor;
}
