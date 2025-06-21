using UnityEngine;
using UnityEngine.UI;

namespace VideojogosLusofona.LusoLander.ML
{
    public class AITrainingManager : MonoBehaviour
    {
        [SerializeField]
        private SimpleLanderAI landerAI;

        [SerializeField]
        private Transform target;

        [SerializeField]
        private Transform lander;

        [SerializeField]
        private Button trainButton;

        [SerializeField]
        private Button toggleAIButton;

        [SerializeField]
        private Button recordButton;

        [SerializeField]
        private Text statusText;

        [SerializeField]
        private bool autoResetOnCrash = true;

        [SerializeField]
        private float maxDistanceFromTarget = 50f;

        [SerializeField]
        private Vector2 landerStartPosition;

        [SerializeField]
        private float normalSpeed = 1f;

        [SerializeField]
        private float trainingSpeed = 5f;

        [SerializeField]
        private float maxTrainingSpeed = 20f;

        private bool isRecording = false;
        private bool aiEnabled = false;
        private bool comportamentoPuro = false;
        private float sessionStartTime;
        private int crashCount = 0;
        private int successCount = 0;

        void Start()
        {
            if (landerAI == null)
                landerAI = FindObjectOfType<SimpleLanderAI>();

            if (lander == null && landerAI != null)
                lander = landerAI.transform;

            if (target == null)
            {
                GameObject targetObj = GameObject.FindWithTag("Target");
                if (targetObj != null)
                    target = targetObj.transform;
            }

            SetupUI();
            sessionStartTime = Time.time;
            UpdateStatusText();
        }

        void Update()
        {
            if (lander != null && target != null)
            {
                float distance = Vector2.Distance(lander.position, target.position);
                if (distance > maxDistanceFromTarget && autoResetOnCrash)
                {
                    ResetLander();
                    crashCount++;
                }
            }

            UpdateStatusText();
            HandleKeyboardInput();
        }

        void SetupUI()
        {
            if (trainButton != null)
                trainButton.onClick.AddListener(TrainAI);

            if (toggleAIButton != null)
                toggleAIButton.onClick.AddListener(ToggleAI);

            if (recordButton != null)
                recordButton.onClick.AddListener(ToggleRecording);
        }

        void HandleKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.T))
                TrainAI();

            if (Input.GetKeyDown(KeyCode.I))
                ToggleAI();

            if (Input.GetKeyDown(KeyCode.R))
                ToggleRecording();

            if (Input.GetKeyDown(KeyCode.Space))
                ResetLander();

            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
                IncreaseSpeed();

            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
                DecreaseSpeed();

            if (Input.GetKeyDown(KeyCode.Alpha1))
                SetSpeed(normalSpeed);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                SetSpeed(trainingSpeed);

            if (Input.GetKeyDown(KeyCode.P))
                ToggleComportamentoPuro();
        }

        public void TrainAI()
        {
            Debug.Log("Algoritmo genético já está rodando automaticamente!");
        }

        public void ToggleAI()
        {
            if (landerAI != null)
            {
                aiEnabled = !aiEnabled;
                landerAI.enabled = aiEnabled;
                Debug.Log($"IA {(aiEnabled ? "Ativada" : "Desativada")}");
            }
        }

        public void ToggleRecording()
        {
            Debug.Log("Algoritmo genético não usa gravação - evolui automaticamente!");
        }

        public void ResetLander()
        {
            Debug.Log("Reset automático pelo algoritmo genético!");
        }

        void UpdateStatusText()
        {
            if (statusText != null)
            {
                float sessionTime = Time.time - sessionStartTime;
                string status = $"Tempo: {sessionTime:F1}s\n";
                status += $"Velocidade: {Time.timeScale:F1}x\n";
                status += $"IA: {(aiEnabled ? "ON" : "OFF")}\n";
                status += $"Gravação: {(isRecording ? "ON" : "OFF")}\n";
                status += $"Crashes: {crashCount}\n";
                status += $"Sucessos: {successCount}\n\n";
                status += "Controles:\n";
                status += "T - Treinar IA\n";
                status += "I - Toggle IA\n";
                status += "R - Toggle Gravação\n";
                status += "Espaço - Reset Nave\n";
                status += "+/- - Velocidade\n";
                status += "1 - Normal | 2 - Treino";

                statusText.text = status;
            }
        }

        public void OnLandingSuccess()
        {
            successCount++;

            if (autoResetOnCrash)
            {
                Invoke(nameof(ResetLander), 2f);
            }
        }

        public void OnLandingFailure()
        {
            crashCount++;

            if (autoResetOnCrash)
            {
                Invoke(nameof(ResetLander), 1f);
            }
        }

        public void CreateRandomTarget()
        {
            if (target != null)
            {
                Vector2 randomPos = new Vector2(Random.Range(-20f, 20f), Random.Range(-5f, 5f));
                target.position = randomPos;
            }
        }

        public void SetSpeed(float speed)
        {
            Time.timeScale = Mathf.Clamp(speed, 0.1f, maxTrainingSpeed);
            Debug.Log($"Velocidade definida para: {Time.timeScale:F1}x");
        }

        public void IncreaseSpeed()
        {
            float newSpeed = Time.timeScale + 1f;
            SetSpeed(newSpeed);
        }

        public void DecreaseSpeed()
        {
            float newSpeed = Time.timeScale - 1f;
            SetSpeed(newSpeed);
        }

        public void SetNormalSpeed() => SetSpeed(normalSpeed);

        public void SetTrainingSpeed() => SetSpeed(trainingSpeed);

        public void ToggleComportamentoPuro()
        {
            Debug.Log("Algoritmo genético sempre usa o melhor indivíduo da geração atual!");
        }

        void OnGUI()
        {
            if (statusText == null && landerAI != null)
            {
                GUILayout.BeginArea(new Rect(10, 10, 350, 200));
                GUILayout.Label($"Velocidade: {Time.timeScale:F1}x");
                GUILayout.Label($"IA: {(aiEnabled ? "ON" : "OFF")}");
                GUILayout.Label($"Geração: {landerAI.generation}");
                GUILayout.Label($"Indivíduo: {landerAI.currentIndex}/{landerAI.populationSize}");
                GUILayout.Label($"Melhor Fitness: {landerAI.bestFitness:F1}");

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("1x"))
                    SetNormalSpeed();
                if (GUILayout.Button("10x"))
                    SetTrainingSpeed();
                if (GUILayout.Button("+"))
                    IncreaseSpeed();
                if (GUILayout.Button("-"))
                    DecreaseSpeed();
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Toggle IA"))
                    ToggleAI();

                GUILayout.EndArea();
            }
        }
    }
}
