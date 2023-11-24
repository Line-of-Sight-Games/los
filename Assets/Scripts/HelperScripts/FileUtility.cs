using System.IO;
using UnityEngine;

public static class FileUtility
{
    private static readonly string fileName = "BattleReport.txt";

    public static void WriteToReport(string message)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        // Check if the file exists, and create it if not
        if (!File.Exists(path))
        {
            using StreamWriter sw = File.CreateText(path);
            sw.WriteLine(message);
        }
        else
        {
            // Append a new line and the content to the existing file
            using StreamWriter sw = File.AppendText(path);
            sw.WriteLine(message);
        }
    }
}