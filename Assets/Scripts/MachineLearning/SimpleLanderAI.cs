using System.Collections.Generic;
using UnityEngine;

namespace VideojogosLusofona.LusoLander
{
    public class SimpleLanderAI : MonoBehaviour
    {
        [Header("Configurações Hill Climbing")]
        [SerializeField] public bool iaAtiva = true;
        [SerializeField] public int populationSize = 20;

        [SerializeField]
        private float mutationRate = 0.1f;

        [SerializeField]
        private float simulationSpeed = 10f;

        [Header("Estado")]
        [SerializeField]
        public int generation;

        [SerializeField]
        public int currentIndex;

        [SerializeField]
        public float bestFitness;

        private List<SimpleNeuralNetwork> population = new List<SimpleNeuralNetwork>();
        private SimpleNeuralNetwork currentBrain;
        private LanderControl controle;
        private Rigidbody2D fisica;
        private Transform alvo;
        private float tempoEpisodio = 0f;
        private float tempoMaximoEpisodio = 30f;

        void Start()
        {
            Time.timeScale = simulationSpeed;
            InicializarComponentes();
            InicializarPopulacao();
        }

        void InicializarComponentes()
        {
            controle = GetComponent<LanderControl>();
            fisica = GetComponent<Rigidbody2D>();
            alvo = GameObject.FindWithTag("Target")?.transform;
            controle.SetAIControl(iaAtiva);
        }

        void InicializarPopulacao()
        {
            population.Clear();
            for (int i = 0; i < populationSize; i++)
            {
                var brain = new SimpleNeuralNetwork();
                brain.InitializeNetwork();
                population.Add(brain);
            }
            currentIndex = 0;
            currentBrain = population[0];
        }

        void FixedUpdate()
        {
            if (!iaAtiva || alvo == null || currentBrain == null)
                return;

            tempoEpisodio += Time.fixedDeltaTime;

            float[] inputs =
            {
                controle.combustivel / 100f,
                fisica.linearVelocity.x,
                fisica.linearVelocity.y,
                transform.eulerAngles.z / 360f,
                (alvo.position.x - transform.position.x) / 50f,
                (alvo.position.y - transform.position.y) / 50f,
            };

            var (rotation, thrust) = currentBrain.Predict(inputs);
            controle.SetAIInputs(rotation, thrust);

            if (VerificarFimEpisodio())
            {
                AvaliarDesempenho();
                ProximoIndividuo();
                ResetPosicao();
            }
        }

        bool VerificarFimEpisodio()
        {
            if (controle.aterrou || controle.colidiu || controle.combustivel <= 0)
                return true;

            if (alvo != null)
            {
                float distancia = Vector2.Distance(transform.position, alvo.position);
                if (distancia > 100f)
                    return true;
            }

            if (tempoEpisodio > tempoMaximoEpisodio)
                return true;

            return false;
        }

        void AvaliarDesempenho()
        {
            float fitness = 0f;

            if (controle.aterrou)
            {
                fitness += 1000;
                fitness += Mathf.Clamp(100 - Mathf.Abs(fisica.linearVelocity.x) * 10, 0, 500);
                fitness += Mathf.Clamp(100 - Mathf.Abs(fisica.linearVelocity.y) * 10, 0, 500);
                fitness += (1 - Mathf.Abs(transform.eulerAngles.z / 180f)) * 200;
            }

            if (controle.colidiu)
            {
                fitness -= 500;
            }

            fitness += controle.combustivel * 2;

            float distancia = Vector2.Distance(transform.position, alvo.position);
            fitness += (1 - Mathf.Clamp01(distancia / 100f)) * 300;

            currentBrain.fitness = fitness;

            if (fitness > bestFitness)
            {
                bestFitness = fitness;
            }
        }

        void ProximoIndividuo()
        {
            currentIndex++;

            if (currentIndex >= populationSize)
            {
                EvoluirPopulacao();
                generation++;
                currentIndex = 0;
            }

            currentBrain = population[currentIndex];
        }

        void EvoluirPopulacao()
        {
            population.Sort((a, b) => b.fitness.CompareTo(a.fitness));

            List<SimpleNeuralNetwork> newPopulation = new List<SimpleNeuralNetwork>
            {
                population[0].Clone(),
                population[1].Clone(),
            };

            while (newPopulation.Count < populationSize)
            {
                SimpleNeuralNetwork parent1 = SelecionarPai();
                SimpleNeuralNetwork parent2 = SelecionarPai();

                SimpleNeuralNetwork child = parent1.Clone();
                int crossoverPoint = UnityEngine.Random.Range(0, child.weights.Length);

                for (int i = crossoverPoint; i < child.weights.Length; i++)
                {
                    child.weights[i] = parent2.weights[i];
                }

                child.Mutate(mutationRate);
                newPopulation.Add(child);
            }

            population = newPopulation;
        }

        SimpleNeuralNetwork SelecionarPai()
        {
            float totalFitness = 0;
            foreach (var brain in population)
            {
                totalFitness += brain.fitness;
            }

            float randomPoint = UnityEngine.Random.value * totalFitness;
            float currentSum = 0;

            foreach (var brain in population)
            {
                currentSum += brain.fitness;
                if (currentSum >= randomPoint)
                {
                    return brain;
                }
            }

            return population[0];
        }

        void ResetPosicao()
        {
            transform.position = new Vector2(0, 20);
            transform.rotation = Quaternion.identity;
            fisica.linearVelocity = Vector2.zero;
            fisica.angularVelocity = 0f;
            controle.ResetarEstado();
            tempoEpisodio = 0f;
        }

        void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 300, 20), $"Geração: {generation}");
            GUI.Label(new Rect(10, 30, 300, 20), $"Indivíduo: {currentIndex}/{populationSize}");
            GUI.Label(new Rect(10, 50, 300, 20), $"Melhor Fitness: {bestFitness:F2}");
            GUI.Label(new Rect(10, 70, 300, 20), $"Fitness Atual: {currentBrain?.fitness ?? 0:F2}");
        }
    }
}
