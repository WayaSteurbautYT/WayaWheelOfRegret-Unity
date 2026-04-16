using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditsScreen : MonoBehaviour
{
    [Header("UI Components")]
    public Button backButton;
    public TextMeshProUGUI creditsTitle;
    public GameObject creditsContent;
    public GameObject finalSection;

    [Header("Visual Effects")]
    public GameObject starfieldContainer;
    public GameObject appIcon;
    public TextMeshProUGUI finalTitle;
    public TextMeshProUGUI finalSubtitle;
    public Button youtubeButton;

    [Header("Credits Data")]
    [SerializeField] private CreditEntry[] creditEntries;

    [System.Serializable]
    public class CreditEntry
    {
        public string role;
        public string name;
        public bool highlighted;
        public string iconName; // Optional icon name
    }

    private void Start()
    {
        InitializeCredits();
        CreateStarfield();
        StartCoroutine(PlayCreditsSequence());
    }

    private void InitializeCredits()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        if (youtubeButton != null)
        {
            youtubeButton.onClick.AddListener(OnYouTubeButtonClicked);
        }

        // Initialize credits entries if not set in inspector
        if (creditEntries == null || creditEntries.Length == 0)
        {
            InitializeDefaultCredits();
        }

        // Hide final section initially
        if (finalSection != null)
        {
            finalSection.SetActive(false);
        }
    }

    private void InitializeDefaultCredits()
    {
        creditEntries = new CreditEntry[]
        {
            new CreditEntry { role = "Created By", name = "Waya Steurbaut", highlighted = true },
            new CreditEntry { role = "Channel", name = "WayaCreate on YouTube", iconName = "YouTube" },
            new CreditEntry { role = "Development", name = "Vibe Coding with AI" },
            new CreditEntry { role = "Engine", name = "Unity (Web Version in Next.js)" },
            new CreditEntry { role = "Design", name = "Cosmic Chaos Aesthetic" },
            new CreditEntry { role = "Sound Design", name = "Web Audio API Magic" },
            new CreditEntry { role = "Wheel Logic", name = "Pure Randomness (or is it?)" },
            new CreditEntry { role = "Fate Algorithm", name = "Top Secret" },
            new CreditEntry { role = "Special Thanks", name = "The Cursed Viewers" },
            new CreditEntry { role = "Inspiration", name = "Bad Decisions Everywhere" }
        };
    }

    private void CreateStarfield()
    {
        if (starfieldContainer == null) return;

        // Create 50 stars with random properties
        for (int i = 0; i < 50; i++)
        {
            GameObject star = CreateStar();
            if (star != null)
            {
                star.transform.SetParent(starContainer.transform, false);
            }
        }
    }

    private GameObject CreateStar()
    {
        GameObject star = new GameObject("Star");
        star.AddComponent<RectTransform>();

        Image starImage = star.AddComponent<Image>();
        starImage.color = Color.white;

        RectTransform rect = star.GetComponent<RectTransform>();
        float size = Random.Range(1f, 4f);
        rect.sizeDelta = new Vector2(size, size);

        // Random position across the screen
        rect.anchorMin = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        rect.anchorMax = rect.anchorMin;
        rect.anchoredPosition = Vector2.zero;

        // Add animation
        StartCoroutine(AnimateStar(star));

        return star;
    }

    private IEnumerator AnimateStar(GameObject star)
    {
        RectTransform rect = star.GetComponent<RectTransform>();
        Image image = star.GetComponent<Image>();
        Vector2 originalPosition = rect.anchoredPosition;
        
        float pulseSpeed = Random.Range(1f, 3f);
        float moveSpeed = Random.Range(0.5f, 2f);
        float scaleSpeed = Random.Range(2f, 4f);

        while (star != null)
        {
            // Pulsing effect
            float alpha = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;
            image.color = new Color(1f, 1f, 1f, alpha);

            // Slow drift
            rect.anchoredPosition = originalPosition + new Vector2(
                Mathf.Sin(Time.time * moveSpeed) * 20f,
                Mathf.Cos(Time.time * moveSpeed * 0.7f) * 20f
            );

            // Scale pulsing
            float scale = Mathf.Sin(Time.time * scaleSpeed) * 0.3f + 1f;
            rect.localScale = Vector3.one * scale;

            yield return null;
        }
    }

    private IEnumerator PlayCreditsSequence()
    {
        // Show title
        if (creditsTitle != null)
        {
            creditsTitle.gameObject.SetActive(true);
            StartCoroutine(FadeInText(creditsTitle, 1f));
        }

        yield return new WaitForSeconds(2f);

        // Show credits one by one
        for (int i = 0; i < creditEntries.Length; i++)
        {
            yield return StartCoroutine(ShowCreditEntry(creditEntries[i], i));
            yield return new WaitForSeconds(0.6f);
        }

        // Show final section
        yield return new WaitForSeconds(2f);
        ShowFinalSection();
    }

    private IEnumerator ShowCreditEntry(CreditEntry entry, int index)
    {
        GameObject creditItem = CreateCreditItem(entry, index);
        if (creditItem != null)
        {
            creditItem.transform.SetParent(creditsContent.transform, false);
            
            // Slide in from alternating sides
            bool fromLeft = index % 2 == 0;
            yield return StartCoroutine(SlideInCredit(creditItem, fromLeft));
            
            // Play hover sound
            SoundManager.Instance?.PlayHover();
        }
    }

    private GameObject CreateCreditItem(CreditEntry entry, int index)
    {
        GameObject item = new GameObject($"CreditItem_{index}");
        item.AddComponent<RectTransform>();

        // Background
        Image background = item.AddComponent<Image>();
        background.color = entry.highlighted ? 
            new Color(1f, 0.2f, 0.2f, 0.3f) : // Red tint for highlighted
            new Color(0.2f, 0.2f, 0.2f, 0.8f);  // Semi-transparent dark

        // Role text
        GameObject roleObj = new GameObject("Role");
        roleObj.transform.SetParent(item.transform, false);
        TextMeshProUGUI roleText = roleObj.AddComponent<TextMeshProUGUI>();
        roleText.text = entry.role;
        roleText.fontSize = 16;
        roleText.fontStyle = FontStyles.Bold;
        roleText.color = entry.highlighted ? Color.red : Color.white;
        roleText.alignment = TextAlignmentOptions.Left;

        // Name text
        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(item.transform, false);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.text = entry.name;
        nameText.fontSize = 14;
        nameText.color = entry.highlighted ? new Color(1f, 1f, 1f, 0.9f) : new Color(0.8f, 0.8f, 0.8f, 1f);
        nameText.alignment = TextAlignmentOptions.Left;

        // Set up layouts
        RectTransform itemRect = item.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(600, 60);

        RectTransform roleRect = roleObj.GetComponent<RectTransform>();
        roleRect.anchorMin = Vector2.zero;
        roleRect.anchorMax = new Vector2(0.3f, 1f);
        roleRect.offsetMin = new Vector2(20, 5);
        roleRect.offsetMax = new Vector2(-10, -5);

        RectTransform nameRect = nameObj.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.3f, 0);
        nameRect.anchorMax = Vector2.one;
        nameRect.offsetMin = new Vector2(10, 5);
        nameRect.offsetMax = new Vector2(-20, -5);

        return item;
    }

    private IEnumerator SlideInCredit(GameObject credit, bool fromLeft)
    {
        RectTransform rect = credit.GetComponent<RectTransform>();
        Vector3 originalPosition = rect.anchoredPosition;
        Vector3 startPosition = originalPosition + (fromLeft ? Vector3.left * 800 : Vector3.right * 800);
        rect.anchoredPosition = startPosition;

        float elapsed = 0f;
        float duration = 0.8f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            rect.anchoredPosition = Vector3.Lerp(startPosition, originalPosition, easedProgress);
            yield return null;
        }

        rect.anchoredPosition = originalPosition;
    }

    private void ShowFinalSection()
    {
        if (finalSection != null)
        {
            finalSection.SetActive(true);
            
            // Animate app icon
            if (appIcon != null)
            {
                appIcon.SetActive(true);
                StartCoroutine(BounceIconAnimation());
            }

            // Show final text
            if (finalTitle != null)
            {
                finalTitle.text = "Waya's Wheel of Regret";
                StartCoroutine(FadeInText(finalTitle, 1f));
            }

            if (finalSubtitle != null)
            {
                finalSubtitle.text = "Made with ❤️ and chaos";
                StartCoroutine(FadeInText(finalSubtitle, 1f, 0.5f));
            }

            // Show YouTube button
            if (youtubeButton != null)
            {
                youtubeButton.gameObject.SetActive(true);
                StartCoroutine(ScaleInAnimation(youtubeButton.transform, 0.5f, 1f));
            }
        }
    }

    private IEnumerator BounceIconAnimation()
    {
        if (appIcon == null) yield break;

        Vector3 originalPosition = appIcon.transform.position;
        float elapsed = 0f;
        float bounceSpeed = 3f;
        float bounceHeight = 20f;

        while (true)
        {
            elapsed += Time.deltaTime;
            float yOffset = Mathf.Abs(Mathf.Sin(elapsed * bounceSpeed)) * bounceHeight;
            appIcon.transform.position = originalPosition + new Vector3(0, yOffset, 0);
            yield return null;
        }
    }

    private IEnumerator FadeInText(TextMeshProUGUI text, float duration, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        Color originalColor = text.color;
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, progress);
            yield return null;
        }

        text.color = originalColor;
    }

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
            float scale = Mathf.SmoothStep(0f, 1f, progress);
            target.localScale = originalScale * scale;
            yield return null;
        }

        target.localScale = originalScale;
    }

    private void OnBackButtonClicked()
    {
        SoundManager.Instance?.PlayClick();
        GameManager.Instance.GoToMenu();
    }

    private void OnYouTubeButtonClicked()
    {
        SoundManager.Instance?.PlayClick();
        Application.OpenURL("https://www.youtube.com/@WayaCreate");
    }
}
