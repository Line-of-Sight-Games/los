
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class FileDataHandler
{
    private string path = "";
    private string fileName = "";

    // Converts Unity Vector3 values to and from JSON using only x, y and z.
    private class Vector3Converter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3);
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            Vector3 vector = (Vector3)value;

            writer.WriteStartObject();

            writer.WritePropertyName("x");
            writer.WriteValue(vector.x);

            writer.WritePropertyName("y");
            writer.WriteValue(vector.y);

            writer.WritePropertyName("z");
            writer.WriteValue(vector.z);

            writer.WriteEndObject();
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return Vector3.zero;

            JObject obj = JObject.Load(reader);

            return new Vector3(
                obj["x"] != null ? obj["x"].Value<float>() : 0f,
                obj["y"] != null ? obj["y"].Value<float>() : 0f,
                obj["z"] != null ? obj["z"].Value<float>() : 0f
            );
        }
    }

    private readonly JsonSerializerSettings jsonSettings =
        new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = { new Vector3Converter() }
        };

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
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonConvert.DeserializeObject<GameData>(
                    dataToLoad,
                    jsonSettings
                );
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    "Error occurred when trying to load data from file: "
                    + fullPath + "\n" + ex
                );
            }
        }

        return loadedData;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(path, fileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonConvert.SerializeObject(
                data,
                jsonSettings
            );

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(
                "Error occurred when trying to save data to file: "
                + fullPath + "\n" + ex
            );
        }
    }

    public void Delete()
    {
        string fullPath = Path.Combine(path, fileName);
        File.Delete(fullPath);
    }
}
