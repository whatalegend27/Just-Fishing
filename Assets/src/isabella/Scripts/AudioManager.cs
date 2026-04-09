using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Color hoverColor = Color.gray;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void OnMouseDown()
    {
        if (gameObject.tag == "AudioOff")
        {
            masterMixer.SetFloat("MasterVolume", -80f); // effectively mute
        }
        else
        {
            masterMixer.SetFloat("MasterVolume", 0f); // normal volume
        }
    }

    void OnMouseEnter() => spriteRenderer.color = hoverColor;
    void OnMouseExit() => spriteRenderer.color = originalColor;
}
