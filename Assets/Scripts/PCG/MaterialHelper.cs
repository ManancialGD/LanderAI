using UnityEngine;

public static class MaterialHelper
{
    public static Material CreateBasicTerrainMaterial()
    {
        Material material = new Material(Shader.Find("Standard"));
        material.name = "TerrainMaterial";

        material.SetFloat("_Metallic", 0f);
        material.SetFloat("_Smoothness", 0.1f);
        material.color = Color.gray;

        return material;
    }

    public static Material CreateLunarMaterial()
    {
        Material material = new Material(Shader.Find("Standard"));
        material.name = "LunarMaterial";

        material.SetFloat("_Metallic", 0f);
        material.SetFloat("_Smoothness", 0.05f);
        material.color = new Color(0.6f, 0.6f, 0.6f);

        return material;
    }

    public static Material CreateMartianMaterial()
    {
        Material material = new Material(Shader.Find("Standard"));
        material.name = "MartianMaterial";

        material.SetFloat("_Metallic", 0f);
        material.SetFloat("_Smoothness", 0.2f);
        material.color = new Color(0.8f, 0.4f, 0.2f);

        return material;
    }

    public static Material CreateProceduralMaterial(Color baseColor, float roughness = 0.8f)
    {
        Material material = new Material(Shader.Find("Standard"));
        material.name = "ProceduralMaterial";

        material.SetFloat("_Metallic", 0f);
        material.SetFloat("_Smoothness", 1f - roughness);
        material.color = baseColor;

        return material;
    }

    public static void ConfigureMaterialForTerrain(Material material, string terrainType)
    {
        switch (terrainType.ToLower())
        {
            case "moon":
            case "lunar":
                material.SetFloat("_Metallic", 0f);
                material.SetFloat("_Smoothness", 0.05f);
                material.color = new Color(0.6f, 0.6f, 0.6f);
                break;

            case "mars":
            case "martian":
                material.SetFloat("_Metallic", 0f);
                material.SetFloat("_Smoothness", 0.2f);
                material.color = new Color(0.8f, 0.4f, 0.2f);
                break;

            case "earth":
            case "terrestrial":
                material.SetFloat("_Metallic", 0f);
                material.SetFloat("_Smoothness", 0.3f);
                material.color = new Color(0.4f, 0.6f, 0.3f);
                break;

            default:
                material.SetFloat("_Metallic", 0f);
                material.SetFloat("_Smoothness", 0.1f);
                material.color = Color.gray;
                break;
        }
    }
}
