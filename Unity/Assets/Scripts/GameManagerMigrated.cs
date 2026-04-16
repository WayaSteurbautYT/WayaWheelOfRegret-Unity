using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// Migrated from Next.js game context to Unity GameManager
// Original file: lib/game-context.tsx
// Tests Cursor Unity integration with complete game management

public class GameManagerMigrated : MonoBehaviour
{
    public static GameManagerMigrated Instance { get; private set; }

    [Header("Current Game State")]
    public GameSessionMigrated currentSession;
    public List<WheelHistoryEntryMigrated> gameHistory = new List<WheelHistoryEntryMigrated>();
    
    [Header("Settings")]
    public bool musicEnabled = true;
    public bool sfxEnabled = true;
    public string username = "";

    [Header("Scene Management")]
    public string loadingSceneName = "LoadingScene";
    public string menuSceneName = "MenuScene";
    public string gameSceneName = "GameScene";
    public string resultsSceneName = "ResultsScene";
    public string creditsSceneName = "CreditsScene";

    private const string HISTORY_FILE_NAME = "wheel_history_migrated.json";
    private const string SETTINGS_FILE_NAME = "wheel_settings_migrated.json";

    // Events - migrated from React context
    public System.Action<GameSessionMigrated> onSessionStarted;
    public System.Action<SpinResultMigrated> onSpinAdded;
    public System.Action<GameSessionMigrated> onSessionCompleted;
    public System.Action<bool> onMusicToggled;
    public System.Action<bool> onSfxToggled;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
            Debug.Log("GameManagerMigrated initialized - Cursor Unity Integration Test");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Start with loading scene
        SceneManager.LoadScene(loadingSceneName);
    }

    #region Game Session Management (Migrated from React context)

    public void StartGame(GameModeMigrated mode, string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Cannot start game without username");
            return;
        }

        username = playerName;
        
        // Detect personality for Fate mode
        PersonalityDataMigrated personality = null;
        if (mode == GameModeMigrated.Fate)
        {
            personality = GameStateMigrated.DetectPersonality(playerName);
        }

        currentSession = new GameSessionMigrated
        {
            id = GameStateMigrated.GenerateId(),
            mode = mode,
            username = playerName,
            detectedPersonality = personality?.name,
            spins = new List<SpinResultMigrated>(),
            maxSpins = GameStateMigrated.GAME_MODES[mode].maxSpins,
            isComplete = false,
            finalScore = null
        };

        Debug.Log($"Game started: {mode} mode for {playerName}");
        onSessionStarted?.Invoke(currentSession);

        // Load game scene
        SceneManager.LoadScene(gameSceneName);
    }

    public void AddSpin(string question, string answer, int doom)
    {
        if (currentSession == null)
        {
            Debug.LogWarning("No active game session");
            return;
        }

        var newSpin = new SpinResultMigrated
        {
            id = GameStateMigrated.GenerateId(),
            question = question,
            answer = answer,
            doom = doom,
            timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            spinIndex = currentSession.spins.Count
        };

        currentSession.spins.Add(newSpin);
        Debug.Log($"Spin added: {question} -> {answer} (doom: {doom})");
        onSpinAdded?.Invoke(newSpin);

        // Check if game is complete
        if (currentSession.spins.Count >= currentSession.maxSpins)
        {
            CompleteGame();
        }
    }

    public void CompleteGame()
    {
        if (currentSession == null) return;

        currentSession.finalScore = GameStateMigrated.CalculateFinalScore(currentSession.spins, currentSession.mode);
        currentSession.isComplete = true;

        // Add to history
        var historyEntry = new WheelHistoryEntryMigrated
        {
            sessionId = currentSession.id,
            mode = currentSession.mode,
            username = currentSession.username,
            date = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            spins = new List<SpinResultMigrated>(currentSession.spins),
            finalScore = currentSession.finalScore
        };

        gameHistory.Add(historyEntry);
        
        // Keep only last 20 entries
        if (gameHistory.Count > 20)
        {
            gameHistory.RemoveAt(0);
        }

        SaveGameData();

        Debug.Log($"Game completed: {currentSession.finalScore.totalPoints} points, {currentSession.finalScore.stars} stars");
        onSessionCompleted?.Invoke(currentSession);

        // Load results scene
        SceneManager.LoadScene(resultsSceneName);
    }

    public void ResetGame()
    {
        currentSession = null;
        SceneManager.LoadScene(menuSceneName);
    }

    #endregion

    #region Settings Management (Migrated from React context)

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        onMusicToggled?.Invoke(musicEnabled);
        SaveGameData();
        Debug.Log($"Music toggled: {musicEnabled}");
    }

    public void ToggleSfx()
    {
        sfxEnabled = !sfxEnabled;
        onSfxToggled?.Invoke(sfxEnabled);
        SaveGameData();
        Debug.Log($"SFX toggled: {sfxEnabled}");
    }

    public void SetUsername(string newName)
    {
        username = newName;
        SaveGameData();
    }

    #endregion

    #region History Management

    public void ClearHistory()
    {
        gameHistory.Clear();
        SaveGameData();
        Debug.Log("Game history cleared");
    }

    public WheelHistoryEntryMigrated GetHistoryEntry(string sessionId)
    {
        return gameHistory.Find(entry => entry.sessionId == sessionId);
    }

    #endregion

    #region Save/Load System (Migrated from localStorage)

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
                gameHistory = JsonUtility.FromJson<Wrapper<List<WheelHistoryEntryMigrated>>>(json).data;
                Debug.Log($"Loaded {gameHistory.Count} history entries");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game history: {e.Message}");
            gameHistory = new List<WheelHistoryEntryMigrated>();
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
                Debug.Log($"Settings loaded: music={musicEnabled}, sfx={sfxEnabled}, user={username}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load settings: {e.Message}");
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
            string json = JsonUtility.ToJson(new Wrapper<List<WheelHistoryEntryMigrated>>(gameHistory));
            File.WriteAllText(filePath, json);
            Debug.Log("Game history saved");
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
            Debug.Log("Settings saved");
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
        SceneManager.LoadScene(menuSceneName);
    }

    public void GoToCredits()
    {
        SceneManager.LoadScene(creditsSceneName);
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

    #region Test Methods

    public void TestGameManagerMigration()
    {
        Debug.Log("=== Testing GameManager Migration ===");
        
        // Test game session creation
        StartGame(GameModeMigrated.Classic, "TestUser");
        if (currentSession == null)
        {
            Debug.LogError("Failed to create game session");
            return;
        }

        // Test adding spins
        AddSpin("Should I test this?", "The cosmic forces align...", 25);
        if (currentSession.spins.Count != 1)
        {
            Debug.LogError("Failed to add spin");
            return;
        }

        // Test settings
        bool originalMusic = musicEnabled;
        ToggleMusic();
        if (musicEnabled == originalMusic)
        {
            Debug.LogError("Failed to toggle music");
            return;
        }

        // Test history
        CompleteGame();
        if (gameHistory.Count == 0)
        {
            Debug.LogError("Failed to add to history");
            return;
        }

        Debug.Log("✅ GameManager Migration Test PASSED");
    }

    [ContextMenu("Test GameManager")]
    public void RunGameManagerTest()
    {
        TestGameManagerMigration();
    }

    #endregion

    private void OnDestroy()
    {
        // Clean up events
        onSessionStarted = null;
        onSpinAdded = null;
        onSessionCompleted = null;
        onMusicToggled = null;
        onSfxToggled = null;
    }
}
