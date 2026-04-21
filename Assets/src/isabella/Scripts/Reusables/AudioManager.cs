using UnityEngine;
using UnityEngine.Audio;

// This script manages the audio settings for the game, allowing players to toggle audio on and off by clicking on the associated button. It also changes the button's color when hovered over for better user feedback.
public class AudioManager : MonoBehaviour
{
    //Variables for Audio Manager
    [SerializeField] public AudioMixer masterMixer; //handle the audio mixer to control volume
    private Color hoverColor = Color.gray; // color when hovering over button
    private Color originalColor; //original color to change it back
    private SpriteRenderer spriteRenderer; //grab the sprite renderer to change the color


    // On start of scene
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void SetAudio(bool isOn)
    {
        if (isOn)
        {
            masterMixer.SetFloat("MasterVolume", 0f); // normal volume
        }
        else
        {
            masterMixer.SetFloat("MasterVolume", -80f); // mute
        }
    }

    //On button click
    private void OnMouseDown()
    {
        //If tag is AudioOff, set volume to -80dB (mute), else set it back to 0dB (normal)
        if (gameObject.tag == "AudioOff")
        {
            SetAudio(false); // effectively mute
        }
        else
        {
            SetAudio(true); // normal volume
        }
    }

    //Change color on hover
    void OnMouseEnter() => spriteRenderer.color = hoverColor;
    void OnMouseExit() => spriteRenderer.color = originalColor;
}
