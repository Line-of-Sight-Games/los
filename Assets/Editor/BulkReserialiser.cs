using UnityEditor;
using UnityEngine;
using System.Linq;

public class BulkReserialize
{
    [MenuItem("Tools/Serialization/Bulk Upgrade All Metadata")]
    public static void UpgradeAllMetadata()
    {
        // Get every asset inside Assets/
        var assetPaths = AssetDatabase.GetAllAssetPaths()
            .Where(path => path.StartsWith("Assets/"))
            .Where(path => !path.EndsWith(".meta"))
            .ToArray();

        Debug.Log($"Upgrading metadata for {assetPaths.Length} assets...");

        // Upgrade .meta files only (PluginImporter, TextureImporter, etc.)
        AssetDatabase.ForceReserializeAssets(
            assetPaths,
            ForceReserializeAssetsOptions.ReserializeMetadata
        );

        Debug.Log("Metadata bulk upgrade complete!");
    }

    [MenuItem("Tools/Serialization/Bulk Upgrade Physics Settings")]
    public static void UpgradePhysicsSettings()
    {
        AssetDatabase.ForceReserializeAssets(
            new[] { "ProjectSettings/DynamicsManager.asset" }
        );

        Debug.Log("Physics settings upgraded!");
    }

    [MenuItem("Tools/Serialization/Upgrade TMP Prefabs")]
    public static void UpgradeTMPPrefabs()
    {
        string[] prefabs =
        {
            "Assets/TextMesh Pro/Examples & Extras/Prefabs/Text Popup.prefab",
            "Assets/TextMesh Pro/Examples & Extras/Prefabs/TextMeshPro - Prefab 1.prefab",
            "Assets/TextMesh Pro/Examples & Extras/Prefabs/TextMeshPro - Prefab 2.prefab"
        };

        AssetDatabase.ForceReserializeAssets(
            prefabs,
            ForceReserializeAssetsOptions.ReserializeAssets
        );

        Debug.Log("TextMesh Pro prefabs successfully reserialized!");
    }
}
