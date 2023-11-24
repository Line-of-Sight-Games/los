using System.IO;
using UnityEngine;

public static class FileUtility
{
    public static void WriteToReport(string content)
    {
        string path = "Assets/Resources/Report.txt";

        // Check if the file exists, and create it if not
        if (!File.Exists(path))
            File.WriteAllText(path, content);
        else
            File.AppendAllText(path, "\n" + content);

        // Refresh the asset database (important for the Editor)
        UnityEditor.AssetDatabase.Refresh();
    }
}