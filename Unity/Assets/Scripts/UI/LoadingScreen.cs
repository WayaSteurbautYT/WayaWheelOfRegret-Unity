using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI Components")]
    public Image appIcon;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public Image progressBar;
    public TextMeshProUGUI percentageText;
    public TextMeshProUGUI tipText;
    public TextMeshProUGUI versionText;
    public TextMeshProUGUI madeByText;

    [Header("Visual Effects")]
    public GameObject lightningContainer;
    public Image glowRing;
    public Color redColor = Color.red;
    public Color purpleColor = new Color(0.545f, 0f, 1f, 1f); // #8B00FF
    public Color silverColor = new Color(0.545f, 0.545f, 0.545f, 1f); // #8B8B8B

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
        // Set initial colors
        if (titleText != null)
        {
            titleText.color = redColor;
        }

        if (subtitleText != null)
        {
            subtitleText.color = silverColor;
        }

        if (progressBar != null)
        {
            // Set progress bar background color
            progressBar.color = new Color(0.1f, 0.1f, 0.12f, 1f); // #1A1A1F
        }

        if (madeByText != null)
        {
            madeByText.text = "Made by WayaCreate";
        }

        if (versionText != null)
        {
            versionText.text = "v1.0.0 Build 2024.1";
        }

        // Start lightning effects
        StartCoroutine(LightningEffect());
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

            // Animate elements
            AnimateElements(progress);

            yield return null;
        }

        // Loading complete
        StartCoroutine(LoadingComplete());
    }

    private void UpdateProgressBar(float progress)
    {
        if (progressBar != null)
        {
            // Animate fill amount
            progressBar.fillAmount = progress;

            // Add glow effect at the fill point
            if (progress > 0f && progress < 1f)
            {
                // You could add a glowing edge effect here
                // For now, we'll just pulse the alpha
                Color barColor = progressBar.color;
                barColor.a = 0.8f + Mathf.Sin(Time.time * 10f) * 0.2f;
                progressBar.color = barColor;
            }
        }

        if (percentageText != null)
        {
            percentageText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
        }
    }

    private void AnimateElements(float progress)
    {
        // Animate app icon entrance
        if (appIcon != null)
        {
            float scale = Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0f, 1f, progress));
            appIcon.transform.localScale = Vector3.one * scale;

            // Add spring effect near the end
            if (progress > 0.8f)
            {
                float spring = Mathf.Sin((progress - 0.8f) * 10f) * 0.1f;
                appIcon.transform.localScale = Vector3.one * (scale + spring);
            }
        }

        // Animate glow ring
        if (glowRing != null)
        {
            float pulse = Mathf.Sin(Time.time * 3f) * 0.2f + 0.8f;
            glowRing.color = new Color(redColor.r, redColor.g, redColor.b, pulse);
        }

        // Animate title appearance
        if (titleText != null)
        {
            Color titleAlpha = titleText.color;
            titleAlpha.a = Mathf.Lerp(0f, 1f, progress);
            titleText.color = titleAlpha;
        }
    }

    private IEnumerator LightningEffect()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));

            if (Random.value < 0.3f && lightningContainer != null)
            {
                // Create lightning flash
                GameObject lightning = CreateLightningBolt();
                if (lightning != null)
                {
                    lightning.transform.SetParent(lightningContainer.transform, false);
                    
                    // Flash effect
                    yield return new WaitForSeconds(0.1f);
                    Destroy(lightning);
                }
            }
        }
    }

    private GameObject CreateLightningBolt()
    {
        // Create a simple lightning effect using UI elements
        GameObject lightning = new GameObject("LightningBolt");
        Image lightningImage = lightning.AddComponent<Image>();
        
        // Set lightning properties
        lightningImage.color = new Color(purpleColor.r, purpleColor.g, purpleColor.b, 0.8f);
        
        // Random position and size
        RectTransform rect = lightning.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        rect.anchorMax = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        rect.sizeDelta = new Vector2(Random.Range(50f, 200f), Random.Range(2f, 5f));
        rect.rotation = Quaternion.Euler(0, 0, Random.Range(-45f, 45f));

        return lightning;
    }

    private IEnumerator LoadingComplete()
    {
        // Show completion state
        if (tipText != null)
        {
            tipText.text = "Loading complete!";
        }

        // Show "Made by WayaCreate" prominently
        if (madeByText != null)
        {
            madeByText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(2f);

        // Load menu scene
        SceneManager.LoadScene("MenuScene");
    }
}
