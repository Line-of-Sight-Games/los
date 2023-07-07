using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;

public class FileDataHandler
{
    private string path = "";
    private string fileName = "";

    public FileDataHandler(string path, string fileName)
    {
        this.path = path;
        this.fileName = fileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(path, fileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                //load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //deserialize the data from JSON back into C# gamedata
                loadedData = JsonConvert.DeserializeObject<GameData>(dataToLoad);
            }
            catch (Exception ex)
            {
                Debug.Log("Error occured when trying to load data from file: " + fullPath + "\n" + ex);
            }
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(path, fileName);
        try
        {
            //create directory path in case it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //serialize gamedata into JSON string
            string dataToStore = JsonConvert.SerializeObject(data, Formatting.Indented);

            //write the data to file
            using (FileStream stream = new FileStream(fullPath,FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Error occured when trying to save data to file: " + fullPath + "\n" + ex);
        }
    }

    public void Delete()
    {
        string fullPath = Path.Combine(path, fileName);
        File.Delete(fullPath);
    }
}
