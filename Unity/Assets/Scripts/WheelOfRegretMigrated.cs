using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Migrated from Next.js TypeScript to Unity C#
// Original file: components/wheel-of-regret.tsx
// This tests Cursor Unity integration with direct component migration

public class WheelOfRegretMigrated : MonoBehaviour
{
    [Header("Wheel Components")]
    public RectTransform wheelTransform;
    public Image wheelImage;
    public Image pointerImage;
    public Image centerIcon;
    public TextMeshProUGUI cyclingText;
    public TextMeshProUGUI subtitleText;

    [Header("Input")]
    public TMP_InputField questionInput;
    public Button spinButton;

    [Header("Result Display")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI regretRatingText;

    [Header("Visual Effects")]
    public GameObject lightningContainer;
    public GameObject particleContainer;

    [Header("Settings")]
    public float spinDuration = 4f;
    public float minRotations = 5f;
    public float maxRotations = 10f;
    public float cyclingTextSpeed = 0.1f;

    // State variables - migrated from React useState
    private bool isSpinning = false;
    private float rotation = 0f;
    private WheelSegmentMigrated result = null;
    private string regretRating = "";
    private string question = "";
    private bool showResult = false;
    private bool eyesGlowing = false;

    // Lightning system - migrated from React state
    private List<LightningBolt> lightningBolts = new List<LightningBolt>();

    // Regret ratings - migrated from const array
    private readonly string[] REGRET_RATINGS = {
        "0/10 - You got lucky... this time",
        "3/10 - Minor consequences incoming",
        "5/10 - Balanced chaos, as the wheel intended",
        "7/10 - Future you is already disappointed",
        "10/10 - Maximum regret unlocked",
        "???/10 - The wheel refuses to comment",
        "DOOM/10 - You should not have spun",
    };

    [Serializable]
    public class LightningBolt
    {
        public int id;
        public string path;
        public float delay;
        public GameObject gameObject;
    }

    private void Start()
    {
        InitializeWheel();
        SetupInputHandlers();
        StartCoroutine(LightningEffect());
    }

    private void InitializeWheel()
    {
        // Set initial state
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        if (cyclingText != null)
        {
            cyclingText.text = "";
        }

        if (subtitleText != null)
        {
            subtitleText.text = "The wheel decides...";
        }

        // Create visual wheel segments
        CreateWheelVisuals();
    }

    private void CreateWheelVisuals()
    {
        if (wheelImage == null) return;

        // Create a simple wheel visualization
        // In a real implementation, you'd use a sprite or custom mesh
        wheelImage.color = new Color(0.1f, 0.1f, 0.12f, 1f); // #1A1A1F

        // Create segment dividers (visual only)
        CreateSegmentDividers();
    }

    private void CreateSegmentDividers()
    {
        if (wheelTransform == null) return;

        int segmentCount = GameStateMigrated.WHEEL_SEGMENTS.Count;
        float anglePerSegment = 360f / segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject divider = new GameObject($"Divider_{i}");
            divider.transform.SetParent(wheelTransform, false);

            Image dividerImage = divider.AddComponent<Image>();
            dividerImage.color = Color.gray;

            RectTransform rect = divider.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(2, 100);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0);
            rect.anchoredPosition = Vector2.zero;
            rect.rotation = Quaternion.Euler(0, 0, i * anglePerSegment);
        }
    }

    private void SetupInputHandlers()
    {
        if (spinButton != null)
        {
            spinButton.onClick.AddListener(SpinWheel);
        }

        if (questionInput != null)
        {
            questionInput.onValueChanged.AddListener(OnQuestionChanged);
        }
    }

    private void OnQuestionChanged(string newQuestion)
    {
        question = newQuestion;
        
        if (spinButton != null)
        {
            spinButton.interactable = !string.IsNullOrEmpty(question) && !isSpinning;
        }
    }

    // Migrated from React useCallback spinWheel
    public void SpinWheel()
    {
        if (isSpinning) return;

        isSpinning = true;
        showResult = false;
        result = null;
        eyesGlowing = true;

        GenerateLightning();

        // Random spin: 5-10 full rotations + random segment
        float spins = minRotations + UnityEngine.Random.value * (maxRotations - minRotations);
        float segmentAngle = 360f / GameStateMigrated.WHEEL_SEGMENTS.Count;
        int randomSegment = UnityEngine.Random.Range(0, GameStateMigrated.WHEEL_SEGMENTS.Count);
        float targetRotation = rotation + spins * 360f + randomSegment * segmentAngle;

        // Start spin animation
        StartCoroutine(AnimateSpin(targetRotation, randomSegment));
    }

    private IEnumerator AnimateSpin(float targetRotation, int targetSegment)
    {
        float elapsedTime = 0f;
        float startRotation = rotation;

        // Start cycling text
        StartCoroutine(CycleSegmentText());

        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / spinDuration;

            // Easing function (cubic out)
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);
            
            rotation = Mathf.Lerp(startRotation, targetRotation, easedProgress);
            
            if (wheelTransform != null)
            {
                wheelTransform.rotation = Quaternion.Euler(0, 0, -rotation);
            }

            // Glow effect during spin
            if (centerIcon != null && eyesGlowing)
            {
                float glow = Mathf.Sin(Time.time * 10f) * 0.3f + 0.7f;
                centerIcon.color = new Color(1f, glow, glow, 1f);
            }

            yield return null;
        }

        // Spin complete
        rotation = targetRotation;
        if (wheelTransform != null)
        {
            wheelTransform.rotation = Quaternion.Euler(0, 0, -rotation);
        }

        // Set result
        result = GameStateMigrated.WHEEL_SEGMENTS[targetSegment];
        regretRating = REGRET_RATINGS[UnityEngine.Random.Range(0, REGRET_RATINGS.Length)];
        showResult = true;
        eyesGlowing = false;

        // Show result panel
        ShowSpinResult();

        isSpinning = false;
    }

    private IEnumerator CycleSegmentText()
    {
        while (isSpinning)
        {
            if (cyclingText != null)
            {
                var randomSegment = GameStateMigrated.WHEEL_SEGMENTS[UnityEngine.Random.Range(0, GameStateMigrated.WHEEL_SEGMENTS.Count)];
                cyclingText.text = randomSegment.text;
                
                // Pulse effect
                cyclingText.transform.localScale = Vector3.one * 1.2f;
            }

            yield return new WaitForSeconds(cyclingTextSpeed);
        }

        if (cyclingText != null)
        {
            cyclingText.text = "";
        }
    }

    private void ShowSpinResult()
    {
        if (result == null) return;

        // Generate AI answer
        string aiAnswer = GameStateMigrated.GetAIAnswer(result.doom, GameModeMigrated.Classic);

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (resultText != null)
        {
            resultText.text = result.text;
            StartCoroutine(SpringAnimation(resultText.transform));
        }

        if (answerText != null)
        {
            answerText.text = aiAnswer;
        }

        if (regretRatingText != null)
        {
            regretRatingText.text = regretRating;
        }

        // Reset center icon color
        if (centerIcon != null)
        {
            centerIcon.color = Color.white;
        }
    }

    // Migrated from useCallback generateLightning
    private void GenerateLightning()
    {
        ClearLightning();

        for (int i = 0; i < 5; i++)
        {
            var bolt = new LightningBolt
            {
                id = i,
                path = GenerateLightningPath(),
                delay = UnityEngine.Random.value * 0.5f,
                gameObject = CreateLightningBolt()
            };
            
            lightningBolts.Add(bolt);
        }

        // Clear lightning after delay
        StartCoroutine(ClearLightningAfterDelay(0.5f));
    }

    private string GenerateLightningPath()
    {
        string path = $"M{UnityEngine.Random.value * 100},0 ";
        float y = 0;
        
        while (y < 100)
        {
            y += 15 + UnityEngine.Random.value * 20;
            float x = UnityEngine.Random.value * 100;
            path += $"L{x},{Mathf.Min(y, 100)} ";
        }
        
        return path;
    }

    private GameObject CreateLightningBolt()
    {
        GameObject lightning = new GameObject("LightningBolt");
        lightning.transform.SetParent(lightningContainer.transform, false);

        Image lightningImage = lightning.AddComponent<Image>();
        lightningImage.color = new Color(0.545f, 0f, 1f, 0.8f); // Purple

        RectTransform rect = lightning.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(UnityEngine.Random.Range(50, 200), UnityEngine.Random.Range(2, 5));
        rect.anchorMin = new Vector2(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        rect.anchorMax = rect.anchorMin + new Vector2(0.1f, 0.1f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-45f, 45f));

        return lightning;
    }

    private void ClearLightning()
    {
        foreach (var bolt in lightningBolts)
        {
            if (bolt.gameObject != null)
            {
                Destroy(bolt.gameObject);
            }
        }
        lightningBolts.Clear();
    }

    private IEnumerator ClearLightningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClearLightning();
    }

    private IEnumerator LightningEffect()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            if (UnityEngine.Random.value > 0.7f)
            {
                GenerateLightning();
            }
        }
    }

    private IEnumerator SpringAnimation(Transform target)
    {
        Vector3 originalScale = target.localScale;
        target.localScale = Vector3.zero;

        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float spring = Mathf.Sin(progress * Mathf.PI * 2f) * Mathf.Exp(-progress * 5f);
            float scale = Mathf.Lerp(0f, 1f, progress) + spring * 0.2f;

            target.localScale = originalScale * scale;
            yield return null;
        }

        target.localScale = originalScale;
    }

    // Test method to verify migration works
    public void TestMigration()
    {
        Debug.Log("=== Testing Wheel of Regret Migration ===");
        
        // Test wheel segments
        Debug.Log($"Wheel segments count: {GameStateMigrated.WHEEL_SEGMENTS.Count}");
        Debug.Log($"First segment: {GameStateMigrated.WHEEL_SEGMENTS[0].text}");
        
        // Test AI answers
        string positiveAnswer = GameStateMigrated.GetAIAnswer(20, GameModeMigrated.Classic);
        string negativeAnswer = GameStateMigrated.GetAIAnswer(80, GameModeMigrated.Classic);
        string chaosAnswer = GameStateMigrated.GetAIAnswer(50, GameModeMigrated.Chaos);
        
        Debug.Log($"Positive answer: {positiveAnswer}");
        Debug.Log($"Negative answer: {negativeAnswer}");
        Debug.Log($"Chaos answer: {chaosAnswer}");
        
        // Test personality detection
        var personality = GameStateMigrated.DetectPersonality("pewdiepie");
        Debug.Log($"Personality detection: {personality?.name} - {personality?.trait}");
        
        // Test score calculation
        var testSpins = new List<SpinResultMigrated>
        {
            new SpinResultMigrated { id = "1", question = "Test question", answer = "Test answer", doom = 50, timestamp = DateTime.Now.Ticks, spinIndex = 0 }
        };
        var score = GameStateMigrated.CalculateFinalScore(testSpins, GameModeMigrated.Classic);
        Debug.Log($"Test score: {score.totalPoints} points, {score.stars} stars");
        
        Debug.Log("=== Migration Test Complete ===");
    }

    private void OnDestroy()
    {
        // Clean up
        ClearLightning();
    }
}
