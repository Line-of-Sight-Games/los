#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class BatchAudioSettings
{
    [MenuItem("Tools/Audio/Set Recommended Settings For All Clips")]
    public static void SetAudioSettings()
    {
        string[] guids = AssetDatabase.FindAssets("t:AudioClip");

        int modifiedCount = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;

            if (importer == null)
                continue;

            AudioImporterSampleSettings settings = importer.defaultSampleSettings;

            // Modify settings
            settings.loadType = AudioClipLoadType.Streaming;
            settings.compressionFormat = AudioCompressionFormat.Vorbis;
            settings.quality = 0.5f; // 0 = lowest, 1 = highest
            settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;

            importer.defaultSampleSettings = settings;

            // Set flags
            importer.loadInBackground = true;

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            modifiedCount++;
        }

        Debug.Log($"[BatchAudioSettings] Updated {modifiedCount} audio clips.");
    }
}
#endif
