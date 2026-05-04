using UnityEngine;

public class CloudLooper : MonoBehaviour
{
    public float speed = 1f;
    private float width;
    public Transform otherCloud;
    private Camera cam;

    void Start()
    {
        width = GetComponent<SpriteRenderer>().bounds.size.x;
        cam = Camera.main;
        if (transform.position.x < otherCloud.position.x)
        {
            transform.position = new Vector3(
                otherCloud.position.x - width,
                transform.position.y,
                transform.position.z
            );
        }
        else
        {
            transform.position = new Vector3(
                otherCloud.position.x + width,
                transform.position.y,
                transform.position.z
            );
        }
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        float leftEdge = cam.transform.position.x - cam.orthographicSize * cam.aspect;

        if (transform.position.x + width / 2 < leftEdge)
        {
            transform.position += new Vector3(width * 2f, 0, 0);
        }
    }
}