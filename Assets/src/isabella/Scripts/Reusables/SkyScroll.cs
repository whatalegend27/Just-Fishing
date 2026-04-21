using UnityEngine;

// This script should be attached to the Sky GameObject in the scene. It creates a scrolling effect by moving the sky to the left at a constant speed. When the sky reaches a certain position, it resets back to the starting position, creating a seamless loop.
public class SkyScroll : MonoBehaviour
{
    // Move the sky to the left at a speed of 0.5 units per second
    void Update()
    {
        transform.Translate(Vector2.left * 0.5f * Time.deltaTime);

        if (transform.position.x < -20f)
        {
            transform.position = new Vector2(20f, transform.position.y);
        }
    }
}