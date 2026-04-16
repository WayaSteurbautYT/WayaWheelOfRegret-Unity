using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditsController : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI creditsText;
    public Button backButton;
    public Button youtubeButton;
    
    [Header("Credits Data")]
    public CreditEntry[] creditEntries;
    
    [System.Serializable]
    public class CreditEntry
    {
        public string role = "";
        public string name = "";
        public float delay = 0f;
    }
    
    private int currentCreditIndex = 0;
    
    private void Start()
    {
        InitializeCredits();
    }
    
    private void InitializeCredits()
    {
        // Setup button listeners
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
        
        if (youtubeButton != null)
        {
            youtubeButton.onClick.AddListener(OnYouTubeClicked);
        }
        
        // Start credits sequence
        if (creditEntries != null && creditEntries.Length > 0)
        {
            StartCoroutine(PlayCreditsSequence());
        }
    }
    
    private IEnumerator PlayCreditsSequence()
    {
        currentCreditIndex = 0;
        
        while (currentCreditIndex < creditEntries.Length)
        {
            var entry = creditEntries[currentCreditIndex];
            
            if (creditsText != null)
            {
                creditsText.text = $"{entry.role}\n{entry.name}";
            }
            
            yield return new WaitForSeconds(entry.delay);
            
            currentCreditIndex++;
        }
        
        // Show final message
        if (creditsText != null)
        {
            creditsText.text = "Thank you for playing!\n\n© 2024 WayaCreate\n\nSubscribe for more!";
        }
        
        yield return new WaitForSeconds(3f);
    }
    
    private void OnBackClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    private void OnYouTubeClicked()
    {
        Application.OpenURL("https://www.youtube.com/@WayaCreate");
    }
    
    public void RestartCredits()
    {
        StopAllCoroutines();
        currentCreditIndex = 0;
        
        if (creditsText != null)
        {
            creditsText.text = "";
        }
        
        StartCoroutine(PlayCreditsSequence());
    }
}
