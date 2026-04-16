using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameScreen : MonoBehaviour
{
    [Header("Header UI")]
    public Button homeButton;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI modeText;
    public Button autoSpinButton;
    public TextMeshProUGUI autoSpinText;

    [Header("Sidebar (Desktop)")]
    public GameObject sidebarPanel;
    public GameObject sidebarContent;
    public Button[] sidebarSpinButtons;

    [Header("Main Area")]
    public GameObject wheelContainer;
    public WheelSpinner wheelSpinner;
    public Image outerGlowRing;

    [Header("Mobile Navigation")]
    public GameObject mobileNavPanel;
    public Button previousSpinButton;
    public Button nextSpinButton;
    public TextMeshProUGUI mobileSpinIndicator;

    [Header("Visual Effects")]
    public Color redColor = Color.red;
    public Color purpleColor = new Color(0.545f, 0f, 1f, 1f); // #8B00FF

    private bool isAutoSpinning = false;
    private int currentViewingSpin = 0;
    private bool isDesktop = true;

    private void Start()
    {
        InitializeScreen();
        DetectPlatform();
        UpdateUI();
    }

    private void InitializeScreen()
    {
        // Set up header
        SetupHeader();

        // Set up sidebar
        SetupSidebar();

        // Set up mobile navigation
        SetupMobileNavigation();

        // Set up wheel
        if (wheelSpinner != null)
        {
            wheelSpinner.OnSpinComplete += OnSpinCompleted;
        }

        // Start glow effect
        if (outerGlowRing != null)
        {
            StartCoroutine(AnimateGlowRing());
        }
    }

    private void SetupHeader()
    {
        if (homeButton != null)
        {
            homeButton.onClick.AddListener(OnHomeButtonClicked);
        }

        if (autoSpinButton != null)
        {
            autoSpinButton.onClick.AddListener(ToggleAutoSpin);
            autoSpinButton.gameObject.SetActive(false); // Only show in Rapid mode
        }

        UpdateHeaderText();
    }

    private void SetupSidebar()
    {
        if (sidebarPanel != null)
        {
            // Initially hide sidebar on mobile
            sidebarPanel.SetActive(isDesktop);
        }
    }

    private void SetupMobileNavigation()
    {
        if (previousSpinButton != null)
        {
            previousSpinButton.onClick.AddListener(OnPreviousSpinClicked);
        }

        if (nextSpinButton != null)
        {
            nextSpinButton.onClick.AddListener(OnNextSpinClicked);
        }
    }

    private void DetectPlatform()
    {
        // Simple platform detection - in a real app, you'd use more sophisticated detection
        isDesktop = Screen.width > 1024f;
        
        if (sidebarPanel != null)
        {
            sidebarPanel.SetActive(isDesktop);
        }

        if (mobileNavPanel != null)
        {
            mobileNavPanel.SetActive(!isDesktop);
        }
    }

    private void UpdateUI()
    {
        UpdateHeaderText();
        UpdateSidebar();
        UpdateMobileNavigation();
        UpdateAutoSpinButton();
    }

    private void UpdateHeaderText()
    {
        if (GameManager.Instance.currentSession == null) return;

        if (titleText != null)
        {
            titleText.text = $"{GameManager.Instance.currentSession.username}'s Fate";
        }

        if (modeText != null)
        {
            string modeName = GameState.GAME_MODES[GameManager.Instance.currentSession.mode].name;
            int currentSpin = GameManager.Instance.currentSession.spins.Count + 1;
            int maxSpins = GameManager.Instance.currentSession.maxSpins;
            modeText.text = $"Spin {currentSpin}/{maxSpins} - {modeName}";
        }
    }

    private void UpdateSidebar()
    {
        if (!isDesktop || sidebarContent == null) return;

        // Clear existing sidebar content
        foreach (Transform child in sidebarContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Add spin history items
        var session = GameManager.Instance.currentSession;
        if (session != null)
        {
            for (int i = 0; i < session.spins.Count; i++)
            {
                var spin = session.spins[i];
                GameObject spinItem = CreateSidebarSpinItem(spin, i);
                if (spinItem != null)
                {
                    spinItem.transform.SetParent(sidebarContent.transform, false);
                }
            }
        }
    }

    private GameObject CreateSidebarSpinItem(SpinResult spin, int index)
    {
        GameObject item = new GameObject($"SpinItem_{index}");
        item.AddComponent<RectTransform>();

        // Background
        Image background = item.AddComponent<Image>();
        background.color = index == currentViewingSpin ? redColor : new Color(0.2f, 0.2f, 0.2f, 1f);

        // Button component
        Button button = item.AddComponent<Button>();
        button.onClick.AddListener(() => OnSidebarSpinClicked(index));

        // Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(item.transform, false);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        string questionPreview = spin.question.Length > 30 ? spin.question.Substring(0, 30) + "..." : spin.question;
        text.text = $"Q{index + 1}: {questionPreview}\n{spin.answer}";
        text.fontSize = 12;
        text.color = Color.white;

        // Set up rect transform
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(5, 3);
        textRect.offsetMax = new Vector2(-5, -3);

        // Set item size
        RectTransform itemRect = item.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(250, 60);

        return item;
    }

    private void UpdateMobileNavigation()
    {
        if (isDesktop || mobileSpinIndicator == null) return;

        var session = GameManager.Instance.currentSession;
        if (session != null)
        {
            int totalSpins = session.spins.Count;
            if (totalSpins > 0)
            {
                mobileSpinIndicator.text = $"{currentViewingSpin + 1} of {totalSpins}";
            }
            else
            {
                mobileSpinIndicator.text = "No spins yet";
            }
        }

        // Update button states
        if (previousSpinButton != null)
        {
            previousSpinButton.interactable = currentViewingSpin > 0;
        }

        if (nextSpinButton != null)
        {
            var sessionSpins = GameManager.Instance.currentSession?.spins;
            nextSpinButton.interactable = sessionSpins != null && currentViewingSpin < sessionSpins.Count - 1;
        }
    }

    private void UpdateAutoSpinButton()
    {
        if (autoSpinButton == null) return;

        var session = GameManager.Instance.currentSession;
        bool shouldShow = session != null && session.mode == GameMode.Rapid;
        
        autoSpinButton.gameObject.SetActive(shouldShow);

        if (shouldShow && autoSpinText != null)
        {
            autoSpinText.text = isAutoSpinning ? "Stop Auto" : "Auto Spin";
            autoSpinButton.GetComponent<Image>().color = isAutoSpinning ? purpleColor : redColor;
        }
    }

    private void OnHomeButtonClicked()
    {
        // Show confirmation dialog
        // For now, just go back to menu
        GameManager.Instance.ResetGame();
    }

    private void ToggleAutoSpin()
    {
        isAutoSpinning = !isAutoSpinning;

        if (isAutoSpinning)
        {
            StartCoroutine(AutoSpinSequence());
        }

        UpdateAutoSpinButton();
        SoundManager.Instance?.PlayClick();
    }

    private IEnumerator AutoSpinSequence()
    {
        while (isAutoSpinning && wheelSpinner != null)
        {
            // Wait a moment between spins
            yield return new WaitForSeconds(1f);

            // Check if game is complete
            var session = GameManager.Instance.currentSession;
            if (session == null || session.spins.Count >= session.maxSpins)
            {
                isAutoSpinning = false;
                UpdateAutoSpinButton();
                yield break;
            }

            // Generate random question
            string[] randomQuestions = {
                "Should I make this decision?",
                "Is this a good idea?",
                "What does the wheel think?",
                "Should I proceed?",
                "What does fate say?"
            };

            string question = randomQuestions[Random.Range(0, randomQuestions.Length)];
            
            // Trigger spin
            wheelSpinner.StartSpin(question);

            // Wait for spin to complete
            while (wheelSpinner != null && wheelSpinner.gameObject.activeInHierarchy)
            {
                // Check if wheel is still spinning by checking if input is disabled
                var inputField = wheelSpinner.GetComponentInChildren<TMP_InputField>();
                if (inputField != null && inputField.interactable)
                {
                    break; // Spin completed
                }
                yield return null;
            }
        }
    }

    private void OnSpinCompleted(GameState.WheelSegment segment, string answer, int doom)
    {
        UpdateUI();
        SoundManager.Instance?.PlaySuccess();
    }

    private void OnSidebarSpinClicked(int spinIndex)
    {
        currentViewingSpin = spinIndex;
        UpdateSidebar();
        SoundManager.Instance?.PlayClick();
        
        // Could show detailed view of this spin here
        Debug.Log($"Viewing spin {spinIndex}");
    }

    private void OnPreviousSpinClicked()
    {
        if (currentViewingSpin > 0)
        {
            currentViewingSpin--;
            UpdateMobileNavigation();
            SoundManager.Instance?.PlayClick();
        }
    }

    private void OnNextSpinClicked()
    {
        var session = GameManager.Instance.currentSession;
        if (session != null && currentViewingSpin < session.spins.Count - 1)
        {
            currentViewingSpin++;
            UpdateMobileNavigation();
            SoundManager.Instance?.PlayClick();
        }
    }

    private IEnumerator AnimateGlowRing()
    {
        if (outerGlowRing == null) yield break;

        while (true)
        {
            float pulse = Mathf.Sin(Time.time * 2f) * 0.3f + 0.7f;
            Color glowColor = Color.red;
            glowColor.a = pulse * 0.3f;
            outerGlowRing.color = glowColor;

            yield return null;
        }
    }

    private void OnEnable()
    {
        // Update UI when screen becomes active
        UpdateUI();
    }

    private void OnDestroy()
    {
        // Clean up event listeners
        if (wheelSpinner != null)
        {
            wheelSpinner.OnSpinComplete -= OnSpinCompleted;
        }
    }
}
