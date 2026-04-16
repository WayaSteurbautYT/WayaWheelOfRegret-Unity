using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("UI Components")]
    public Image appIcon;
    public TextMeshProUGUI titleText;
    public TMP_InputField usernameInput;
    public Button startButton;
    public Button classicModeButton;
    public Button chaosModeButton;
    public Button fateModeButton;
    public Button rapidModeButton;

    private GameMode selectedMode = GameMode.Classic;

    private void Start()
    {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        if (usernameInput != null)
        {
            usernameInput.text = GameManager.Instance?.username ?? "";
            usernameInput.onValueChanged.AddListener(OnUsernameChanged);
        }

        SetupGameModeButtons();

        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartGame);
            startButton.interactable = !string.IsNullOrEmpty(GameManager.Instance?.username);
        }

        if (appIcon != null)
        {
            StartCoroutine(AnimateIcon());
        }
    }

    private void SetupGameModeButtons()
    {
        if (classicModeButton != null) classicModeButton.onClick.AddListener(() => SelectGameMode(GameMode.Classic));
        if (chaosModeButton != null) chaosModeButton.onClick.AddListener(() => SelectGameMode(GameMode.Chaos));
        if (fateModeButton != null) fateModeButton.onClick.AddListener(() => SelectGameMode(GameMode.Fate));
        if (rapidModeButton != null) rapidModeButton.onClick.AddListener(() => SelectGameMode(GameMode.Rapid));

        SelectGameMode(GameMode.Classic);
    }

    private void SelectGameMode(GameMode mode)
    {
        selectedMode = mode;
        UpdateGameModeButtonAppearance();
    }

    private void UpdateGameModeButtonAppearance()
    {
        Color selectedColor = Color.red;
        Color normalColor = new Color(0.165f, 0.165f, 0.212f, 1f);

        UpdateButtonAppearance(classicModeButton, mode == GameMode.Classic, selectedColor, normalColor);
        UpdateButtonAppearance(chaosModeButton, mode == GameMode.Chaos, selectedColor, normalColor);
        UpdateButtonAppearance(fateModeButton, mode == GameMode.Fate, selectedColor, normalColor);
        UpdateButtonAppearance(rapidModeButton, mode == GameMode.Rapid, selectedColor, normalColor);
    }

    private void UpdateButtonAppearance(Button button, bool isSelected, Color selectedColor, Color normalColor)
    {
        if (button == null) return;

        ColorBlock colors = button.colors;
        colors.normalColor = isSelected ? selectedColor : normalColor;
        colors.highlightedColor = isSelected ? new Color(selectedColor.r * 0.8f, selectedColor.g * 0.8f, selectedColor.b * 0.8f, 1f) : new Color(normalColor.r * 1.2f, normalColor.g * 1.2f, normalColor.b * 1.2f, 1f);
        button.colors = colors;
    }

    private void OnUsernameChanged(string newText)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.username = newText;
        }
        
        if (startButton != null)
        {
            startButton.interactable = !string.IsNullOrEmpty(newText);
        }
    }

    private void OnStartGame()
    {
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.username))
        {
            GameManager.Instance.StartGame(selectedMode, GameManager.Instance.username);
        }
    }

    private IEnumerator AnimateIcon()
    {
        if (appIcon == null) yield break;

        while (true)
        {
            float rotation = Time.time * 30f;
            float scale = Mathf.Sin(Time.time * 2f) * 0.1f + 1f;
            
            appIcon.transform.rotation = Quaternion.Euler(0, 0, rotation);
            appIcon.transform.localScale = Vector3.one * scale;

            yield return null;
        }
    }
}
