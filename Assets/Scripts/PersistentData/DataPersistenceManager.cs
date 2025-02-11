using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;
using System.Threading.Tasks;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }
    public GameData gameData;
    public List<IDataPersistence> dataPersistanceObjects;
    public FileDataHandler coreDataHandler;
    public MainGame game;
    public ItemManager itemManager;
    public bool dataLoaded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            coreDataHandler = new(Application.persistentDataPath, "LOSCore.json");
            // Load the Loading Screen scene additively
            if (SceneManager.GetSceneByName("LoadingScreen").isLoaded == false)
                SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);

            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Battlefield"))
            {
                game = FindFirstObjectByType<MainGame>();
                itemManager = FindFirstObjectByType<ItemManager>();
            }
        }
        else
            Destroy(gameObject);
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
        LoadGame();

        dataLoaded = true;
    }
    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        dataPersistanceObjects = FindAllDataPersistenceObjects();
        gameData = coreDataHandler.Load();
        //Load any saved data from data handler
        if (gameData == null)
        {
            print("No data was found. Starting new game.");
            NewGame();
        }
        //Push loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistanceObjects)
            dataPersistenceObj.LoadData(gameData);

        if (itemManager != null)
            itemManager.AssignItemsToOwners();

        if (game != null)
            game.Init();
    }

    public void SaveGame()
    {
        //print("Saving game");
        dataPersistanceObjects = FindAllDataPersistenceObjects();
        //pass data to other scripts to update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistanceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        //save that data to a file using the data handler

        coreDataHandler.Save(gameData);
    }
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(default).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
