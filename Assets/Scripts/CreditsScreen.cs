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
    }

    private void Start()
    {
        InitializeCredits();
        StartCoroutine(PlayCreditsSequence());
    }

    private void InitializeCredits()
    {
        if (backButton != null) backButton.onClick.AddListener(OnBackButtonClicked);
        if (youtubeButton != null) youtubeButton.onClick.AddListener(OnYouTubeButtonClicked);

        // Initialize credits if not set
        if (creditEntries == null || creditEntries.Length == 0)
        {
            InitializeDefaultCredits();
        }

        // Hide final section initially
        if (finalTitle != null) finalTitle.gameObject.SetActive(false);
        if (finalSubtitle != null) finalSubtitle.gameObject.SetActive(false);
        if (youtubeButton != null) youtubeButton.gameObject.SetActive(false);
    }

    private void InitializeDefaultCredits()
    {
        creditEntries = new CreditEntry[]
        {
            new CreditEntry { role = "Created By", name = "Waya Steurbaut", highlighted = true },
            new CreditEntry { role = "Channel", name = "WayaCreate on YouTube", highlighted = true },
            new CreditEntry { role = "Development", name = "Vibe Coding with AI" },
            new CreditEntry { role = "Engine", name = "Unity" },
            new CreditEntry { role = "Design", name = "Cosmic Chaos Aesthetic" },
            new CreditEntry { role = "Sound Design", name = "Procedural Audio" },
            new CreditEntry { role = "Wheel Logic", name = "Pure Randomness" },
            new CreditEntry { role = "Special Thanks", name = "The Viewers" },
            new CreditEntry { role = "Inspiration", name = "Bad Decisions Everywhere" }
        };
    }

    private IEnumerator PlayCreditsSequence()
    {
        // Show title
        if (creditsTitle != null)
        {
            creditsTitle.gameObject.SetActive(true);
            StartCoroutine(FadeInAnimation(creditsTitle, 1f));
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
        if (creditItem != null && creditsContent != null)
        {
            creditItem.transform.SetParent(creditsContent.transform, false);
            
            bool fromLeft = index % 2 == 0;
            yield return StartCoroutine(SlideInAnimation(creditItem.transform, fromLeft));
            
            SoundManager.Instance?.PlayClick();
        }
    }

    private GameObject CreateCreditItem(CreditEntry entry, int index)
    {
        GameObject item = new GameObject($"CreditItem_{index}");
        item.AddComponent<RectTransform>();

        // Background
        Image background = item.AddComponent<Image>();
        background.color = entry.highlighted ? 
            new Color(1f, 0.2f, 0.2f, 0.3f) : 
            new Color(0.2f, 0.2f, 0.2f, 0.8f);

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

    private IEnumerator SlideInAnimation(Transform target, bool fromLeft)
    {
        RectTransform rect = target.GetComponent<RectTransform>();
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
        if (finalTitle != null)
        {
            finalTitle.text = "Waya's Wheel of Regret";
            finalTitle.gameObject.SetActive(true);
            StartCoroutine(FadeInAnimation(finalTitle, 1f));
        }

        if (finalSubtitle != null)
        {
            finalSubtitle.text = "Made with chaos and AI";
            finalSubtitle.gameObject.SetActive(true);
            StartCoroutine(FadeInAnimation(finalSubtitle, 1f, 0.5f));
        }

        if (youtubeButton != null)
        {
            youtubeButton.gameObject.SetActive(true);
            StartCoroutine(ScaleInAnimation(youtubeButton.transform, 0.5f, 1f));
        }
    }

    private IEnumerator FadeInAnimation(TextMeshProUGUI text, float duration, float delay = 0f)
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
        GameManager.Instance?.GoToMenu();
    }

    private void OnYouTubeButtonClicked()
    {
        SoundManager.Instance?.PlayClick();
        Application.OpenURL("https://www.youtube.com/@WayaCreate");
    }
}
