using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelSpinner : MonoBehaviour
{
    [Header("Wheel Components")]
    public RectTransform wheelTransform;
    public Image wheelImage;
    public Image pointerImage;
    public Image centerIcon;
    public TextMeshProUGUI cyclingText;
    public TextMeshProUGUI subtitleText;

    [Header("Visual Effects")]
    public GameObject lightningContainer;
    public Image outerGlowRing;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI doomText;
    public TextMeshProUGUI spinCountText;

    [Header("Input")]
    public TMP_InputField questionInput;
    public Button spinButton;
    public Button viewFateButton;

    [Header("Settings")]
    public float spinDuration = 4f;
    public float minRotations = 5f;
    public float maxRotations = 8f;
    public float cyclingTextSpeed = 0.1f;
    public Color redColor = Color.red;
    public Color blackColor = new Color(0.1f, 0.1f, 0.12f, 1f); // #1A1A1F
    public Color gunmetalColor = new Color(0.165f, 0.165f, 0.212f, 1f); // #2A2A35

    private bool isSpinning = false;
    private float currentRotation = 0f;
    private GameState.WheelSegment currentResult = null;
    private int currentSpinIndex = 0;

    public event Action<GameState.WheelSegment, string, int> OnSpinComplete;

    private void Start()
    {
        InitializeWheel();
        SetupInputHandlers();
    }

    private void InitializeWheel()
    {
        // Create wheel segments visually
        CreateWheelVisual();
        
        // Set initial state
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        if (viewFateButton != null)
        {
            viewFateButton.gameObject.SetActive(false);
        }

        // Start lightning effects
        StartCoroutine(LightningEffect());
    }

    private void CreateWheelVisual()
    {
        if (wheelImage == null) return;

        // Create a simple wheel with alternating colors
        // In a real implementation, you'd use a sprite or custom mesh
        wheelImage.color = gunmetalColor;
        
        // Add visual segments (simplified for this example)
        // You could create a custom sprite with the 12 segments
    }

    private void SetupInputHandlers()
    {
        if (spinButton != null)
        {
            spinButton.onClick.AddListener(OnSpinButtonClicked);
        }

        if (viewFateButton != null)
        {
            viewFateButton.onClick.AddListener(OnViewFateClicked);
        }

        if (questionInput != null)
        {
            questionInput.onValueChanged.AddListener(OnQuestionChanged);
        }
    }

    private void OnQuestionChanged(string question)
    {
        // Enable/disable spin button based on whether there's a question
        if (spinButton != null)
        {
            spinButton.interactable = !string.IsNullOrEmpty(question) && !isSpinning;
        }
    }

    private void OnSpinButtonClicked()
    {
        if (isSpinning || string.IsNullOrEmpty(questionInput?.text))
        {
            return;
        }

        StartSpin(questionInput.text);
    }

    private void OnViewFateClicked()
    {
        GameManager.Instance.CompleteGame();
    }

    public void StartSpin(string question)
    {
        if (isSpinning) return;

        isSpinning = true;
        currentSpinIndex++;

        // Disable input during spin
        if (questionInput != null)
        {
            questionInput.interactable = false;
        }

        if (spinButton != null)
        {
            spinButton.interactable = false;
        }

        // Update spin count
        UpdateSpinCount();

        // Start spinning animation
        StartCoroutine(SpinSequence(question));
    }

    private IEnumerator SpinSequence(string question)
    {
        // Play start sound
        SoundManager.Instance?.PlaySpinStart();

        // Start cycling text
        StartCoroutine(CycleSegmentText());

        // Calculate final rotation
        float targetRotations = UnityEngine.Random.Range(minRotations, maxRotations);
        float finalRotation = currentRotation + (targetRotations * 360f);
        
        // Select random segment
        int segmentIndex = UnityEngine.Random.Range(0, GameState.WHEEL_SEGMENTS.Count);
        currentResult = GameState.WHEEL_SEGMENTS[segmentIndex];
        
        // Adjust final rotation to land on selected segment
        float segmentAngle = 360f / GameState.WHEEL_SEGMENTS.Count;
        float targetAngle = segmentIndex * segmentAngle;
        finalRotation = Mathf.Floor(finalRotation / 360f) * 360f + targetAngle;

        // Spin animation with easing
        float elapsedTime = 0f;
        float startRotation = currentRotation;

        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / spinDuration;

            // Cubic easing: fast start, slow end
            float easedProgress = CubicEaseOut(progress);
            
            currentRotation = Mathf.Lerp(startRotation, finalRotation, easedProgress);
            wheelTransform.rotation = Quaternion.Euler(0, 0, -currentRotation);

            // Play tick sounds (decreasing frequency as wheel slows)
            if (elapsedTime < spinDuration * 0.8f)
            {
                float tickInterval = Mathf.Lerp(0.05f, 0.3f, progress);
                if (Time.time % tickInterval < Time.deltaTime)
                {
                    SoundManager.Instance?.PlayTick();
                }
            }

            // Glow center icon during spin
            if (centerIcon != null)
            {
                float glow = Mathf.Sin(Time.time * 10f) * 0.3f + 0.7f;
                centerIcon.color = new Color(redColor.r, redColor.g, redColor.b, glow);
            }

            yield return null;
        }

        // Spin complete
        currentRotation = finalRotation;
        wheelTransform.rotation = Quaternion.Euler(0, 0, -currentRotation);

        // Play stop sound
        SoundManager.Instance?.PlaySpinStop();

        // Screen shake
        StartCoroutine(ScreenShake());

        // Lightning burst
        StartCoroutine(LightningBurst());

        yield return new WaitForSeconds(0.5f);

        // Show result
        ShowSpinResult(question);
    }

    private IEnumerator CycleSegmentText()
    {
        while (isSpinning)
        {
            if (cyclingText != null)
            {
                // Pick random segment text
                var randomSegment = GameState.WHEEL_SEGMENTS[UnityEngine.Random.Range(0, GameState.WHEEL_SEGMENTS.Count)];
                cyclingText.text = randomSegment.text;
                
                // Pulse and color shift
                cyclingText.color = randomSegment.isRed ? redColor : Color.white;
                cyclingText.transform.localScale = Vector3.one * 1.2f;
            }

            yield return new WaitForSeconds(cyclingTextSpeed);
        }

        // Reset text
        if (cyclingText != null)
        {
            cyclingText.text = "";
        }
    }

    private void ShowSpinResult(string question)
    {
        if (currentResult == null) return;

        // Generate AI answer
        string answer = AIAnswerGenerator.GetAnswerForSegment(currentResult.text, currentResult.doomValue, GameManager.Instance.currentSession.mode);

        // Show result panel
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
            
            // Set result text with spring animation
            if (resultText != null)
            {
                resultText.text = currentResult.text;
                StartCoroutine(SpringAnimation(resultText.transform));
            }

            if (answerText != null)
            {
                answerText.text = answer;
            }

            if (doomText != null)
            {
                doomText.text = $"Doom: {currentResult.doomValue}% - Spin {currentSpinIndex}/{GameManager.Instance.currentSession.maxSpins}";
            }
        }

        // Play reveal sound
        SoundManager.Instance?.PlayReveal();

        // Add spin to game session
        GameManager.Instance.AddSpin(question, answer, currentResult.doomValue);

        // Re-enable input if game isn't complete
        if (GameManager.Instance.currentSession.spins.Count < GameManager.Instance.currentSession.maxSpins)
        {
            if (questionInput != null)
            {
                questionInput.text = "";
                questionInput.interactable = true;
            }

            if (spinButton != null)
            {
                spinButton.interactable = false;
            }
        }
        else
        {
            // Game complete - show view fate button
            if (viewFateButton != null)
            {
                viewFateButton.gameObject.SetActive(true);
            }

            if (spinButton != null)
            {
                spinButton.gameObject.SetActive(false);
            }
        }

        isSpinning = false;
    }

    private void UpdateSpinCount()
    {
        if (spinCountText != null && GameManager.Instance.currentSession != null)
        {
            spinCountText.text = $"Spin {currentSpinIndex}/{GameManager.Instance.currentSession.maxSpins} - {GameState.GAME_MODES[GameManager.Instance.currentSession.mode].name}";
        }
    }

    private IEnumerator LightningEffect()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.5f));

            if (UnityEngine.Random.value < 0.3f && lightningContainer != null)
            {
                CreateLightningFlash();
            }
        }
    }

    private void CreateLightningFlash()
    {
        GameObject lightning = new GameObject("LightningFlash");
        Image lightningImage = lightning.AddComponent<Image>();
        
        lightningImage.color = new Color(0.545f, 0f, 1f, 0.6f); // Purple
        
        RectTransform rect = lightning.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        rect.anchorMax = rect.anchorMin + new Vector2(0.1f, 0.1f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        lightning.transform.SetParent(lightningContainer.transform, false);

        // Auto-destroy after flash
        Destroy(lightning, 0.1f);
    }

    private IEnumerator LightningBurst()
    {
        // Create multiple lightning flashes for dramatic effect
        for (int i = 0; i < 5; i++)
        {
            CreateLightningFlash();
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator ScreenShake()
    {
        Vector3 originalPosition = transform.position;
        float shakeDuration = 0.3f;
        float shakeMagnitude = 10f;

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = originalPosition + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition;
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

            // Spring easing: overshoot and settle
            float spring = Mathf.Sin(progress * Mathf.PI * 2f) * Mathf.Exp(-progress * 5f);
            float scale = Mathf.Lerp(0f, 1f, progress) + spring * 0.2f;

            target.localScale = originalScale * scale;
            yield return null;
        }

        target.localScale = originalScale;
    }

    private float CubicEaseOut(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    public void ResetWheel()
    {
        isSpinning = false;
        currentRotation = 0f;
        currentResult = null;
        currentSpinIndex = 0;

        if (wheelTransform != null)
        {
            wheelTransform.rotation = Quaternion.identity;
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        if (questionInput != null)
        {
            questionInput.text = "";
            questionInput.interactable = true;
        }

        if (spinButton != null)
        {
            spinButton.interactable = false;
            spinButton.gameObject.SetActive(true);
        }

        if (viewFateButton != null)
        {
            viewFateButton.gameObject.SetActive(false);
        }

        UpdateSpinCount();
    }
}
