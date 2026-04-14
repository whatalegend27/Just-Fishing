using UnityEngine;
using UnityEngine.SceneManagement;

// This script should be attached to any GameObject with a Collider2D component that you want to act as a button for changing scenes.
public class SceneChanger : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField]private string sceneToLoad;
    private Color hoverColor = Color.gray;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // This runs when the mouse clicks the collider
    void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No scene name entered on " + gameObject.name);
        }
    }

    // Hover effect for the button
    void OnMouseEnter() => spriteRenderer.color = hoverColor;
    void OnMouseExit() => spriteRenderer.color = originalColor;
}