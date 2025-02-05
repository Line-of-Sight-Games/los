using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static class FileUtility
{
    private static readonly string fileName = "BattleReport.txt";

    public static void WriteToReport(string message)
    {
        message = CleanMessage(message);

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

    public static string CleanMessage(string message)
    {
        return Regex.Replace(message, "<.*?>", string.Empty);
    }
}