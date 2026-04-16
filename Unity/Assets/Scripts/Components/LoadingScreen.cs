using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI Components - Drag these here")]
    public Image appIcon;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public Image progressBar;
    public TextMeshProUGUI percentageText;
    public TextMeshProUGUI tipText;
    public TextMeshProUGUI versionText;

    [Header("Loading Settings")]
    public float loadingDuration = 3f;
    public float tipChangeInterval = 1.5f;

    private string[] loadingTips = {
        "The wheel sees all...",
        "Preparing your fate...",
        "Loading cosmic chaos...",
        "Calibrating regret levels...",
        "Summoning the void...",
        "The wheel awaits your question..."
    };

    private void Start()
    {
        InitializeScreen();
        StartCoroutine(LoadingSequence());
    }

    private void InitializeScreen()
    {
        // Set initial text
        if (titleText != null)
        {
            titleText.text = "Waya's Wheel";
            titleText.color = Color.red;
        }

        if (subtitleText != null)
        {
            subtitleText.text = "of Regret";
            subtitleText.color = Color.gray;
        }

        if (versionText != null)
        {
            versionText.text = "v1.0.0 Build 2024.1";
        }

        if (progressBar != null)
        {
            progressBar.fillAmount = 0f;
            progressBar.color = new Color(0.1f, 0.1f, 0.12f, 1f); // #1A1A1F
        }
    }

    private IEnumerator LoadingSequence()
    {
        float elapsedTime = 0f;
        int currentTipIndex = 0;
        float lastTipChange = 0f;

        while (elapsedTime < loadingDuration)
        {
            elapsedTime += Time.deltaTime;

            // Update progress
            float progress = Mathf.Clamp01(elapsedTime / loadingDuration);
            UpdateProgressBar(progress);

            // Change tips periodically
            if (elapsedTime - lastTipChange >= tipChangeInterval)
            {
                currentTipIndex = (currentTipIndex + 1) % loadingTips.Length;
                if (tipText != null)
                {
                    tipText.text = loadingTips[currentTipIndex];
                }
                lastTipChange = elapsedTime;
            }

            yield return null;
        }

        // Loading complete - go to menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }

    private void UpdateProgressBar(float progress)
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = progress;
        }

        if (percentageText != null)
        {
            percentageText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
        }
    }
}
