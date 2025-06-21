using System;
using UnityEngine;
using Mathf = UnityEngine.Mathf;

namespace VideojogosLusofona.LusoLander
{
    [System.Serializable]
    public class SimpleNeuralNetwork
    {
        public int inputSize = 6;
        public int hiddenSize = 8;
        public int outputSize = 2;

        [SerializeField]
        public float[] weights;

        public float fitness;

        public SimpleNeuralNetwork() { }

        public void InitializeNetwork()
        {
            int totalWeights =
                (inputSize * hiddenSize) + (hiddenSize * outputSize) + hiddenSize + outputSize;
            weights = new float[totalWeights];

            System.Random random = new System.Random();
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = (float)(random.NextDouble() * 2 - 1);
            }
        }

        public (int rotation, bool thrust) Predict(float[] inputs)
        {
            float[] hidden = new float[hiddenSize];
            int index = 0;

            for (int h = 0; h < hiddenSize; h++)
            {
                for (int i = 0; i < inputSize; i++)
                {
                    hidden[h] += inputs[i] * weights[index++];
                }
                hidden[h] = MathFunctions.Tanh(hidden[h] + weights[index++]);
            }

            float[] outputs = new float[outputSize];
            for (int o = 0; o < outputSize; o++)
            {
                for (int h = 0; h < hiddenSize; h++)
                {
                    outputs[o] += hidden[h] * weights[index++];
                }
                outputs[o] = MathFunctions.Tanh(outputs[o] + weights[index++]);
            }

            int rotation = outputs[0] switch
            {
                < -0.33f => -1,
                > 0.33f => 1,
                _ => 0,
            };

            bool thrust = outputs[1] > 0;

            return (rotation, thrust);
        }

        public void Mutate(float mutationRate)
        {
            System.Random random = new System.Random();
            for (int i = 0; i < weights.Length; i++)
            {
                if (random.NextDouble() < mutationRate)
                {
                    weights[i] += (float)(random.NextDouble() * 2 - 1) * 0.5f;
                }
            }
        }

        public SimpleNeuralNetwork Clone()
        {
            return new SimpleNeuralNetwork
            {
                inputSize = inputSize,
                hiddenSize = hiddenSize,
                outputSize = outputSize,
                weights = (float[])weights.Clone(),
                fitness = fitness,
            };
        }
    }
}
