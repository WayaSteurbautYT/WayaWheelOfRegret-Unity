using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public GameSession currentSession;
    public List<WheelHistoryEntry> gameHistory = new List<WheelHistoryEntry>();
    
    [Header("Settings")]
    public bool musicEnabled = true;
    public bool sfxEnabled = true;
    public string username = "";

    private const string HISTORY_FILE_NAME = "wheel_history.json";
    private const string SETTINGS_FILE_NAME = "wheel_settings.json";

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = null;
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
        // Start with loading screen
        SceneManager.LoadScene("LoadingScene");
    }

    #region Game Session Management

    public void StartGame(GameMode mode, string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Cannot start game without username");
            return;
        }

        username = playerName;
        
        // Detect personality for Fate mode
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

        // Load game scene
        SceneManager.LoadScene("GameScene");
        SoundManager.Instance?.PlayWhoosh();
    }

    public void AddSpin(string question, string answer, int doom)
    {
        if (currentSession == null)
        {
            Debug.LogWarning("No active game session");
            return;
        }

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

        // Check if game is complete
        if (currentSession.spins.Count >= currentSession.maxSpins)
        {
            CompleteGame();
        }
    }

    public void CompleteGame()
    {
        if (currentSession == null) return;

        currentSession.finalScore = GameState.CalculateFinalScore(currentSession.spins, currentSession.mode);
        currentSession.isComplete = true;

        // Add to history
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
        
        // Keep only last 20 entries
        if (gameHistory.Count > 20)
        {
            gameHistory.RemoveAt(0);
        }

        SaveGameData();

        // Load results scene
        SceneManager.LoadScene("ResultsScene");
    }

    public void ResetGame()
    {
        currentSession = null;
        SceneManager.LoadScene("MenuScene");
    }

    #endregion

    #region Settings Management

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        SoundManager.Instance?.SetMusicEnabled(musicEnabled);
        SaveGameData();
    }

    public void ToggleSfx()
    {
        sfxEnabled = !sfxEnabled;
        SoundManager.Instance?.SetSfxEnabled(sfxEnabled);
        SaveGameData();
    }

    #endregion

    #region History Management

    public void ClearHistory()
    {
        gameHistory.Clear();
        SaveGameData();
    }

    #endregion

    #region Save/Load System

    private void LoadGameData()
    {
        LoadHistory();
        LoadSettings();
    }

    private void LoadHistory()
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, HISTORY_FILE_NAME);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                gameHistory = JsonUtility.FromJson<Wrapper<List<WheelHistoryEntry>>>(json).data;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game history: {e.Message}");
            gameHistory = new List<WheelHistoryEntry>();
        }
    }

    private void LoadSettings()
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, SETTINGS_FILE_NAME);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var settings = JsonUtility.FromJson<Wrapper<GameSettings>>(json).data;
                musicEnabled = settings.musicEnabled;
                sfxEnabled = settings.sfxEnabled;
                username = settings.username;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load settings: {e.Message}");
        }

        // Apply settings to sound manager
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicEnabled(musicEnabled);
            SoundManager.Instance.SetSfxEnabled(sfxEnabled);
        }
    }

    public void SaveGameData()
    {
        SaveHistory();
        SaveSettings();
    }

    private void SaveHistory()
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, HISTORY_FILE_NAME);
            string json = JsonUtility.ToJson(new Wrapper<List<WheelHistoryEntry>>(gameHistory));
            File.WriteAllText(filePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game history: {e.Message}");
        }
    }

    private void SaveSettings()
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, SETTINGS_FILE_NAME);
            var settings = new GameSettings
            {
                musicEnabled = musicEnabled,
                sfxEnabled = sfxEnabled,
                username = username
            };
            string json = JsonUtility.ToJson(new Wrapper<GameSettings>(settings));
            File.WriteAllText(filePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save settings: {e.Message}");
        }
    }

    #endregion

    #region Utility Classes

    [Serializable]
    private class Wrapper<T>
    {
        public T data;
        public Wrapper(T data) { this.data = data; }
    }

    [Serializable]
    private class GameSettings
    {
        public bool musicEnabled;
        public bool sfxEnabled;
        public string username;
    }

    #endregion

    #region Scene Navigation

    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void GoToCredits()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    #endregion
}
