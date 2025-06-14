using UnityEngine;
using UnityEngine.InputSystem;
using VideojogosLusofona.LusoLander;

public class LanderController : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField]
    [Tooltip("Thrust/force to apply when pressing up")]
    private float thrustSpeed = 4.0f;

    [SerializeField]
    [Tooltip("Rotation (degrees/second) to apply when pressing the right or left")]
    private float rotationSpeed = 35f;
    [SerializeField]
    private float maxRotation = 70;

    [SerializeField]
    private float minDotNotToDestroy = 0.75f;
    [SerializeField]
    private float minSpeedNotToDestroy = 0.15f;

    private Rigidbody2D rb;

    private float rotation;

    private IInputSender inputSender;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing from the LanderController GameObject.");
        }

        inputSender = GetComponent<IInputSender>();
        if (inputSender == null)
        {
            Debug.LogError("IInputSender component is missing from the LanderController GameObject.");
        }

        rotation = transform.rotation.eulerAngles.z;
    }

    private void Start()
    {
        rotation = 90;
        transform.rotation = Quaternion.Euler(0, 0, rotation);

        rb.AddForce(Vector3.right * 1.75f, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        inputSender.GetInput(out bool thrust, out RotationInput rotationInput);

        if (thrust)
        {
            Thrust();
        }

        if (rotationInput != 0)
        {
            Rotate(rotationInput == RotationInput.Left ? -1 : 1);
        }
    }

    private void Thrust()
    {
        // Apply thrust in the direction the lander is facing
        rb.AddForce(transform.up * thrustSpeed);
    }

    private void Rotate(float direction)
    {
        // Calculate the new rotation angle
        rotation += -direction * rotationSpeed * Time.fixedDeltaTime;

        // Clamp the rotation to the specified maximum angle
        rotation = Mathf.Clamp(rotation, -maxRotation, maxRotation);

        // Apply the rotation to the Rigidbody2D
        rb.MoveRotation(rotation);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 collisionNormal = collision.contacts[0].normal;
        Vector3 landerUp = transform.up;
        float dot = Vector3.Dot(collisionNormal.normalized, landerUp.normalized);

        bool dotPass = dot > minDotNotToDestroy;

        bool isLowSpeed = rb.linearVelocity.magnitude < minSpeedNotToDestroy;

        Debug.Log($"Collision detected. Dot product: {dot}, Speed: {rb.linearVelocity.magnitude}");

        if (dotPass && isLowSpeed)
        {
            Debug.Log("Lander landed successfully.");
        }
        else
        {
            Debug.Log("Lander destroyed due to collision with insufficient angle.");
        }
    }
}
