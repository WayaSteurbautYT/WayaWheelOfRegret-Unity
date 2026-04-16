using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Current Game State")]
    public GameSession currentSession;
    public List<WheelHistoryEntry> gameHistory = new List<WheelHistoryEntry>();
    
    [Header("Settings")]
    public bool musicEnabled = true;
    public bool sfxEnabled = true;
    public string username = "";

    private const string HISTORY_FILE_NAME = "wheel_history.json";
    private const string SETTINGS_FILE_NAME = "wheel_settings.json";

    public event Action<GameSession> OnSessionStarted;
    public event Action<SpinResult> OnSpinAdded;
    public event Action<GameSession> OnSessionCompleted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    public void StartGame(GameMode mode, string playerName)
    {
        if (string.IsNullOrEmpty(playerName)) return;

        username = playerName;
        
        PersonalityData personality = null;
        if (mode == GameMode.Fate)
        {
            personality = PersonalityDetector.DetectPersonality(playerName);
        }

        currentSession = new GameSession
        {
            id = GameState.GenerateId(),
            mode = mode,
            username = playerName,
            detectedPersonality = personality?.name,
            spins = new List<SpinResult>(),
            maxSpins = GameState.GAME_MODES[mode].maxSpins,
            isComplete = false,
            finalScore = null
        };

        OnSessionStarted?.Invoke(currentSession);
        SceneManager.LoadScene("GameScene");
    }

    public void AddSpin(string question, string answer, int doom)
    {
        if (currentSession == null) return;

        var newSpin = new SpinResult
        {
            id = GameState.GenerateId(),
            question = question,
            answer = answer,
            doom = doom,
            timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            spinIndex = currentSession.spins.Count
        };

        currentSession.spins.Add(newSpin);
        OnSpinAdded?.Invoke(newSpin);

        if (currentSession.spins.Count >= currentSession.maxSpins)
        {
            CompleteGame();
        }
    }

    public void CompleteSpin(WheelSegment segment, string question)
    {
        if (currentSession == null || segment == null) return;

        // Generate AI answer based on doom value and game mode
        string aiAnswer = GameState.GetAIAnswer(segment.doomValue, currentSession.mode);
        
        // Add spin with the actual question from UI
        AddSpin(question, aiAnswer, segment.doomValue);
    }

    public void CompleteGame()
    {
        if (currentSession == null) return;

        currentSession.finalScore = GameState.CalculateFinalScore(currentSession.spins, currentSession.mode);
        currentSession.isComplete = true;

        var historyEntry = new WheelHistoryEntry
        {
            sessionId = currentSession.id,
            mode = currentSession.mode,
            username = currentSession.username,
            date = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            spins = new List<SpinResult>(currentSession.spins),
            finalScore = currentSession.finalScore
        };

        gameHistory.Add(historyEntry);
        if (gameHistory.Count > 20) gameHistory.RemoveAt(0);

        SaveGameData();
        OnSessionCompleted?.Invoke(currentSession);
        SceneManager.LoadScene("ResultsScene");
    }

    public void ResetGame()
    {
        currentSession = null;
        SceneManager.LoadScene("MenuScene");
    }

    public void GoToMenu() => SceneManager.LoadScene("MenuScene");
    public void GoToCredits() => SceneManager.LoadScene("CreditsScene");

    private void LoadGameData()
    {
        try
        {
            string historyPath = Path.Combine(Application.persistentDataPath, HISTORY_FILE_NAME);
            if (File.Exists(historyPath))
            {
                string json = File.ReadAllText(historyPath);
                gameHistory = JsonUtility.FromJson<Wrapper<List<WheelHistoryEntry>>>(json).data;
            }
        }
        catch (Exception e) { Debug.LogError($"Failed to load history: {e.Message}"); }

        try
        {
            string settingsPath = Path.Combine(Application.persistentDataPath, SETTINGS_FILE_NAME);
            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                var settings = JsonUtility.FromJson<Wrapper<GameSettings>>(json).data;
                musicEnabled = settings.musicEnabled;
                sfxEnabled = settings.sfxEnabled;
                username = settings.username;
            }
        }
        catch (Exception e) { Debug.LogError($"Failed to load settings: {e.Message}"); }
    }

    public void SaveGameData()
    {
        try
        {
            string historyPath = Path.Combine(Application.persistentDataPath, HISTORY_FILE_NAME);
            string json = JsonUtility.ToJson(new Wrapper<List<WheelHistoryEntry>>(gameHistory));
            File.WriteAllText(historyPath, json);
        }
        catch (Exception e) { Debug.LogError($"Failed to save history: {e.Message}"); }

        try
        {
            string settingsPath = Path.Combine(Application.persistentDataPath, SETTINGS_FILE_NAME);
            var settings = new GameSettings { musicEnabled = musicEnabled, sfxEnabled = sfxEnabled, username = username };
            string json = JsonUtility.ToJson(new Wrapper<GameSettings>(settings));
            File.WriteAllText(settingsPath, json);
        }
        catch (Exception e) { Debug.LogError($"Failed to save settings: {e.Message}"); }
    }

    [Serializable] private class Wrapper<T> { public T data; public Wrapper(T data) { this.data = data; } }
    [Serializable] private class GameSettings { public bool musicEnabled; public bool sfxEnabled; public string username; }
}
