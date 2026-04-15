using UnityEngine;
using UnityEngine.SceneManagement;

// This script serves as a base class for any scene-changing objects, providing common functionality and allowing for specific overrides in child classes.
public class SceneChanger : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] protected string sceneToLoad; // 'protected' allows child classes to see this
    
    [Header("Color Settings")]
    [SerializeField] private Color hoverColor = Color.gray;
    protected Color originalColor;
    protected SpriteRenderer spriteRenderer;

    // Virtual Start allows children to add setup logic if needed
    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // Use this OnMouseDown unless child has an override.
    public virtual void OnMouseDown()
    {
        ExecuteSceneLoad();
    }

    //Change the scene.
    protected void ExecuteSceneLoad()
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

    // Shared hover logic (no need to repeat this in other scripts)
    protected virtual void OnMouseEnter() => spriteRenderer.color = hoverColor;
    protected virtual void OnMouseExit() => spriteRenderer.color = originalColor;
}