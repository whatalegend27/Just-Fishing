using UnityEngine;

public class BoatTiltOnly : MonoBehaviour
{
    public float frequency = 5f;
    public float speed = 2f;
    public float tiltStrength = 10f;

    void Update()
    {
        Vector3 pos = transform.position;
        float t = Time.time * speed;

        float waveX =
            Mathf.Sin(pos.x * frequency + t);

        float waveZ =
            Mathf.Sin(pos.z * frequency * 0.7f + t * 1.2f);

        float tiltX = waveZ;
        float tiltZ = waveX;

        Quaternion targetRot =
            Quaternion.Euler(tiltZ * tiltStrength, 0, -tiltX * tiltStrength);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * 3f
        );
    }
}