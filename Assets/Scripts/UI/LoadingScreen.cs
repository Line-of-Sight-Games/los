using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;
    public GameObject loadingPanel;
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void LoadSceneWithData(string sceneName)
    {
        loadingPanel.SetActive(true); // Show loading UI
        progressBar.value = 0;
        progressText.text = "Loading...";

        await DataPersistenceManager.Instance.LoadGameData();
        await LoadSceneAsync(sceneName);

        loadingPanel.SetActive(false); // Hide loading UI when done
    }

    private async Task LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        float progress = 0f;

        while (!operation.isDone)
        {
            // Smoothly update the progress bar
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
            progress = Mathf.Lerp(progress, targetProgress, Time.deltaTime * 5);

            progressBar.value = progress;
            progressText.text = $"Loading... {progress * 100:F0}%";

            if (progress >= 0.99f)
            {
                await Task.Delay(500); // Small delay for a smooth transition
                operation.allowSceneActivation = true;
            }
            await Task.Yield();
        }
    }
}