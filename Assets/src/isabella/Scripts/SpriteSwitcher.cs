using UnityEngine;

public class SpriteSwitcher : MonoBehaviour
{
    [Header("Toolbox Settings")]
    private GameObject[] toolboxes;
    [SerializeField] private GameObject tbShow;

    private Color hoverColor = Color.gray;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    // Get the SpriteRenderer component and store the original color for hover effects.

    void Awake()
    {
        toolboxes = GameObject.FindGameObjectsWithTag("Toolbox");
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // Change Toolboxes when the button is clicked.
    private void OnMouseDown()
    {
        foreach(GameObject tb in toolboxes)
        {
            tb.SetActive(false);
        }
        tbShow.SetActive(true);
    }

    // Hover effect for the button
    void OnMouseEnter() => spriteRenderer.color = hoverColor;
    void OnMouseExit() => spriteRenderer.color = originalColor;
}
