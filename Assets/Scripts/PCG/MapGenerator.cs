using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainType
{
    public string name;
    public float height;
    public Color color;
}

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapSize = 256;
    public int seed = 0;
    public bool autoUpdate = true;

    [Header("Midpoint Displacement Settings")]
    [Range(0.1f, 2f)]
    public float roughness = 0.7f;

    [Range(0.1f, 5f)]
    public float initialHeight = 1f;

    [Range(0f, 1f)]
    public float baseLevel = 0f;

    [Header("Terrain Settings")]
    public float mountainHeight = 1.5f;
    public float terrainScale = 10f;
    public TerrainType[] regions;

    [Header("Collision Settings")]
    public float colliderHeight = 1f;
    public bool generateColliders = true;

    [Header("Mesh Settings")]
    public Material terrainMaterial;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    void Awake()
    {
        if (regions == null || regions.Length == 0)
        {
            regions = TerrainPresets.GetMoonPreset();
        }
    }

    void Start()
    {
        InitializeComponents();
        GenerateMap();
    }

    void InitializeComponents()
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
        if (meshCollider == null)
            meshCollider = GetComponent<MeshCollider>();

        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        if (meshCollider == null && generateColliders)
            meshCollider = gameObject.AddComponent<MeshCollider>();

        if (terrainMaterial != null && meshRenderer != null)
            meshRenderer.material = terrainMaterial;
        else if (meshRenderer != null && meshRenderer.material == null)
        {
            meshRenderer.material = MaterialHelper.CreateBasicTerrainMaterial();
        }
    }

    public void GenerateMap()
    {
        InitializeComponents();

        int powerOfTwo = Mathf.NextPowerOfTwo(mapSize - 1);
        mapSize = powerOfTwo + 1;

        float[,] heightMap = GenerateMidpointDisplacement(mapSize, mapSize);

        Texture2D texture = CreateTerrainTexture(heightMap);

        if (meshRenderer != null)
        {
            if (meshRenderer.material != null)
                meshRenderer.material.mainTexture = texture;
            else
            {
                Material newMaterial = MaterialHelper.CreateBasicTerrainMaterial();
                newMaterial.mainTexture = texture;
                meshRenderer.material = newMaterial;
            }
        }

        // mesh
        if (meshFilter != null)
            GenerateMesh(heightMap);

        // colisões
        if (generateColliders)
            GenerateColliders(heightMap);
    }

    // Midpoint Displacement
    private float[,] GenerateMidpointDisplacement(int width, int height)
    {
        float[,] heightMap = new float[width, height];
        System.Random random = new System.Random(seed);

        float baseHeight = baseLevel * initialHeight;
        for (int x = 0; x < width; x++)
        {
            heightMap[x, 0] = baseHeight;
        }

        heightMap[0, height - 1] = baseHeight + (float)random.NextDouble() * initialHeight;
        heightMap[width - 1, height - 1] = baseHeight + (float)random.NextDouble() * initialHeight;

        int stepSize = width - 1;
        float scale = initialHeight;

        while (stepSize > 1)
        {
            int halfStep = stepSize / 2;

            // Diamond step
            for (int y = halfStep; y < height; y += stepSize)
            {
                for (int x = halfStep; x < width; x += stepSize)
                {
                    float average =
                        (
                            heightMap[x - halfStep, y - halfStep]
                            + heightMap[x + halfStep, y - halfStep]
                            + heightMap[x - halfStep, y + halfStep]
                            + heightMap[x + halfStep, y + halfStep]
                        ) / 4f;

                    heightMap[x, y] = average + ((float)random.NextDouble() * 2 - 1) * scale;
                }
            }

            // Square step
            for (int y = 0; y < height; y += halfStep)
            {
                for (int x = (y + halfStep) % stepSize; x < width; x += stepSize)
                {
                    float average = 0;
                    int count = 0;

                    if (x - halfStep >= 0)
                    {
                        average += heightMap[x - halfStep, y];
                        count++;
                    }
                    if (x + halfStep < width)
                    {
                        average += heightMap[x + halfStep, y];
                        count++;
                    }
                    if (y - halfStep >= 0)
                    {
                        average += heightMap[x, y - halfStep];
                        count++;
                    }
                    if (y + halfStep < height)
                    {
                        average += heightMap[x, y + halfStep];
                        count++;
                    }

                    if (count > 0)
                    {
                        average /= count;
                        heightMap[x, y] = average + ((float)random.NextDouble() * 2 - 1) * scale;
                    }
                }
            }

            stepSize /= 2;
            scale *= roughness;
        }
        return NormalizeHeightMap(heightMap);
    }

    private float[,] NormalizeHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (heightMap[x, y] < minHeight)
                    minHeight = heightMap[x, y];
                if (heightMap[x, y] > maxHeight)
                    maxHeight = heightMap[x, y];
            }
        }

        // Normalizar preservando a base
        float range = maxHeight - minHeight;
        if (range > 0)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Manter a primeira linha (base)
                    if (y == 0)
                    {
                        heightMap[x, y] = baseLevel;
                    }
                    else
                    {
                        float normalizedValue = (heightMap[x, y] - minHeight) / range;
                        heightMap[x, y] = baseLevel + normalizedValue * (1f - baseLevel);

                        heightMap[x, y] = Mathf.Pow(heightMap[x, y], mountainHeight);
                    }
                }
            }
        }

        return heightMap;
    }

    private Texture2D CreateTerrainTexture(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);
        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float currentHeight = heightMap[x, y];
                Color terrainColor = Color.white;

                // Aplicar cores baseadas na altura
                if (regions != null && regions.Length > 0)
                {
                    for (int i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight <= regions[i].height)
                        {
                            terrainColor = regions[i].color;
                            break;
                        }
                    }
                }
                else
                {
                    // cores default
                    terrainColor = Color.Lerp(Color.black, Color.white, currentHeight);
                }

                colorMap[y * width + x] = terrainColor;
            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    private void GenerateMesh(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Vector3[] vertices = new Vector3[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];
        Vector2[] uvs = new Vector2[width * height];

        int vertIndex = 0;
        int triIndex = 0;

        // vértices e UVs
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                vertices[vertIndex] = new Vector3(
                    x * terrainScale,
                    heightMap[x, y] * terrainScale,
                    y * terrainScale
                );
                uvs[vertIndex] = new Vector2((float)x / (width - 1), (float)y / (height - 1));
                vertIndex++;
            }
        }

        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                int i = y * width + x;

                triangles[triIndex] = i;
                triangles[triIndex + 1] = i + width;
                triangles[triIndex + 2] = i + 1;

                triangles[triIndex + 3] = i + 1;
                triangles[triIndex + 4] = i + width;
                triangles[triIndex + 5] = i + width + 1;

                triIndex += 6;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private void GenerateColliders(float[,] heightMap)
    {
        if (meshCollider != null && meshFilter != null && meshFilter.mesh != null)
        {
            meshCollider.sharedMesh = meshFilter.mesh;
        }
        else
        {
            EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
            if (edgeCollider == null)
                edgeCollider = gameObject.AddComponent<EdgeCollider2D>();

            List<Vector2> points = new List<Vector2>();
            int width = heightMap.GetLength(0);

            for (int x = 0; x < width; x++)
            {
                float height = heightMap[x, 0] * terrainScale;
                points.Add(new Vector2(x * terrainScale, height));
            }

            edgeCollider.points = points.ToArray();
        }
    }

    void OnValidate()
    {
        if (mapSize < 3)
            mapSize = 3;

        int powerOfTwo = Mathf.NextPowerOfTwo(mapSize - 1);
        mapSize = powerOfTwo + 1;

        baseLevel = Mathf.Clamp01(baseLevel);

        if (autoUpdate && Application.isPlaying)
            GenerateMap();
    }

    public void RegenerateMap()
    {
        GenerateMap();
    }
}
