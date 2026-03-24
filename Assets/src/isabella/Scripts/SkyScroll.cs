using UnityEngine;

public class SkyScroll : MonoBehaviour
{
    public float speed = 0.5f;
    public float resetPosition = -20f;
    public float startPosition = 20f;

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.position.x < resetPosition)
        {
            transform.position = new Vector2(startPosition, transform.position.y);
        }
    }
}