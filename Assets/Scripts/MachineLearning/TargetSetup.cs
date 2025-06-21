using UnityEngine;

namespace VideojogosLusofona.LusoLander.ML
{
    public class TargetSetup : MonoBehaviour
    {
        [SerializeField]
        private Vector2 targetPosition = new Vector2(0, -5);

        [SerializeField]
        private float targetSize = 2f;

        [SerializeField]
        private Color targetColor = Color.green;

        void Start() => SetupTarget();

        void SetupTarget()
        {
            transform.position = targetPosition;

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null)
                sr = gameObject.AddComponent<SpriteRenderer>();

            if (sr.sprite == null)
            {
                sr.sprite = CreateSimpleSprite();
            }

            sr.color = targetColor;
            transform.localScale = Vector3.one * targetSize;
            gameObject.tag = "Target";
        }

        Sprite CreateSimpleSprite()
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        }

        public void SetTargetPosition(Vector2 newPosition)
        {
            targetPosition = newPosition;
            transform.position = targetPosition;
        }

        public void SetRandomPosition()
        {
            Vector2 randomPos = new Vector2(Random.Range(-15f, 15f), Random.Range(-8f, -2f));
            SetTargetPosition(randomPos);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = targetColor;
            Gizmos.DrawWireCube(transform.position, Vector3.one * targetSize);
        }
    }
}
