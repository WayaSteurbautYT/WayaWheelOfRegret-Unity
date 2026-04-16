using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("UI Components")]
    public Image appIcon;
    public TextMeshProUGUI titleText;
    public TMP_InputField usernameInput;
    public Button startButton;
    public Button settingsButton;
    public Button historyButton;
    public GameObject settingsModal;
    public GameObject historyModal;
    public GameObject historyContent;
    public Button clearHistoryButton;

    [Header("Game Mode Buttons")]
    public Button classicModeButton;
    public Button chaosModeButton;
    public Button fateModeButton;
    public Button rapidModeButton;
    public TextMeshProUGUI classicModeText;
    public TextMeshProUGUI chaosModeText;
    public TextMeshProUGUI fateModeText;
    public TextMeshProUGUI rapidModeText;

    [Header("Settings")]
    public Toggle musicToggle;
    public Toggle sfxToggle;

    [Header("Visual Effects")]
    public GameObject particleContainer;
    public Color redColor = Color.red;
    public Color gunmetalColor = new Color(0.165f, 0.165f, 0.212f, 1f); // #2A2A35
    public Color selectedColor = new Color(1f, 0.2f, 0.2f, 1f); // Red with glow

    private GameMode selectedMode = GameMode.Classic;
    private bool settingsOpen = false;
    private bool historyOpen = false;

    private void Start()
    {
        InitializeMenu();
        CreateParticleEffects();
        LoadSettings();
    }

    private void InitializeMenu()
    {
        // Set up username input
        if (usernameInput != null)
        {
            usernameInput.text = GameManager.Instance.username;
            usernameInput.onValueChanged.AddListener(OnUsernameChanged);
            
            // Set placeholder text
            usernameInput.placeholder.GetComponent<TextMeshProUGUI>().text = "Enter your name (the wheel may remember you...)";
        }

        // Set up game mode buttons
        SetupGameModeButtons();

        // Set up other buttons
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartGame);
            startButton.interactable = !string.IsNullOrEmpty(GameManager.Instance.username);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(ToggleSettings);
        }

        if (historyButton != null)
        {
            historyButton.onClick.AddListener(ToggleHistory);
            UpdateHistoryButtonBadge();
        }

        // Set up modals
        if (settingsModal != null) settingsModal.SetActive(false);
        if (historyModal != null) historyModal.SetActive(false);

        // Animate icon
        if (appIcon != null)
        {
            StartCoroutine(AnimateIcon());
        }
    }

    private void SetupGameModeButtons()
    {
        // Classic mode
        if (classicModeButton != null)
        {
            classicModeButton.onClick.AddListener(() => SelectGameMode(GameMode.Classic));
            if (classicModeText != null)
            {
                classicModeText.text = "Classic\nOne question, one fate";
            }
        }

        // Chaos mode
        if (chaosModeButton != null)
        {
            chaosModeButton.onClick.AddListener(() => SelectGameMode(GameMode.Chaos));
            if (chaosModeText != null)
            {
                chaosModeText.text = "Chaos Chain\n6 spins of escalating doom";
            }
        }

        // Fate mode
        if (fateModeButton != null)
        {
            fateModeButton.onClick.AddListener(() => SelectGameMode(GameMode.Fate));
            if (fateModeText != null)
            {
                fateModeText.text = "Wheel of Fate\nThe wheel knows who you are...";
            }
        }

        // Rapid mode
        if (rapidModeButton != null)
        {
            rapidModeButton.onClick.AddListener(() => SelectGameMode(GameMode.Rapid));
            if (rapidModeText != null)
            {
                rapidModeText.text = "Rapid Fire\nAuto-spin madness";
            }
        }

        // Select default mode
        SelectGameMode(GameMode.Classic);
    }

    private void SelectGameMode(GameMode mode)
    {
        selectedMode = mode;

        // Update button appearances
        UpdateGameModeButtonAppearance(classicModeButton, mode == GameMode.Classic);
        UpdateGameModeButtonAppearance(chaosModeButton, mode == GameMode.Chaos);
        UpdateGameModeButtonAppearance(fateModeButton, mode == GameMode.Fate);
        UpdateGameModeButtonAppearance(rapidModeButton, mode == GameMode.Rapid);

        // Play sound
        SoundManager.Instance?.PlayClick();
    }

    private void UpdateGameModeButtonAppearance(Button button, bool isSelected)
    {
        if (button == null) return;

        ColorBlock colors = button.colors;
        
        if (isSelected)
        {
            colors.normalColor = selectedColor;
            colors.highlightedColor = new Color(selectedColor.r * 0.8f, selectedColor.g * 0.8f, selectedColor.b * 0.8f, 1f);
            // Add glow effect here if needed
        }
        else
        {
            colors.normalColor = gunmetalColor;
            colors.highlightedColor = new Color(gunmetalColor.r * 1.2f, gunmetalColor.g * 1.2f, gunmetalColor.b * 1.2f, 1f);
        }

        button.colors = colors;
    }

    private void OnUsernameChanged(string newText)
    {
        GameManager.Instance.username = newText;
        
        if (startButton != null)
        {
            startButton.interactable = !string.IsNullOrEmpty(newText);
        }
    }

    private void OnStartGame()
    {
        if (string.IsNullOrEmpty(GameManager.Instance.username))
        {
            Debug.LogWarning("Cannot start game without username");
            return;
        }

        GameManager.Instance.StartGame(selectedMode, GameManager.Instance.username);
    }

    private void ToggleSettings()
    {
        settingsOpen = !settingsOpen;
        
        if (settingsModal != null)
        {
            settingsModal.SetActive(settingsOpen);
            
            if (settingsOpen)
            {
                LoadSettingsIntoModal();
                SoundManager.Instance?.PlayClick();
            }
        }

        // Close history if open
        if (settingsOpen && historyOpen)
        {
            ToggleHistory();
        }
    }

    private void ToggleHistory()
    {
        historyOpen = !historyOpen;
        
        if (historyModal != null)
        {
            historyModal.SetActive(historyOpen);
            
            if (historyOpen)
            {
                PopulateHistory();
                SoundManager.Instance?.PlayClick();
            }
        }

        // Close settings if open
        if (historyOpen && settingsOpen)
        {
            ToggleSettings();
        }
    }

    private void LoadSettings()
    {
        if (musicToggle != null)
        {
            musicToggle.isOn = GameManager.Instance.musicEnabled;
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        }

        if (sfxToggle != null)
        {
            sfxToggle.isOn = GameManager.Instance.sfxEnabled;
            sfxToggle.onValueChanged.AddListener(OnSfxToggleChanged);
        }
    }

    private void LoadSettingsIntoModal()
    {
        if (musicToggle != null)
        {
            musicToggle.isOn = GameManager.Instance.musicEnabled;
        }

        if (sfxToggle != null)
        {
            sfxToggle.isOn = GameManager.Instance.sfxEnabled;
        }
    }

    private void OnMusicToggleChanged(bool isOn)
    {
        GameManager.Instance.ToggleMusic();
    }

    private void OnSfxToggleChanged(bool isOn)
    {
        GameManager.Instance.ToggleSfx();
    }

    private void PopulateHistory()
    {
        if (historyContent == null) return;

        // Clear existing content
        foreach (Transform child in historyContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Add history entries
        foreach (var entry in GameManager.Instance.gameHistory)
        {
            GameObject historyItem = CreateHistoryItem(entry);
            if (historyItem != null)
            {
                historyItem.transform.SetParent(historyContent.transform, false);
            }
        }

        // Set up clear button
        if (clearHistoryButton != null)
        {
            clearHistoryButton.onClick.AddListener(ClearHistory);
        }
    }

    private GameObject CreateHistoryItem(WheelHistoryEntry entry)
    {
        // Create a simple history item
        GameObject item = new GameObject("HistoryItem");
        item.AddComponent<RectTransform>();

        // Add background
        Image background = item.AddComponent<Image>();
        background.color = gunmetalColor;

        // Add text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(item.transform, false);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = $"{entry.username} - {GameState.GAME_MODES[entry.mode].name}\n" +
                   $"{System.DateTime.FromFileTimeUtc(entry.date * 10000):MMM dd, yyyy}\n" +
                   $"Stars: {entry.finalScore.stars}/5 - Regret: {entry.finalScore.regretLevel}%";
        text.fontSize = 14;
        text.color = Color.white;

        // Set up rect transform
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 5);
        textRect.offsetMax = new Vector2(-10, -5);

        // Set item size
        RectTransform itemRect = item.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(400, 80);

        // Add button component for clicking
        Button button = item.AddComponent<Button>();
        button.onClick.AddListener(() => OnHistoryItemClicked(entry));

        return item;
    }

    private void OnHistoryItemClicked(WheelHistoryEntry entry)
    {
        // Could show detailed view of this session
        Debug.Log($"History item clicked: {entry.sessionId}");
        SoundManager.Instance?.PlayClick();
    }

    private void ClearHistory()
    {
        GameManager.Instance.ClearHistory();
        PopulateHistory(); // Refresh the list
        UpdateHistoryButtonBadge();
        SoundManager.Instance?.PlayClick();
    }

    private void UpdateHistoryButtonBadge()
    {
        // Update history button to show count
        int count = GameManager.Instance.gameHistory.Count;
        // You could add a badge UI element here
        Debug.Log($"History entries: {count}");
    }

    private void CreateParticleEffects()
    {
        if (particleContainer == null) return;

        // Create floating particles
        for (int i = 0; i < 20; i++)
        {
            GameObject particle = CreateParticle();
            if (particle != null)
            {
                particle.transform.SetParent(particleContainer.transform, false);
            }
        }
    }

    private GameObject CreateParticle()
    {
        GameObject particle = new GameObject("Particle");
        particle.AddComponent<RectTransform>();

        Image image = particle.AddComponent<Image>();
        
        // Random color (red, purple, or gold)
        Color[] colors = { redColor, new Color(0.545f, 0f, 1f, 1f), Color.yellow };
        image.color = colors[Random.Range(0, colors.Length)];

        // Random size
        RectTransform rect = particle.GetComponent<RectTransform>();
        float size = Random.Range(3f, 8f);
        rect.sizeDelta = new Vector2(size, size);

        // Random position
        rect.anchorMin = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        rect.anchorMax = rect.anchorMin;
        rect.anchoredPosition = Vector2.zero;

        // Add animation
        StartCoroutine(AnimateParticle(particle));

        return particle;
    }

    private IEnumerator AnimateParticle(GameObject particle)
    {
        RectTransform rect = particle.GetComponent<RectTransform>();
        Vector2 originalPosition = rect.anchoredPosition;
        float pulseSpeed = Random.Range(1f, 3f);
        float moveSpeed = Random.Range(0.5f, 2f);

        while (particle != null)
        {
            // Pulse effect
            float scale = Mathf.Sin(Time.time * pulseSpeed) * 0.3f + 1f;
            rect.localScale = Vector3.one * scale;

            // Slow movement
            rect.anchoredPosition = originalPosition + new Vector2(
                Mathf.Sin(Time.time * moveSpeed) * 20f,
                Mathf.Cos(Time.time * moveSpeed) * 20f
            );

            yield return null;
        }
    }

    private IEnumerator AnimateIcon()
    {
        if (appIcon == null) yield break;

        while (true)
        {
            // Spinning glow animation
            float rotation = Time.time * 30f; // 30 degrees per second
            appIcon.transform.rotation = Quaternion.Euler(0, 0, rotation);

            // Pulse effect
            float scale = Mathf.Sin(Time.time * 2f) * 0.1f + 1f;
            appIcon.transform.localScale = Vector3.one * scale;

            yield return null;
        }
    }
}
