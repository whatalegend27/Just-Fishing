using UnityEngine; // Standard library for Unity features

public class CameraFollow : MonoBehaviour // Main class that controls the camera movement
{
    public Transform target; // The object the camera is locked onto (usually the Hook)
    public float smoothSpeed = 0.125f; // How "laggy" or smooth the camera follow feels
    public float verticalOffset = -2f; // Offset so the camera isn't exactly centered on the target

    // LATEUPDATE PATTERN: This runs AFTER the Hook moves in Update, so the camera doesn't shake
    void LateUpdate() 
    {
        if (target != null) // Safety check: only move if the camera actually has something to follow
        {
            // Create a new position: keep current X and Z, but match the Target's Y (plus the offset)
            Vector3 desiredPosition = new Vector3(transform.position.x, target.position.y + verticalOffset, transform.position.z);
            
            // LERP (Linear Interpolation): Smoothly slide from the current position to the new position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }
    }
}