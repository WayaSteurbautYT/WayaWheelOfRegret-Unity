using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelOfRegret : MonoBehaviour
{
    [Header("Wheel Components")]
    public RectTransform wheelTransform;
    public Image wheelImage;
    public TextMeshProUGUI cyclingText;
    
    [Header("Input")]
    public TMP_InputField questionInput;
    public Button spinButton;
    
    [Header("Result Display")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI doomText;

    [Header("Settings")]
    public float spinDuration = 4f;
    public float minRotations = 5f;
    public float maxRotations = 8f;

    private bool isSpinning = false;
    private float currentRotation = 0f;
    private WheelSegmentData currentResult = null;

    private void Start()
    {
        InitializeWheel();
        SetupInputHandlers();
    }

    private void InitializeWheel()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
        if (wheelImage != null) wheelImage.color = new Color(0.1f, 0.1f, 0.12f, 1f);
        CreateSegmentDividers();
    }

    private void CreateSegmentDividers()
    {
        if (wheelTransform == null) return;
        
        int segmentCount = GameState.WHEEL_SEGMENTS.Count;
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
        if (spinButton != null) spinButton.onClick.AddListener(SpinWheel);
        if (questionInput != null) questionInput.onValueChanged.AddListener(OnQuestionChanged);
    }

    private void OnQuestionChanged(string newQuestion)
    {
        if (spinButton != null) spinButton.interactable = !string.IsNullOrEmpty(newQuestion) && !isSpinning;
    }

    public void SpinWheel()
    {
        if (isSpinning || string.IsNullOrEmpty(questionInput?.text)) return;

        isSpinning = true;
        if (spinButton != null) spinButton.interactable = false;
        if (questionInput != null) questionInput.interactable = false;

        float spins = minRotations + UnityEngine.Random.value * (maxRotations - minRotations);
        float segmentAngle = 360f / GameState.WHEEL_SEGMENTS.Count;
        int randomSegment = UnityEngine.Random.Range(0, GameState.WHEEL_SEGMENTS.Count);
        float targetRotation = currentRotation + spins * 360f + randomSegment * segmentAngle;

        StartCoroutine(AnimateSpin(targetRotation, randomSegment));
    }

    private IEnumerator AnimateSpin(float targetRotation, int targetSegment)
    {
        float elapsedTime = 0f;
        float startRotation = currentRotation;

        StartCoroutine(CycleSegmentText());

        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / spinDuration;
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);
            
            currentRotation = Mathf.Lerp(startRotation, targetRotation, easedProgress);
            if (wheelTransform != null) wheelTransform.rotation = Quaternion.Euler(0, 0, -currentRotation);

            yield return null;
        }

        currentRotation = targetRotation;
        if (wheelTransform != null) wheelTransform.rotation = Quaternion.Euler(0, 0, -currentRotation);

        currentResult = GameState.WHEEL_SEGMENTS[targetSegment];
        ShowSpinResult();

        isSpinning = false;
        if (questionInput != null) questionInput.interactable = true;
        if (spinButton != null) spinButton.interactable = true;
    }

    private IEnumerator CycleSegmentText()
    {
        while (isSpinning)
        {
            if (cyclingText != null)
            {
                var randomSegment = GameState.WHEEL_SEGMENTS[UnityEngine.Random.Range(0, GameState.WHEEL_SEGMENTS.Count)];
                cyclingText.text = randomSegment.text;
                cyclingText.transform.localScale = Vector3.one * 1.2f;
            }
            yield return new WaitForSeconds(0.1f);
        }
        if (cyclingText != null) cyclingText.text = "";
    }

    private void ShowSpinResult()
    {
        if (currentResult == null || GameManager.Instance == null) return;

        string answer = GameState.GetAIAnswer(currentResult.doom, GameManager.Instance.currentSession.mode);

        if (resultPanel != null) resultPanel.SetActive(true);
        if (resultText != null) resultText.text = currentResult.text;
        if (answerText != null) answerText.text = answer;
        if (doomText != null) doomText.text = $"Doom: {currentResult.doom}%";

        GameManager.Instance.AddSpin(questionInput.text, answer, currentResult.doom);
    }

    public void ResetWheel()
    {
        isSpinning = false;
        currentRotation = 0f;
        currentResult = null;
        if (wheelTransform != null) wheelTransform.rotation = Quaternion.identity;
        if (resultPanel != null) resultPanel.SetActive(false);
        if (questionInput != null) { questionInput.text = ""; questionInput.interactable = true; }
        if (spinButton != null) spinButton.interactable = false;
    }
}
