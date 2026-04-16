using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Enum definitions
public enum GameMode { Classic, Chaos, Fate, Rapid }
public enum EndingType { Hammer, Pokeball, SCP, Glitch }

// Data classes
[Serializable]
public class SpinResult
{
    public string id;
    public string question;
    public string answer;
    public int doom; // 0-100
    public long timestamp;
    public int spinIndex;
}

[Serializable]
public class FinalScore
{
    public int stars;           // 0-5
    public int regretLevel;     // 0-100
    public int creativityPoints;
    public int stupidityPoints;
    public int totalPoints;
}

[Serializable]
public class GameSession
{
    public string id;
    public GameMode mode;
    public string username;
    public string detectedPersonality; // null if none
    public List<SpinResult> spins;
    public int maxSpins;
    public bool isComplete;
    public FinalScore finalScore;
}

[Serializable]
public class WheelHistoryEntry
{
    public string sessionId;
    public GameMode mode;
    public string username;
    public long date;
    public List<SpinResult> spins;
    public FinalScore finalScore;
}

[Serializable]
public class PersonalityData
{
    public string name;
    public string trait;
    public EndingType endingType;
}

public static class GameState
{
    // Game mode configurations
    public static readonly Dictionary<GameMode, (int maxSpins, string name, string description)> GAME_MODES = 
        new Dictionary<GameMode, (int, string, string)>
    {
        { GameMode.Classic, (1, "Classic", "One question, one fate") },
        { GameMode.Chaos, (6, "Chaos Chain", "6 spins of escalating doom") },
        { GameMode.Fate, (6, "Wheel of Fate", "The wheel knows who you are...") },
        { GameMode.Rapid, (6, "Rapid Fire", "Auto-spin madness") }
    };

    // Wheel segments data
    [Serializable]
    public class WheelSegment
    {
        public string text;
        public string color;
        public bool isRed;
        public int doomValue;
    }

    public static readonly List<WheelSegment> WHEEL_SEGMENTS = new List<WheelSegment>
    {
        new WheelSegment { text = "DO IT", color = "#FF0000", isRed = true, doomValue = 30 },
        new WheelSegment { text = "REGRET IT", color = "#1A1A1F", isRed = false, doomValue = 80 },
        new WheelSegment { text = "CHAOS", color = "#FF0000", isRed = true, doomValue = 100 },
        new WheelSegment { text = "VIBES ONLY", color = "#1A1A1F", isRed = false, doomValue = 20 },
        new WheelSegment { text = "ASK AGAIN", color = "#FF0000", isRed = true, doomValue = 50 },
        new WheelSegment { text = "ABSOLUTELY", color = "#1A1A1F", isRed = false, doomValue = 10 },
        new WheelSegment { text = "NEVER EVER", color = "#FF0000", isRed = true, doomValue = 90 },
        new WheelSegment { text = "COSMIC YES", color = "#1A1A1F", isRed = false, doomValue = 15 },
        new WheelSegment { text = "RUN AWAY", color = "#FF0000", isRed = true, doomValue = 85 },
        new WheelSegment { text = "EMBRACE IT", color = "#1A1A1F", isRed = false, doomValue = 40 },
        new WheelSegment { text = "DOOM AWAITS", color = "#FF0000", isRed = true, doomValue = 95 },
        new WheelSegment { text = "MAYBE...", color = "#1A1A1F", isRed = false, doomValue = 55 }
    };

    // Utility methods
    public static string GenerateId()
    {
        return Guid.NewGuid().ToString("N")[..8];
    }

    public static FinalScore CalculateFinalScore(List<SpinResult> spins, GameMode mode)
    {
        if (spins == null || spins.Count == 0)
            return new FinalScore { stars = 0, regretLevel = 0, creativityPoints = 0, stupidityPoints = 0, totalPoints = 0 };

        int avgDoom = (int)spins.Average(s => s.doom);
        int stars = Mathf.Clamp((100 - avgDoom) / 20, 0, 5);
        int regretLevel = avgDoom;
        
        int totalQuestionLength = spins.Sum(s => s.question?.Length ?? 0);
        int creativity = Mathf.Min(100, totalQuestionLength / spins.Count * 2);
        int stupidity = (int)(avgDoom * 0.8f + UnityEngine.Random.Range(0, 20));

        float multiplier = mode switch
        {
            GameMode.Chaos => 2.0f,
            GameMode.Fate => 1.5f,
            GameMode.Rapid => 1.2f,
            _ => 1.0f
        };

        int totalPoints = (int)((creativity + (100 - regretLevel) + stars * 20) * multiplier);

        return new FinalScore
        {
            stars = stars,
            regretLevel = regretLevel,
            creativityPoints = creativity,
            stupidityPoints = stupidity,
            totalPoints = totalPoints
        };
    }
}
