using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    [Header("Presentation Components")]
    public GameObject presentationCanvas;
    public Image appIcon;
    public TextMeshProUGUI slideTitle;
    public TextMeshProUGUI slideSubtitle;
    public GameObject slideContent;
    public Button skipButton;

    [Header("Result Components")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI ratingText;
    public Button playAgainButton;
    public Button viewCreditsButton;

    [Header("Visual Effects")]
    public GameObject starContainer;
    public Image starPrefab;
    public Image progressBar;
    public TextMeshProUGUI percentageText;
    public Color goldColor = Color.yellow;
    public Color redColor = Color.red;
    public Color purpleColor = new Color(0.545f, 0f, 1f, 1f); // #8B00FF

    private int currentSlide = 0;
    private bool presentationComplete = false;
    private GameSession session;

    // Slide types
    private enum SlideType { Intro, Summary, Stars, Regret, Creativity, Stupidity, Total, Personality, Final }

    private void Start()
    {
        session = GameManager.Instance.currentSession;
        InitializePresentation();
        StartCoroutine(PresentationSequence());
    }

    private void InitializePresentation()
    {
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipToNextSlide);
        }

        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(PlayAgain);
        }

        if (viewCreditsButton != null)
        {
            viewCreditsButton.onClick.AddListener(ViewCredits);
        }

        // Hide final UI initially
        if (finalScoreText != null) finalScoreText.gameObject.SetActive(false);
        if (ratingText != null) ratingText.gameObject.SetActive(false);
        if (playAgainButton != null) playAgainButton.gameObject.SetActive(false);
        if (viewCreditsButton != null) viewCreditsButton.gameObject.SetActive(false);
    }

    private IEnumerator PresentationSequence()
    {
        while (currentSlide < (int)SlideType.Final && !presentationComplete)
        {
            yield return StartCoroutine(ShowSlide((SlideType)currentSlide));
            
            if (!presentationComplete)
            {
                yield return new WaitForSeconds(GetSlideDuration((SlideType)currentSlide));
            }
            
            currentSlide++;
        }

        // Show final UI
        ShowFinalResults();
    }

    private IEnumerator ShowSlide(SlideType slideType)
    {
        // Clear previous content
        ClearSlideContent();

        // Play transition sound
        SoundManager.Instance?.PlayWhoosh();

        switch (slideType)
        {
            case SlideType.Intro:
                yield return StartCoroutine(ShowIntroSlide());
                break;
            case SlideType.Summary:
                yield return StartCoroutine(ShowSummarySlide());
                break;
            case SlideType.Stars:
                yield return StartCoroutine(ShowStarsSlide());
                break;
            case SlideType.Regret:
                yield return StartCoroutine(ShowRegretSlide());
                break;
            case SlideType.Creativity:
                yield return StartCoroutine(ShowCreativitySlide());
                break;
            case SlideType.Stupidity:
                yield return StartCoroutine(ShowStupiditySlide());
                break;
            case SlideType.Total:
                yield return StartCoroutine(ShowTotalSlide());
                break;
            case SlideType.Personality:
                yield return StartCoroutine(ShowPersonalitySlide());
                break;
        }
    }

    private IEnumerator ShowIntroSlide()
    {
        if (appIcon != null)
        {
            appIcon.gameObject.SetActive(true);
            StartCoroutine(ScaleInAnimation(appIcon.transform, 2.5f));
        }

        if (slideTitle != null)
        {
            slideTitle.text = "YOUR FATE";
            slideTitle.gameObject.SetActive(true);
            StartCoroutine(ScaleInAnimation(slideTitle.transform, 1f, 0.5f));
        }

        if (slideSubtitle != null)
        {
            slideSubtitle.text = "has been decided...";
            slideSubtitle.gameObject.SetActive(true);
            StartCoroutine(FadeInAnimation(slideSubtitle, 1f, 1f));
        }

        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator ShowSummarySlide()
    {
        if (slideTitle != null)
        {
            slideTitle.text = "YOUR QUESTIONS";
            slideTitle.gameObject.SetActive(true);
        }

        if (slideContent != null)
        {
            slideContent.SetActive(true);
            
            // Create question cards
            for (int i = 0; i < session.spins.Count; i++)
            {
                var spin = session.spins[i];
                GameObject card = CreateQuestionCard(spin, i);
                if (card != null)
                {
                    card.transform.SetParent(slideContent.transform, false);
                    
                    // Stagger animation
                    float delay = i * 0.3f;
                    StartCoroutine(SlideInAnimation(card.transform, i % 2 == 0, delay));
                }
            }
        }

        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator ShowStarsSlide()
    {
        if (slideTitle != null)
        {
            slideTitle.text = "⭐ STAR RATING ⭐";
            slideTitle.color = goldColor;
            slideTitle.gameObject.SetActive(true);
        }

        if (starContainer != null)
        {
            starContainer.SetActive(true);
            
            // Create stars
            for (int i = 0; i < 5; i++)
            {
                Image star = Instantiate(starPrefab, starContainer.transform);
                star.gameObject.SetActive(false);
                
                // Reveal stars one by one
                StartCoroutine(RevealStar(star, i, i * 0.4f, i < session.finalScore.stars));
            }
        }

        yield return new WaitForSeconds(3f);
    }

    private IEnumerator ShowRegretSlide()
    {
        if (slideTitle != null)
        {
            slideTitle.text = "💀 REGRET LEVEL";
            slideTitle.color = redColor;
            slideTitle.gameObject.SetActive(true);
        }

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            StartCoroutine(AnimateProgressBar(session.finalScore.regretLevel));
        }

        if (percentageText != null)
        {
            percentageText.text = $"{session.finalScore.regretLevel}%";
            percentageText.gameObject.SetActive(true);
            StartCoroutine(ScaleInAnimation(percentageText.transform, 2.5f, 1f));
        }

        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator ShowCreativitySlide()
    {
        if (slideTitle != null)
        {
            slideTitle.text = "💡 CREATIVITY POINTS";
            slideTitle.color = purpleColor;
            slideTitle.gameObject.SetActive(true);
        }

        if (slideSubtitle != null)
        {
            slideSubtitle.text = $"+{session.finalScore.creativityPoints}\nFor your imaginative questions";
            slideSubtitle.gameObject.SetActive(true);
            StartCoroutine(BounceAnimation(slideSubtitle.transform, 2.5f));
        }

        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator ShowStupiditySlide()
    {
        if (slideTitle != null)
        {
            slideTitle.text = "🧠 STUPIDITY POINTS";
            slideTitle.color = new Color(1f, 0.549f, 0f, 1f); // Orange
            slideTitle.gameObject.SetActive(true);
        }

        if (slideSubtitle != null)
        {
            slideSubtitle.text = $"+{session.finalScore.stupidityPoints}\nFor trusting a chaotic wheel";
            slideSubtitle.gameObject.SetActive(true);
            StartCoroutine(BounceAnimation(slideSubtitle.transform, 2.5f));
        }

        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator ShowTotalSlide()
    {
        if (slideTitle != null)
        {
            slideTitle.text = "🏆 TOTAL SCORE";
            slideTitle.color = goldColor;
            slideTitle.gameObject.SetActive(true);
        }

        if (slideSubtitle != null)
        {
            slideSubtitle.text = session.finalScore.totalPoints.ToString();
            slideSubtitle.gameObject.SetActive(true);
            StartCoroutine(BigScoreAnimation(slideSubtitle.transform));
        }

        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator ShowPersonalitySlide()
    {
        if (string.IsNullOrEmpty(session.detectedPersonality))
        {
            currentSlide++; // Skip this slide
            yield break;
        }

        var personality = PersonalityDetector.DetectPersonality(session.username);
        if (personality == null)
        {
            currentSlide++; // Skip this slide
            yield break;
        }

        // Show special ending based on personality
        yield return StartCoroutine(ShowSpecialEnding(personality.endingType, personality.name, personality.trait));
    }

    private IEnumerator ShowSpecialEnding(EndingType endingType, string name, string trait)
    {
        switch (endingType)
        {
            case EndingType.Hammer:
                yield return StartCoroutine(HammerEnding(name, trait));
                break;
            case EndingType.Pokeball:
                yield return StartCoroutine(PokeballEnding(name, trait));
                break;
            case EndingType.SCP:
                yield return StartCoroutine(SCPEnding(name, trait));
                break;
            case EndingType.Glitch:
                yield return StartCoroutine(GlitchEnding(name, trait));
                break;
        }
    }

    private IEnumerator HammerEnding(string name, string trait)
    {
        if (slideTitle != null)
        {
            slideTitle.text = "🔨 FATE DECIDED";
            slideTitle.color = redColor;
            slideTitle.gameObject.SetActive(true);
        }

        if (slideSubtitle != null)
        {
            slideSubtitle.text = $"The hammer of judgment has fallen\n{name} - {trait}";
            slideSubtitle.gameObject.SetActive(true);
        }

        // Create hammer drop effect
        GameObject hammer = new GameObject("Hammer");
        Image hammerImage = hammer.AddComponent<Image>();
        hammerImage.color = redColor;
        
        RectTransform rect = hammer.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 150);
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0, 200);

        hammer.transform.SetParent(presentationCanvas.transform, false);

        // Drop animation
        yield return StartCoroutine(HammerDropAnimation(rect));

        SoundManager.Instance?.PlayHammerSlam();
        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator PokeballEnding(string name, string trait)
    {
        if (slideTitle != null)
        {
            slideTitle.text = "⭕ DESTINY CAPTURED";
            slideTitle.color = goldColor;
            slideTitle.gameObject.SetActive(true);
        }

        if (slideSubtitle != null)
        {
            slideSubtitle.text = $"Your fate was caught in 4K\n{name} - {trait}";
            slideSubtitle.gameObject.SetActive(true);
        }

        // Create pokeball effect
        GameObject pokeball = CreatePokeball();
        pokeball.transform.SetParent(presentationCanvas.transform, false);

        yield return StartCoroutine(PokeballCatchAnimation(pokeball.transform));
        SoundManager.Instance?.PlayPokeballCatch();
        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator SCPEnding(string name, string trait)
    {
        if (slideTitle != null)
        {
            slideTitle.text = "⚠️ [REDACTED]";
            slideTitle.color = new Color(1f, 0.549f, 0f, 1f); // Orange
            slideTitle.gameObject.SetActive(true);
        }

        if (slideSubtitle != null)
        {
            slideSubtitle.text = $"SCP Foundation has noted your choices\n{name} - {trait}";
            slideSubtitle.gameObject.SetActive(true);
        }

        // Warning flicker effect
        yield return StartCoroutine(WarningFlickerAnimation());
        SoundManager.Instance?.PlayScpBreach();
        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator GlitchEnding(string name, string trait)
    {
        if (slideTitle != null)
        {
            slideTitle.text = "R̷E̷A̷L̷I̷T̷Y̷ ̷E̷R̷R̷O̷R̷";
            slideTitle.color = Color.green;
            slideTitle.gameObject.SetActive(true);
        }

        if (slideSubtitle != null)
        {
            slideSubtitle.text = $"The matrix couldn't handle your chaos\n{name} - {trait}";
            slideSubtitle.gameObject.SetActive(true);
        }

        // Glitch effect
        yield return StartCoroutine(GlitchEffectAnimation());
        SoundManager.Instance?.PlayGlitch();
        yield return new WaitForSeconds(2.5f);
    }

    private void ShowFinalResults()
    {
        presentationComplete = true;
        ClearSlideContent();

        if (slideTitle != null)
        {
            slideTitle.text = "GAME COMPLETE";
            slideTitle.gameObject.SetActive(true);
        }

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

        if (playAgainButton != null)
        {
            playAgainButton.gameObject.SetActive(true);
        }

        if (viewCreditsButton != null)
        {
            viewCreditsButton.gameObject.SetActive(true);
        }

        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false);
        }
    }

    private GameObject CreateQuestionCard(SpinResult spin, int index)
    {
        GameObject card = new GameObject($"QuestionCard_{index}");
        card.AddComponent<RectTransform>();

        Image background = card.AddComponent<Image>();
        background.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        TextMeshProUGUI text = card.AddComponent<TextMeshProUGUI>();
        text.text = $"Q{index + 1}: {spin.question}\n{spin.answer}";
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;

        RectTransform rect = card.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 80);

        return card;
    }

    private GameObject CreatePokeball()
    {
        GameObject pokeball = new GameObject("Pokeball");
        pokeball.AddComponent<RectTransform>();

        Image image = pokeball.AddComponent<Image>();
        image.color = new Color(1f, 0.5f, 0.5f, 1f);

        RectTransform rect = pokeball.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 100);

        return pokeball;
    }

    private void SkipToNextSlide()
    {
        presentationComplete = true;
        SoundManager.Instance?.PlayClick();
    }

    private void PlayAgain()
    {
        GameManager.Instance.ResetGame();
    }

    private void ViewCredits()
    {
        GameManager.Instance.GoToCredits();
    }

    private float GetSlideDuration(SlideType slideType)
    {
        return slideType switch
        {
            SlideType.Intro => 2.5f,
            SlideType.Summary => 2.5f,
            SlideType.Stars => 3f,
            SlideType.Regret => 2.5f,
            SlideType.Creativity => 2.5f,
            SlideType.Stupidity => 2.5f,
            SlideType.Total => 2.5f,
            SlideType.Personality => 5f,
            _ => 2f
        };
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

    private void ClearSlideContent()
    {
        if (appIcon != null) appIcon.gameObject.SetActive(false);
        if (slideTitle != null) slideTitle.gameObject.SetActive(false);
        if (slideSubtitle != null) slideSubtitle.gameObject.SetActive(false);
        if (slideContent != null) slideContent.SetActive(false);
        if (starContainer != null) starContainer.SetActive(false);
        if (progressBar != null) progressBar.gameObject.SetActive(false);
        if (percentageText != null) percentageText.gameObject.SetActive(false);

        // Clear slide content children
        if (slideContent != null)
        {
            foreach (Transform child in slideContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    // Animation coroutines
    private IEnumerator ScaleInAnimation(Transform target, float duration, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        
        Vector3 originalScale = target.localScale;
        target.localScale = Vector3.zero;
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            target.localScale = Vector3.Lerp(Vector3.zero, originalScale, progress);
            yield return null;
        }
        
        target.localScale = originalScale;
    }

    private IEnumerator FadeInAnimation(TextMeshProUGUI target, float duration, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        
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

    private IEnumerator SlideInAnimation(Transform target, bool fromLeft, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        
        Vector3 originalPosition = target.position;
        Vector3 startPosition = originalPosition + (fromLeft ? Vector3.left * 500 : Vector3.right * 500);
        target.position = startPosition;
        
        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            target.position = Vector3.Lerp(startPosition, originalPosition, progress);
            yield return null;
        }
        
        target.position = originalPosition;
    }

    private IEnumerator RevealStar(Image star, int index, float delay, bool earned)
    {
        yield return new WaitForSeconds(delay);
        
        star.gameObject.SetActive(true);
        star.color = earned ? goldColor : new Color(0.3f, 0.3f, 0.3f, 1f);
        
        if (earned)
        {
            SoundManager.Instance?.PlayStar();
            StartCoroutine(StarPopAnimation(star.transform));
        }
    }

    private IEnumerator StarPopAnimation(Transform target)
    {
        Vector3 originalScale = target.localScale;
        target.localScale = Vector3.zero;
        
        float elapsed = 0f;
        float duration = 0.3f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float scale = Mathf.Sin(progress * Mathf.PI);
            target.localScale = Vector3.one * scale * 1.5f;
            yield return null;
        }
        
        target.localScale = originalScale;
    }

    private IEnumerator AnimateProgressBar(int percentage)
    {
        progressBar.fillAmount = 0f;
        
        float elapsed = 0f;
        float duration = 2f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            progressBar.fillAmount = Mathf.Lerp(0f, percentage / 100f, progress);
            yield return null;
        }
        
        progressBar.fillAmount = percentage / 100f;
    }

    private IEnumerator BounceAnimation(Transform target, float duration)
    {
        Vector3 originalScale = target.localScale;
        target.localScale = Vector3.zero;
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float bounce = Mathf.Abs(Mathf.Sin(progress * Mathf.PI * 2f)) * (1f - progress);
            target.localScale = originalScale * (progress + bounce);
            yield return null;
        }
        
        target.localScale = originalScale;
    }

    private IEnumerator BigScoreAnimation(Transform target)
    {
        Vector3 originalScale = target.localScale;
        target.localScale = Vector3.zero;
        
        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float scale = Mathf.Sin(progress * Mathf.PI) * 0.2f + progress;
            target.localScale = originalScale * scale;
            yield return null;
        }
        
        target.localScale = originalScale;
    }

    private IEnumerator HammerDropAnimation(RectTransform hammer)
    {
        Vector3 startPosition = hammer.anchoredPosition;
        Vector3 endPosition = new Vector3(0, 0, 0);
        
        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            hammer.anchoredPosition = Vector3.Lerp(startPosition, endPosition, progress * progress);
            yield return null;
        }
        
        hammer.anchoredPosition = endPosition;
    }

    private IEnumerator PokeballCatchAnimation(Transform target)
    {
        Vector3 originalScale = target.localScale;
        
        float elapsed = 0f;
        float duration = 1f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float scale = Mathf.Sin(progress * Mathf.PI * 4f) * 0.5f + 1f;
            float rotation = progress * 720f;
            target.localScale = originalScale * scale;
            target.rotation = Quaternion.Euler(0, 0, rotation);
            yield return null;
        }
        
        target.localScale = originalScale;
        target.rotation = Quaternion.identity;
    }

    private IEnumerator WarningFlickerAnimation()
    {
        float elapsed = 0f;
        float duration = 2f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bool visible = Mathf.Floor(elapsed * 10f) % 2 == 0;
            if (slideTitle != null)
                slideTitle.gameObject.SetActive(visible);
            yield return null;
        }
        
        if (slideTitle != null)
            slideTitle.gameObject.SetActive(true);
    }

    private IEnumerator GlitchEffectAnimation()
    {
        float elapsed = 0f;
        float duration = 2f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            if (slideTitle != null)
            {
                Vector3 originalPosition = slideTitle.transform.position;
                slideTitle.transform.position = originalPosition + new Vector3(
                    Random.Range(-5f, 5f),
                    Random.Range(-5f, 5f),
                    0
                );
                
                // Color shift
                float hue = (elapsed * 360f) % 360f;
                slideTitle.color = Color.HSVToRGB(hue / 360f, 1f, 1f);
            }
            
            yield return null;
        }
    }
}
