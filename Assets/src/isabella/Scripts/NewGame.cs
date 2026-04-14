using UnityEngine;
using UnityEngine.SceneManagement;

// This script handles the functionality of the "New Game" button, resetting player preferences and loading the specified scene.
public class NewGamer : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;
    private Color hoverColor = Color.gray;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    // Get the SpriteRenderer component and store the original color for hover effects
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // This runs when the mouse clicks the collider
    void OnMouseDown()
    {
        PlayerPrefs.DeleteKey("NewsSeen");
        PlayerPrefs.Save();

        Debug.Log("After reset: " + PlayerPrefs.GetInt("NewsSeen", 0));
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No scene name entered on " + gameObject.name);
        }
    }

    // Hover effects for the button
    void OnMouseEnter() => spriteRenderer.color = hoverColor;
    void OnMouseExit() => spriteRenderer.color = originalColor;
}