using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultsController : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI starsText;
    public TextMeshProUGUI ratingText;
    public TextMeshProUGUI creativityText;
    public TextMeshProUGUI stupidityText;
    public TextMeshProUGUI totalPointsText;
    
    [Header("Navigation")]
    public Button playAgainButton;
    public Button menuButton;
    public Button creditsButton;
    
    [Header("Stars Display")]
    public GameObject[] starObjects;
    
    private void Start()
    {
        InitializeResults();
    }
    
    private void InitializeResults()
    {
        // Setup navigation buttons
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(OnPlayAgain);
        }
            
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenu);
        }
            
        if (creditsButton != null)
        {
            creditsButton.onClick.AddListener(OnCredits);
        }
        
        // Load and display results
        if (GameManager.Instance != null && GameManager.Instance.currentSession != null)
        {
            DisplayResults(GameManager.Instance.currentSession);
        }
    }
    
    private void DisplayResults(GameSession session)
    {
        if (session == null || session.finalScore == null) return;
        
        var score = session.finalScore;
        
        // Display score values
        if (scoreText != null)
        {
            scoreText.text = $"Final Score: {score.totalPoints}";
        }
            
        if (totalPointsText != null)
        {
            totalPointsText.text = $"Total Points: {score.totalPoints}";
        }
            
        if (creativityText != null)
        {
            creativityText.text = $"Creativity: {score.creativityPoints}";
        }
            
        if (stupidityText != null)
        {
            stupidityText.text = $"Stupidity: {score.stupidityPoints}";
        }
        
        // Display stars
        DisplayStars(score.stars);
        
        // Display rating
        if (ratingText != null)
        {
            ratingText.text = GetRatingText(score.totalPoints);
        }
    }
    
    private void DisplayStars(int starCount)
    {
        // Update stars display
        for (int i = 0; i < starObjects.Length; i++)
        {
            if (starObjects[i] != null)
            {
                starObjects[i].SetActive(i < starCount);
            }
        }
        
        if (starsText != null)
        {
            starsText.text = $"{starCount} Stars";
        }
    }
    
    private string GetRatingText(int score)
    {
        return score switch
        {
            > 200 => "Legendary chaos!",
            > 150 => "Impressive regret!",
            > 100 => "Decent doom!",
            > 50 => "Minor regret",
            > 0 => "You tried!",
            _ => "Complete failure!"
        };
    }
    
    private void OnPlayAgain()
    {
        // Reset game session
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentSession = null;
        }
        
        // Load game scene
        SceneManager.LoadScene("GameScene");
    }
    
    private void OnMenu()
    {
        // Reset game session
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentSession = null;
        }
        
        // Load main menu
        SceneManager.LoadScene("MainMenu");
    }
    
    private void OnCredits()
    {
        SceneManager.LoadScene("CreditsScene");
    }
}
