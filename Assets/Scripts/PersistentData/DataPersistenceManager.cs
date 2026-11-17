
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }

    public GameData gameData;
    public List<IDataPersistence> dataPersistanceObjects;

    [NonSerialized] public FileDataHandler coreDataHandler;

    public GameObject loadingScreen;
    public Image loadingImage;
    public Sprite losLogo, lozLogo;
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    public bool lozMode;
    public bool IsLoadingGame { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            coreDataHandler = new FileDataHandler(
                Application.persistentDataPath,
                "LOSCore.json"
            );

            DontDestroyOnLoad(gameObject);

            if (gameData != null && gameData.lozMode)
                lozMode = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --------------------------------------------------
    // SCENE LOADING
    // --------------------------------------------------

    public void LoadSceneWithData(string sceneName)
    {
        if (lozMode)
        {
            loadingImage.sprite = lozLogo;
            progressText.color = new Color32(100, 151, 53, 255);
        }
        else
        {
            loadingImage.sprite = losLogo;
            progressText.color = new Color32(250, 189, 0, 255);
        }

        progressBar.value = 0;
        progressText.text = "Loading...";
        loadingScreen.SetActive(true);

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        if (operation == null)
        {
            Debug.LogError($"Scene '{sceneName}' failed to load.");
            yield break;
        }

        operation.allowSceneActivation = false;

        float displayedProgress = 0f;
        float timer = 0f;
        const float timeout = 30f;

        while (!operation.isDone)
        {
            timer += Time.deltaTime;

            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            if (timer >= timeout)
            {
                Debug.LogError(
                    $"Scene loading stalled at {operation.progress:P0}."
                );

                yield break;
            }

            displayedProgress = Mathf.MoveTowards(
                displayedProgress,
                targetProgress * 0.8f,
                Time.deltaTime * 0.5f
            );

            progressBar.value = displayedProgress;
            progressText.text = $"Loading... {displayedProgress * 100:F0}%";

            if (targetProgress >= 1f)
            {
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log("Scene loaded. Starting data loading.");

        yield return StartCoroutine(LoadGameData());
    }

    // --------------------------------------------------
    // DATA LOADING
    // --------------------------------------------------

    public IEnumerator LoadGameData()
    {
        yield return StartCoroutine(LoadGame());

        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }

    public IEnumerator LoadGame()
    {
        IsLoadingGame = true;

        try
        {
            progressBar.value = 0.8f;
            progressText.text = "Loading Data...";

            gameData = coreDataHandler.Load();

            if (gameData == null)
            {
                Debug.Log("No save data found. Starting a new game.");
                NewGame();
            }

            // Ensure collections exist, including collections introduced
            // by newer versions of the save system.
            EnsureGameDataCollections();

            // Find persistence objects in the newly loaded scene.
            dataPersistanceObjects = FindAllDataPersistenceObjects();

            // Load in a deliberate order.
            List<IDataPersistence> orderedObjects =
                GetOrderedPersistenceObjects(dataPersistanceObjects);

            float step = orderedObjects.Count > 0
                ? 0.15f / orderedObjects.Count
                : 0f;

            float currentProgress = 0.8f;

            foreach (IDataPersistence persistenceObject in orderedObjects)
            {
                if (persistenceObject == null)
                    continue;

                Debug.Log(
                    $"Loading persistence object: {persistenceObject.GetType().Name}"
                );

                persistenceObject.LoadData(gameData);

                currentProgress += step;
                progressBar.value = currentProgress;

                progressText.text =
                    $"Loading Data... {currentProgress * 100:F0}%";

                // Allow Unity to finish instantiating and initializing
                // objects before the next manager loads.
                yield return null;
            }

            // Assign items only after both Soldiers and Items exist.
            if (ItemManager.Instance != null &&
                SoldierManager.Instance != null)
            {
                ItemManager.Instance.AssignItemsToOwners();
            }

            progressBar.value = 0.98f;

            // Initialize gameplay only after persistence has completed.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Init();
            }

            progressBar.value = 1f;
            progressText.text = "Loading Complete!";
        }
        finally
        {
            IsLoadingGame = false;
        }
    }

    // --------------------------------------------------
    // ORDERED PERSISTENCE
    // --------------------------------------------------

    private List<IDataPersistence> GetOrderedPersistenceObjects(
        List<IDataPersistence> objects)
    {
        List<IDataPersistence> ordered = new();

        // 1. Soldiers must exist before alerts reference them.
        AddPersistenceObjects<SoldierManager>(objects, ordered);

        // 2. Items can now load and resolve their references.
        AddPersistenceObjects<ItemManager>(objects, ordered);

        // 3. Load all remaining persistence objects.
        // XpAlertManager is included here, after SoldierManager.
        foreach (IDataPersistence obj in objects)
        {
            if (obj == null ||
                IsManagedPersistenceObject(obj) ||
                ordered.Contains(obj))
                continue;

            ordered.Add(obj);
        }

        return ordered;
    }

    private void AddPersistenceObjects<T>(
        List<IDataPersistence> source,
        List<IDataPersistence> destination)
    {
        foreach (IDataPersistence obj in source)
        {
            if (obj != null && obj is T && !destination.Contains(obj))
            {
                destination.Add(obj);
            }
        }
    }

    // --------------------------------------------------
    // GAME DATA INITIALIZATION
    // --------------------------------------------------

    private void EnsureGameDataCollections()
    {
        if (gameData.allSoldiersIds == null)
            gameData.allSoldiersIds = new List<string>();

        if (gameData.allSoldiersDetails == null)
            gameData.allSoldiersDetails =
                new Dictionary<string, Dictionary<string, object>>();

        if (gameData.allXpAlertIds == null)
            gameData.allXpAlertIds = new List<string>();

        if (gameData.allXpAlertDetails == null)
            gameData.allXpAlertDetails =
                new Dictionary<string, Dictionary<string, object>>();

        // Add other save collections here if they can be null
        // when loading older save files.
    }

    public void NewGame()
    {
        gameData = new GameData();
        EnsureGameDataCollections();
    }

    // --------------------------------------------------
    // SAVE
    // --------------------------------------------------

    public void SaveGame()
    {
        if (gameData == null)
        {
            Debug.LogError("Cannot save: GameData is null.");
            return;
        }

        dataPersistanceObjects = FindAllDataPersistenceObjects();

        foreach (IDataPersistence persistenceObject in dataPersistanceObjects)
        {
            if (persistenceObject == null ||
                IsManagedPersistenceObject(persistenceObject))
                continue;

            persistenceObject.SaveData(ref gameData);
        }

        coreDataHandler.Save(gameData);

        Debug.Log("Game saved successfully.");
        Debug.Log($"Saved XP Alerts: {gameData.allXpAlertIds?.Count ?? 0}");
        Debug.Log($"XP Alert Details: {gameData.allXpAlertDetails?.Count ?? 0}");
    }

    // --------------------------------------------------
    // FIND PERSISTENCE OBJECTS
    // --------------------------------------------------

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
    private bool IsManagedPersistenceObject(IDataPersistence obj)
    {
        return obj is Soldier ||
               obj is Item ||
               obj is POI ||
               obj is XpAlert;
    }
    // --------------------------------------------------
    // DYNAMIC OBJECT LOADING
    // --------------------------------------------------

    public async Task WaitForDynamicObjects(
        IProgress<float> progress = null)
    {
        float currentProgress = 0.9f;

        while (!AllObjectsLoaded())
        {
            currentProgress = Mathf.Lerp(
                currentProgress,
                1f,
                Time.deltaTime * 2f
            );

            progress?.Report(currentProgress);

            await Task.Yield();
        }

        await Task.Delay(500);
        progress?.Report(1f);
    }

    public bool AllObjectsLoaded()
    {
        dataPersistanceObjects = FindAllDataPersistenceObjects();

        foreach (IDataPersistence obj in dataPersistanceObjects)
        {
            if (obj == null)
                continue;

            if (!obj.IsDataLoaded)
            {
                Debug.Log(
                    $"DataPersistence object {obj.GetType().Name} is not loaded yet."
                );

                return false;
            }
        }

        return true;
    }
}
