using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameData gameData;
    public bool dataLoaded = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load the Loading Screen scene additively
            if (SceneManager.GetSceneByName("LoadingScreen").isLoaded == false)
            {
                SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async Task LoadGameData()
    {
        if (dataLoaded) return; // Prevent reloading on scene changes

        string path = Path.Combine(Application.persistentDataPath, "LOSCore.json");

        if (!File.Exists(path))
        {
            Debug.LogError("JSON file not found!");
            return;
        }

        string jsonText = await File.ReadAllTextAsync(path);
        gameData = JsonUtility.FromJson<GameData>(jsonText);

        dataLoaded = true;
    }
}
