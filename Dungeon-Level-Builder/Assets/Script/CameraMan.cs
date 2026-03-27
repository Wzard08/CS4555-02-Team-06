using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -4);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // FIX: Keep camera behind player rotation
        Vector3 desiredPosition = target.position + target.rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Look slightly above player (better view)
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}