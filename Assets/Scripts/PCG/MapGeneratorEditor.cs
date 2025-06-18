#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;

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

        // Só gerar automaticamente se estiver em play mode e autoUpdate estiver ativo, usando reflexão para acessar autoUpdate
        FieldInfo autoUpdateField = typeof(MapGenerator).GetField("autoUpdate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        bool autoUpdate = autoUpdateField != null && (bool)autoUpdateField.GetValue(mapGen);

        if (hasChanged && autoUpdate && Application.isPlaying)
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
            FieldInfo seedField = typeof(MapGenerator).GetField("seed", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            seedField?.SetValue(mapGen, Random.Range(0, 10000));

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

        FieldInfo regionsField = typeof(MapGenerator).GetField("regions", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo roughnessField = typeof(MapGenerator).GetField("roughness", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo baseLevelField = typeof(MapGenerator).GetField("baseLevel", BindingFlags.NonPublic | BindingFlags.Instance);

        if (GUILayout.Button("Moon"))
        {

            regionsField?.SetValue(mapGen, TerrainPresets.GetMoonPreset());
            roughnessField?.SetValue(mapGen, 0.4f);
            baseLevelField?.SetValue(mapGen, 0.1f);
            EditorUtility.SetDirty(mapGen);
        }

        if (GUILayout.Button("Earth"))
        {
            regionsField?.SetValue(mapGen, TerrainPresets.GetEarthPreset());
            roughnessField?.SetValue(mapGen, 0.5f);
            baseLevelField?.SetValue(mapGen, 0.3f);
            EditorUtility.SetDirty(mapGen);
        }

        if (GUILayout.Button("Mars"))
        {
            regionsField?.SetValue(mapGen, TerrainPresets.GetMarsPreset());
            roughnessField?.SetValue(mapGen, 0.6f);
            baseLevelField?.SetValue(mapGen, 0.2f);
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
        if (regionsField.GetValue(mapGen) != null && ((TerrainType[])regionsField.GetValue(mapGen)).Length > 0)
        {
            FieldInfo mapSizeField = typeof(MapGenerator).GetField("mapSize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            int mapSizeValue = mapSizeField != null ? (int)mapSizeField.GetValue(mapGen) : 0;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Regiões configuradas: {((TerrainType[])regionsField.GetValue(mapGen)).Length}");
            EditorGUILayout.LabelField($"Tamanho do mapa: {mapSizeValue}x{mapSizeValue}");
            EditorGUILayout.LabelField($"Base Level: {baseLevelField.GetValue(mapGen):F2}");
            EditorGUILayout.LabelField($"Roughness: {roughnessField.GetValue(mapGen):F2}");
        }
    }
}
# endif
