using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;
using System.Threading.Tasks;
using System;
using TMPro;
using UnityEngine.UI;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }
    public GameData gameData;
    public List<IDataPersistence> dataPersistanceObjects;
    public FileDataHandler coreDataHandler;
    public GameObject loadingScreen;
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            coreDataHandler = new(Application.persistentDataPath, "LOSCore.json");
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    public void LoadSceneWithData(string sceneName)
    {
        loadingScreen.SetActive(true);
        progressBar.value = 0;
        progressText.text = "Loading...";

        StartCoroutine(LoadSceneAsync(sceneName));
    }
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        if (operation == null)
        {
            Debug.LogError($"Scene '{sceneName}' failed to load. Check build settings.");
            yield break;
        }

        operation.allowSceneActivation = false;

        float displayedProgress = 0f;
        float timer = 0f;
        float timeout = 30f; // seconds

        while (!operation.isDone)
        {
            timer += Time.deltaTime;

            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Timeout safety
            if (timer >= timeout)
            {
                Debug.LogError($"Scene load stalled at {operation.progress:P0}. Timeout hit.");
                yield break;
            }

            // Visual progress bar
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress * 0.8f, Time.deltaTime * 0.5f);
            progressBar.value = displayedProgress;
            progressText.text = $"Loading... {displayedProgress * 100:F0}%";

            if (targetProgress >= 1f)
            {
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log("Scene load complete. Proceeding to data load.");
        StartCoroutine(LoadGameData());
    }

    public IEnumerator LoadGameData()
    {
        string path = Path.Combine(Application.persistentDataPath, "LOSCore.json");

        if (!File.Exists(path))
        {
            Debug.LogError("JSON file not found!");
            progressBar.value = 1f;
            progressText.text = "Load Failed!";
            yield break;
        }

        progressBar.value = 0.8f; // Scene loaded, start data loading

        string jsonText = File.ReadAllText(path);
        gameData = JsonUtility.FromJson<GameData>(jsonText);
        progressBar.value = 0.85f;
        progressText.text = "Loading Data...";

        yield return StartCoroutine(LoadGame());

        loadingScreen.SetActive(false);
    }
    public IEnumerator LoadGame()
    {
        dataPersistanceObjects = FindAllDataPersistenceObjects();
        gameData = coreDataHandler.Load();

        if (gameData == null)
        {
            print("No data was found. Starting new game.");
            NewGame();
        }

        float step = 0.15f / dataPersistanceObjects.Count; // 15% for game data
        float currentProgress = 0.85f;

        foreach (IDataPersistence dataPersistenceObj in dataPersistanceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
            currentProgress += step;
            progressBar.value = currentProgress;
            progressText.text = $"Loading Data... {currentProgress * 100:F0}%";
            yield return null;
        }

        ItemManager itemManager = FindFirstObjectByType<ItemManager>();
        if (itemManager != null)
        {
            itemManager.AssignItemsToOwners();
            progressBar.value = 0.98f;
        }

        MainGame mainGame = FindFirstObjectByType<MainGame>();
        if (mainGame != null)
        {
            mainGame.Init();
            progressBar.value = 1f;
        }
    }
    public void NewGame()
    {
        gameData = new GameData();
    }

    public async Task WaitForDynamicObjects(IProgress<float> progress = null)
    {
        float startProgress = 0.9f;
        float endProgress = 1.0f;
        float currentProgress = startProgress;

        while (!AllObjectsLoaded())
        {
            currentProgress = Mathf.Lerp(currentProgress, endProgress, Time.deltaTime * 2);
            progress?.Report(currentProgress);
            await Task.Yield();
        }

        // Ensure it reaches 100%
        await Task.Delay(500);
        progress?.Report(1f);
    }

    private bool AllObjectsLoaded()
    {
        dataPersistanceObjects = FindAllDataPersistenceObjects(); // Refresh list
        foreach (var obj in dataPersistanceObjects)
        {
            if (!obj.IsDataLoaded) // Implement this property in IDataPersistence
            {
                return false;
            }
        }
        return true;
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
