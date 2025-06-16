using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        // Desenhar o inspector padrão sem auto-update automático
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        bool hasChanged = EditorGUI.EndChangeCheck();

        // Só gerar automaticamente se estiver em play mode e autoUpdate estiver ativo
        if (hasChanged && mapGen.autoUpdate && Application.isPlaying)
        {
            mapGen.GenerateMap();
        }

        GUILayout.Space(10);

        // Botões de controle
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate Map"))
        {
            try
            {
                mapGen.GenerateMap();
                EditorUtility.SetDirty(mapGen);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao gerar mapa: {e.Message}");
            }
        }

        if (GUILayout.Button("Randomize Seed"))
        {
            mapGen.seed = Random.Range(0, 10000);
            try
            {
                mapGen.GenerateMap();
                EditorUtility.SetDirty(mapGen);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao gerar mapa: {e.Message}");
            }
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Botões de preset
        EditorGUILayout.LabelField("Quick Presets:", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Moon"))
        {
            mapGen.regions = TerrainPresets.GetMoonPreset();
            mapGen.roughness = 0.8f;
            mapGen.baseLevel = 0.1f;
            EditorUtility.SetDirty(mapGen);
        }

        if (GUILayout.Button("Earth"))
        {
            mapGen.regions = TerrainPresets.GetEarthPreset();
            mapGen.roughness = 0.5f;
            mapGen.baseLevel = 0.3f;
            EditorUtility.SetDirty(mapGen);
        }

        if (GUILayout.Button("Mars"))
        {
            mapGen.regions = TerrainPresets.GetMarsPreset();
            mapGen.roughness = 0.6f;
            mapGen.baseLevel = 0.2f;
            EditorUtility.SetDirty(mapGen);
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Informações de ajuda
        EditorGUILayout.HelpBox(
            "Midpoint Displacement: Algoritmo fractal que cria terrenos naturais com base plana.\n\n" +
            "Parâmetros:\n" +
            "- Map Size: Potência de 2 + 1 (ex: 65, 129, 257, 513)\n" +
            "- Roughness: Controla a rugosidade do terreno (0.1 - 2.0)\n" +
            "- Initial Height: Altura máxima do terreno (0.1 - 5.0)\n" +
            "- Base Level: Nível da base plana (0.0 - 1.0)\n\n" +
            "Dica: Use os botões de preset para configurações rápidas!\n" +
            "Base Level baixo = terreno mais plano na base.",
            MessageType.Info
        );

        // Mostrar informações do mapa atual
        if (mapGen.regions != null && mapGen.regions.Length > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Regiões configuradas: {mapGen.regions.Length}");
            EditorGUILayout.LabelField($"Tamanho do mapa: {mapGen.mapSize}x{mapGen.mapSize}");
            EditorGUILayout.LabelField($"Base Level: {mapGen.baseLevel:F2}");
            EditorGUILayout.LabelField($"Roughness: {mapGen.roughness:F2}");
        }
    }
}
