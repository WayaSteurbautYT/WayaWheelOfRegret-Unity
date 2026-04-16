using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI ratingText;
    public Button playAgainButton;
    public Button menuButton;
    public Button creditsButton;

    [Header("Presentation")]
    public GameObject presentationCanvas;
    public TextMeshProUGUI slideTitle;
    public Button skipButton;

    private GameSession session;
    private int currentSlide = 0;
    private bool presentationComplete = false;

    private void Start()
    {
        session = GameManager.Instance?.currentSession;
        if (session == null)
        {
            Debug.LogError("No game session found!");
            return;
        }

        InitializeResults();
        StartCoroutine(PresentationSequence());
    }

    private void InitializeResults()
    {
        if (skipButton != null) skipButton.onClick.AddListener(SkipPresentation);
        if (playAgainButton != null) playAgainButton.onClick.AddListener(PlayAgain);
        if (menuButton != null) menuButton.onClick.AddListener(GoToMenu);
        if (creditsButton != null) creditsButton.onClick.AddListener(GoToCredits);

        // Hide final UI initially
        if (finalScoreText != null) finalScoreText.gameObject.SetActive(false);
        if (ratingText != null) ratingText.gameObject.SetActive(false);
        if (playAgainButton != null) playAgainButton.gameObject.SetActive(false);
        if (menuButton != null) menuButton.gameObject.SetActive(false);
        if (creditsButton != null) creditsButton.gameObject.SetActive(false);
    }

    private IEnumerator PresentationSequence()
    {
        string[] slides = {
            "YOUR FATE",
            "has been decided...",
            $"QUESTIONS ASKED: {session.spins.Count}",
            $"STAR RATING: {session.finalScore.stars}/5",
            $"REGRET LEVEL: {session.finalScore.regretLevel}%",
            $"CREATIVITY: {session.finalScore.creativityPoints} points",
            $"STUPIDITY: {session.finalScore.stupidityPoints} points",
            $"TOTAL SCORE: {session.finalScore.totalPoints}",
            GetFinalMessage()
        };

        while (currentSlide < slides.Length && !presentationComplete)
        {
            yield return StartCoroutine(ShowSlide(slides[currentSlide]));
            
            if (!presentationComplete)
            {
                yield return new WaitForSeconds(2f);
            }
            
            currentSlide++;
        }

        ShowFinalResults();
    }

    private IEnumerator ShowSlide(string text)
    {
        if (slideTitle != null)
        {
            slideTitle.text = text;
            slideTitle.gameObject.SetActive(true);
            StartCoroutine(FadeInAnimation(slideTitle, 1f));
        }

        SoundManager.Instance?.PlayClick();
        yield return null;
    }

    private void ShowFinalResults()
    {
        presentationComplete = true;

        if (presentationCanvas != null) presentationCanvas.SetActive(false);
        if (skipButton != null) skipButton.gameObject.SetActive(false);

        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {session.finalScore.totalPoints}";
            finalScoreText.gameObject.SetActive(true);
        }

        if (ratingText != null)
        {
            ratingText.text = GetRatingText(session.finalScore.totalPoints);
            ratingText.gameObject.SetActive(true);
        }

        if (playAgainButton != null) playAgainButton.gameObject.SetActive(true);
        if (menuButton != null) menuButton.gameObject.SetActive(true);
        if (creditsButton != null) creditsButton.gameObject.SetActive(true);
    }

    private string GetFinalMessage()
    {
        if (!string.IsNullOrEmpty(session.detectedPersonality))
        {
            var personality = PersonalityDetector.DetectPersonality(session.username);
            if (personality != null)
            {
                return $"SPECIAL ENDING: {personality.name}\n{personality.trait}";
            }
        }

        return "GAME COMPLETE";
    }

    private string GetRatingText(int score)
    {
        return score switch
        {
            > 200 => "Legendary chaos!",
            > 150 => "Impressive regret!",
            > 100 => "Acceptable doom.",
            _ => "The wheel is disappointed."
        };
    }

    private void SkipPresentation()
    {
        presentationComplete = true;
        SoundManager.Instance?.PlayClick();
    }

    private void PlayAgain()
    {
        GameManager.Instance?.ResetGame();
    }

    private void GoToMenu()
    {
        GameManager.Instance?.GoToMenu();
    }

    private void GoToCredits()
    {
        GameManager.Instance?.GoToCredits();
    }

    private IEnumerator FadeInAnimation(TextMeshProUGUI target, float duration)
    {
        Color originalColor = target.color;
        target.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            target.color = new Color(originalColor.r, originalColor.g, originalColor.b, progress);
            yield return null;
        }

        target.color = originalColor;
    }
}
