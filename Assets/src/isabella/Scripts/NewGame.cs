using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGamer : MonoBehaviour
{
    [Header("Settings")]
    public string sceneToLoad;
    public Color hoverColor = Color.gray;
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

    // Optional: Visual feedback so you know the raycast is working
    void OnMouseEnter() => spriteRenderer.color = hoverColor;
    void OnMouseExit() => spriteRenderer.color = originalColor;
}