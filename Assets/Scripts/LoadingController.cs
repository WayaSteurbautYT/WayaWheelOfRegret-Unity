using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingController : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI loadingText;
    public GameObject loadingSpinner;
    public float spinnerSpeed = 360f;
    
    [Header("Loading Settings")]
    public string[] loadingMessages = { "Loading...", "Preparing wheel...", "Calculating doom...", "Almost there..." };
    public float messageChangeInterval = 1f;
    
    private int currentMessageIndex = 0;
    private float messageTimer = 0f;
    private string targetScene = "";
    
    private void Start()
    {
        InitializeLoading();
    }
    
    private void InitializeLoading()
    {
        // Get target scene from GameManager or use default
        if (GameManager.Instance != null)
        {
            targetScene = GameManager.Instance.targetScene ?? "MainMenu";
        }
        else
        {
            targetScene = "MainMenu";
        }
        
        // Start loading sequence
        StartCoroutine(LoadSceneAsync());
    }
    
    private void Update()
    {
        // Rotate spinner
        if (loadingSpinner != null)
        {
            loadingSpinner.transform.Rotate(0, 0, -spinnerSpeed * Time.deltaTime);
        }
        
        // Update loading messages
        messageTimer += Time.deltaTime;
        if (messageTimer >= messageChangeInterval)
        {
            messageTimer = 0f;
            currentMessageIndex = (currentMessageIndex + 1) % loadingMessages.Length;
            
            if (loadingText != null)
            {
                loadingText.text = loadingMessages[currentMessageIndex];
            }
        }
    }
    
    private IEnumerator LoadSceneAsync()
    {
        // Start loading the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        asyncLoad.allowSceneActivation = false;
        
        // Wait until the scene is loaded
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            // Update loading text with progress
            if (loadingText != null)
            {
                loadingText.text = $"{loadingMessages[currentMessageIndex]} {Mathf.RoundToInt(progress * 100)}%";
            }
            
            // When progress is complete, wait a moment before activating
            if (progress >= 1f)
            {
                yield return new WaitForSeconds(0.5f);
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }
    
    public void SetTargetScene(string sceneName)
    {
        targetScene = sceneName;
    }
}
