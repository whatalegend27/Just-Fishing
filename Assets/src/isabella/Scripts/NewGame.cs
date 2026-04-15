using UnityEngine;

// Inherit from SceneChanger instead of MonoBehaviour
public class NewGame : SceneChanger 
{
    // When the mouse clicks this object, the program looks for the most specific version of OnMouseDown in the hierarchy.
    public override void OnMouseDown()
    {
        // Perform the unique logic for a New Game
        PlayerPrefs.DeleteKey("NewsSeen");
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs reset for New Game.");

        // Call the base version to handle the actual scene transition
        base.OnMouseDown(); 
    }
}