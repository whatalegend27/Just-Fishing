using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Drag your Hook_Small here
    public float smoothSpeed = 0.125f;
    public float verticalOffset = -2f; // Keeps the hook slightly above center

    void LateUpdate()
    {
        if (target != null)
        {
            // We only want the camera to follow the Y (up/down)
            // Keep the Camera's X and Z the same
            Vector3 desiredPosition = new Vector3(transform.position.x, target.position.y + verticalOffset, transform.position.z);
            
            // Smoothly move the camera
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }
    }
}