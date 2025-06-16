using UnityEngine;

[CreateAssetMenu(fileName = "TerrainPreset", menuName = "PCG/Terrain Preset")]
public class TerrainPresets : ScriptableObject
{
    [Header("Terrain Regions")]
    public TerrainType[] regions;

    [Header("Generation Settings")]
    public float roughness = 0.7f;
    public float initialHeight = 2f;
    public float mountainHeight = 1.5f;
    public float terrainScale = 10f;
    public float baseLevel = 0.2f;

    public void ApplyToGenerator(MapGenerator generator)
    {
        generator.regions = regions;
        generator.roughness = roughness;
        generator.initialHeight = initialHeight;
        generator.mountainHeight = mountainHeight;
        generator.terrainScale = terrainScale;
        generator.baseLevel = baseLevel;
    }

    public static TerrainType[] GetMoonPreset()
    {
        return new TerrainType[]
        {
            new TerrainType
            {
                name = "Deep Crater",
                height = 0.2f,
                color = new Color(0.1f, 0.1f, 0.1f),
            },
            new TerrainType
            {
                name = "Crater",
                height = 0.4f,
                color = new Color(0.2f, 0.2f, 0.2f),
            },
            new TerrainType
            {
                name = "Low Ground",
                height = 0.6f,
                color = new Color(0.4f, 0.4f, 0.4f),
            },
            new TerrainType
            {
                name = "High Ground",
                height = 0.8f,
                color = new Color(0.6f, 0.6f, 0.6f),
            },
            new TerrainType
            {
                name = "Peak",
                height = 1.0f,
                color = new Color(0.8f, 0.8f, 0.8f),
            },
        };
    }

    public static TerrainType[] GetEarthPreset()
    {
        return new TerrainType[]
        {
            new TerrainType
            {
                name = "Water",
                height = 0.3f,
                color = new Color(0.2f, 0.4f, 0.8f),
            },
            new TerrainType
            {
                name = "Beach",
                height = 0.35f,
                color = new Color(0.9f, 0.8f, 0.6f),
            },
            new TerrainType
            {
                name = "Grass",
                height = 0.6f,
                color = new Color(0.3f, 0.7f, 0.2f),
            },
            new TerrainType
            {
                name = "Forest",
                height = 0.8f,
                color = new Color(0.2f, 0.5f, 0.1f),
            },
            new TerrainType
            {
                name = "Mountain",
                height = 0.9f,
                color = new Color(0.5f, 0.4f, 0.3f),
            },
            new TerrainType
            {
                name = "Snow",
                height = 1.0f,
                color = new Color(0.9f, 0.9f, 0.9f),
            },
        };
    }

    public static TerrainType[] GetMarsPreset()
    {
        return new TerrainType[]
        {
            new TerrainType
            {
                name = "Deep Valley",
                height = 0.3f,
                color = new Color(0.4f, 0.2f, 0.1f),
            },
            new TerrainType
            {
                name = "Valley",
                height = 0.5f,
                color = new Color(0.6f, 0.3f, 0.2f),
            },
            new TerrainType
            {
                name = "Plains",
                height = 0.7f,
                color = new Color(0.8f, 0.4f, 0.2f),
            },
            new TerrainType
            {
                name = "Hills",
                height = 0.85f,
                color = new Color(0.7f, 0.3f, 0.1f),
            },
            new TerrainType
            {
                name = "Mountains",
                height = 1.0f,
                color = new Color(0.5f, 0.2f, 0.1f),
            },
        };
    }
}
