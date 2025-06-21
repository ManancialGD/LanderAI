using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [field: SerializeField] public Transform Target { get; private set; }
    [field: SerializeField] private Vector3 positionLerp = new Vector3(0.1f, 0.1f, 0.1f);

    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float rotationLerp = 0.1f;
    [SerializeField] private float maxRotationAngle = 0;

    private void FixedUpdate()
    {
        if (Target == null)
            return;

        Vector3 targetPosition = Target.position;
        Vector3 currentPosition = transform.position;

        // Smoothly interpolate the camera position towards the target position
        Vector3 newPosition = Vector3.Lerp(currentPosition, targetPosition, positionLerp.x * Time.fixedDeltaTime);
        newPosition.y = Mathf.Lerp(currentPosition.y, targetPosition.y, positionLerp.y * Time.fixedDeltaTime);
        newPosition.z = Mathf.Lerp(currentPosition.z, targetPosition.z, positionLerp.z * Time.fixedDeltaTime);

        transform.position = newPosition + offset;

        float targetRotationZ = Target.rotation.eulerAngles.z;

        // Convert angles to [-180, 180] range for proper clamping
        float normalizedTargetRotationZ = (targetRotationZ > 180f) ? targetRotationZ - 360f : targetRotationZ;
        
        float currentRotationZ = transform.rotation.eulerAngles.z;
        float normalizedCurrentRotationZ = (currentRotationZ > 180f) ? currentRotationZ - 360f : currentRotationZ;

        float clampedTargetRotationZ = Mathf.Clamp(normalizedTargetRotationZ, -maxRotationAngle, maxRotationAngle);

        float smoothRotationZ = Mathf.LerpAngle(normalizedCurrentRotationZ, clampedTargetRotationZ, rotationLerp * Time.fixedDeltaTime);

        transform.rotation = Quaternion.Euler(0, 0, smoothRotationZ);
    }
}
