using UnityEngine;

namespace VideojogosLusofona.LusoLander.ML
{
    public class LandingDetector : MonoBehaviour
    {
        [SerializeField]
        private float maxLandingSpeed = 2f;

        [SerializeField]
        private float maxLandingAngle = 15f;

        [SerializeField]
        private LayerMask groundLayer = 1;

        [SerializeField]
        private Transform target;

        [SerializeField]
        private float targetRadius = 5f;

        [SerializeField]
        private bool showDebugInfo = true;

        private Rigidbody2D rb;
        private AITrainingManager trainingManager;
        private bool hasLanded = false;
        private bool isGrounded = false;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            trainingManager = FindObjectOfType<AITrainingManager>();

            if (target == null)
            {
                GameObject targetObj = GameObject.FindWithTag("Target");
                if (targetObj != null)
                    target = targetObj.transform;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            {
                isGrounded = true;
                CheckLanding();
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            {
                isGrounded = false;
                hasLanded = false;
            }
        }

        void CheckLanding()
        {
            if (hasLanded)
                return;

            float speed = rb.linearVelocity.magnitude;
            float angle = Mathf.Abs(transform.eulerAngles.z);

            if (angle > 180f)
                angle = 360f - angle;

            bool speedOK = speed <= maxLandingSpeed;
            bool angleOK = angle <= maxLandingAngle;
            bool nearTarget = IsNearTarget();

            hasLanded = true;

            if (speedOK && angleOK && nearTarget)
            {
                OnSuccessfulLanding();
            }
            else
            {
                OnFailedLanding(speed, angle, nearTarget);
            }
        }

        bool IsNearTarget()
        {
            if (target == null)
                return true;

            float distance = Vector2.Distance(transform.position, target.position);
            return distance <= targetRadius;
        }

        void OnSuccessfulLanding()
        {
            if (trainingManager != null)
                trainingManager.OnLandingSuccess();
        }

        void OnFailedLanding(float speed, float angle, bool nearTarget)
        {
            if (trainingManager != null)
                trainingManager.OnLandingFailure();
        }

        void OnDrawGizmosSelected()
        {
            if (target != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(target.position, targetRadius);
            }

            if (rb != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(
                    transform.position,
                    transform.position + (Vector3)rb.linearVelocity
                );
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void SetLandingParameters(float maxSpeed, float maxAngle, float radius)
        {
            maxLandingSpeed = maxSpeed;
            maxLandingAngle = maxAngle;
            targetRadius = radius;
        }

        public bool IsLanded() => hasLanded && isGrounded;

        public bool IsGrounded() => isGrounded;

        public float GetCurrentSpeed() => rb != null ? rb.linearVelocity.magnitude : 0f;

        public float GetCurrentAngle()
        {
            float angle = Mathf.Abs(transform.eulerAngles.z);
            return angle > 180f ? 360f - angle : angle;
        }
    }
}
