using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameUIController : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_InputField questionInput;
    public Button spinButton;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI doomText;
    public TextMeshProUGUI answerText;
    
    [Header("Wheel Components")]
    public WheelController wheelController;
    public SoundManager soundManager;
    
    [Header("Game State")]
    public int currentSpins = 0;
    public int maxSpins = 1;
    public GameMode currentGameMode = GameMode.Classic;
    
    private bool isProcessing = false;
    
    private void Start()
    {
        InitializeUI();
    }
    
    private void InitializeUI()
    {
        // Set up button listeners
        if (spinButton != null)
        {
            spinButton.onClick.AddListener(OnSpinButtonClicked);
        }
        
        if (questionInput != null)
        {
            questionInput.onValueChanged.AddListener(OnQuestionChanged);
        }
        
        // Hide result panel initially
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
        
        // Set up wheel controller
        if (wheelController != null)
        {
            wheelController.OnWheelStopped += OnWheelStopped;
        }
        
        UpdateUI();
    }
    
    private void OnQuestionChanged(string question)
    {
        // Enable/disable spin button based on question input
        if (spinButton != null)
        {
            spinButton.interactable = !string.IsNullOrEmpty(question) && !isProcessing && !wheelController.IsSpinning();
        }
    }
    
    private void OnSpinButtonClicked()
    {
        if (isProcessing || wheelController.IsSpinning()) return;
        
        string question = questionInput?.text ?? "";
        if (string.IsNullOrEmpty(question)) return;
        
        StartCoroutine(SpinWheel(question));
    }
    
    private IEnumerator SpinWheel(string question)
    {
        isProcessing = true;
        
        // Disable UI during spin
        if (spinButton != null)
            spinButton.interactable = false;
            
        if (questionInput != null)
            questionInput.interactable = false;
        
        // Play spin sound
        if (soundManager != null)
        {
            soundManager.PlaySpinStart();
        }
        
        // Spin the wheel
        if (wheelController != null)
        {
            wheelController.SpinWheel();
        }
        
        // Wait for wheel to stop
        while (wheelController != null && wheelController.IsSpinning())
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(1f);
        
        isProcessing = false;
        UpdateUI();
    }
    
    private void OnWheelStopped(WheelSegment selectedSegment)
    {
        if (selectedSegment == null) return;
        
        // Show result
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }
        
        if (resultText != null)
        {
            resultText.text = "The wheel has spoken!";
        }
        
        if (doomText != null)
        {
            doomText.text = $"Doom Level: {selectedSegment.doomValue}%";
        }
        
        if (answerText != null)
        {
            answerText.text = selectedSegment.answerText;
        }
        
        // Play result sound
        if (soundManager != null)
        {
            if (selectedSegment.doomValue > 70)
            {
                soundManager.PlayRegretSound();
            }
            else
            {
                soundManager.PlaySuccessSound();
            }
        }
        
        // Increment spin count
        currentSpins++;
        
        // Check if game is complete
        if (currentSpins >= maxSpins)
        {
            StartCoroutine(EndGame());
        }
        else
        {
            // Allow next spin
            UpdateUI();
        }
    }
    
    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(3f);
        
        // Save game session
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteSpin(selectedSegment);
        }
        
        // Load results scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("ResultsScene");
    }
    
    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
        
        switch (mode)
        {
            case GameMode.Classic:
                maxSpins = 1;
                break;
            case GameMode.Chaos:
                maxSpins = 6;
                break;
            case GameMode.Fate:
                maxSpins = 6;
                break;
            case GameMode.Rapid:
                maxSpins = 6;
                break;
        }
        
        currentSpins = 0;
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        // Update spin button
        if (spinButton != null)
        {
            bool canSpin = !isProcessing && !wheelController.IsSpinning() && 
                           !string.IsNullOrEmpty(questionInput?.text) && currentSpins < maxSpins;
            spinButton.interactable = canSpin;
        }
        
        // Update input field
        if (questionInput != null)
        {
            questionInput.interactable = !isProcessing && !wheelController.IsSpinning() && currentSpins < maxSpins;
        }
        
        // Update spin counter
        if (resultText != null && !resultPanel.activeInHierarchy)
        {
            resultText.text = $"Spins: {currentSpins}/{maxSpins}";
        }
    }
    
    public void ResetGame()
    {
        currentSpins = 0;
        isProcessing = false;
        
        if (questionInput != null)
        {
            questionInput.text = "";
        }
        
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
        
        if (wheelController != null)
        {
            wheelController.ResetWheel();
        }
        
        UpdateUI();
    }
}
