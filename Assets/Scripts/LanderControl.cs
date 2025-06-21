using UnityEngine;

namespace VideojogosLusofona.LusoLander
{
    public class LanderControl : MonoBehaviour
    {
        [SerializeField]
        private Sprite landerNoThrusters;

        [SerializeField]
        private Sprite landerWithThrusters;

        [SerializeField]
        private float thrust = 25.0f;

        [SerializeField]
        private float rotationSpeed = 120f;

        [SerializeField]
        private bool allowAIControl = false;

        [SerializeField]
        public float combustivel = 100f;

        [SerializeField]
        public bool aterrou = false;

        [SerializeField]
        public bool colidiu = false;

        private Rigidbody2D rb;
        private SpriteRenderer sr;
        private RotationInput rotationInput = RotationInput.None;
        private bool thrustInput = false;
        private RotationInput aiRotationInput = RotationInput.None;
        private bool aiThrustInput = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            if (thrustInput)
            {
                Vector2 force = thrust * (Vector2)transform.up;
                rb.AddForce(force, ForceMode2D.Force);
            }

            if (rotationInput != RotationInput.None)
            {
                float torque = -(int)rotationInput * rotationSpeed;
                rb.AddTorque(torque, ForceMode2D.Force);
            }
        }

        public void SetAIControl(bool enabled)
        {
            allowAIControl = enabled;
        }

        public void SetAIInputs(RotationInput rotation, bool thrust)
        {
            aiRotationInput = rotation;
            aiThrustInput = thrust;
        }

        public void SetAIInputs(float horizontalInput, bool thrust)
        {
            aiRotationInput = (RotationInput)
                Mathf.RoundToInt(Mathf.Clamp(horizontalInput, -1f, 1f));
            aiThrustInput = thrust;
        }

        public Vector2 GetVelocity() => rb != null ? rb.linearVelocity : Vector2.zero;

        public Vector2 GetPosition() => transform.position;

        public float GetRotation() => transform.eulerAngles.z;

        public bool IsAIControlled() => allowAIControl;

        public void ResetarEstado()
        {
            combustivel = 100f;
            aterrou = false;
            colidiu = false;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                float velocidadeImpacto = rb.linearVelocity.magnitude;
                if (velocidadeImpacto < 3f && Mathf.Abs(transform.eulerAngles.z) < 15f)
                {
                    aterrou = true;
                }
                else
                {
                    colidiu = true;
                }
            }
        }

        void Update()
        {
            if (thrustInput && combustivel > 0)
            {
                combustivel -= Time.deltaTime * 10f;
                combustivel = Mathf.Max(0, combustivel);
            }

            if (allowAIControl)
            {
                rotationInput = aiRotationInput;
                thrustInput = aiThrustInput && combustivel > 0;
            }
            else
            {
                float horizontal = 0f;
                float vertical = 0f;

                if (Input.GetKey(KeyCode.A))
                    horizontal = -1f;
                else if (Input.GetKey(KeyCode.D))
                    horizontal = 1f;

                if (Input.GetKey(KeyCode.W) && combustivel > 0)
                    vertical = 1f;

                rotationInput = (RotationInput)Mathf.Round(horizontal);
                thrustInput = vertical > 0;
            }

            if (thrustInput && sr.sprite == landerNoThrusters)
            {
                sr.sprite = landerWithThrusters;
            }
            else if (!thrustInput && sr.sprite == landerWithThrusters)
            {
                sr.sprite = landerNoThrusters;
            }
        }
    }
}
