using UnityEngine;

// This script allows the player to exit the game when they click on the associated button.
public class ExitGame : MonoBehaviour
{
    private Color hoverColor = Color.gray;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    // Get the SpriteRenderer component and store the original color for hover effects.
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }
    
    //On button click, quit the application
    private void OnMouseDown()
    {
        Application.Quit();
    }

    // Hover effect for the button
    void OnMouseEnter() => spriteRenderer.color = hoverColor;
    void OnMouseExit() => spriteRenderer.color = originalColor;
}
