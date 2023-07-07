using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string coreFileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistanceObjects;
    private FileDataHandler coreDataHandler;


    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Found more than one Data Persistence Manager in the scene.");
        }
        Instance = this;
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
            Debug.Log("No data was found. Starting new game.");
            NewGame();
        }
        //Push loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistanceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        //Debug.Log("Saving game");
        dataPersistanceObjects = FindAllDataPersistenceObjects();
        //pass data to other scripts to update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistanceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        //save that data to a file using the data handler

        coreDataHandler.Save(gameData);
    }

    public void SaveAndLoad()
    {
        SaveGame();
        LoadGame();
    }

    private void Start()
    {
        coreDataHandler = new FileDataHandler(Application.persistentDataPath, coreFileName);
        LoadGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
