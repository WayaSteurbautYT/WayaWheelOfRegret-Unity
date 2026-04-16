using System;
using System.Collections.Generic;
using System.Linq;

// Migrated from Next.js TypeScript to Unity C#
// Original file: lib/game-state.ts
// This is a direct migration to test Cursor Unity integration

public enum GameModeMigrated 
{
    Classic,      // Single spin, simple answer
    Chaos,        // 6 spins, increasingly chaotic
    Fate,         // AI personality detection + special endings
    Rapid         // Auto-spin mode, fast decisions
}

[Serializable]
public class SpinResultMigrated
{
    public string id;
    public string question;
    public string answer;
    public int doom;      // 0-100 doom level
    public long timestamp;
    public int spinIndex;
}

[Serializable]
public class FinalScoreMigrated
{
    public int stars;           // 0-5
    public int regretLevel;     // 0-100
    public int creativityPoints;
    public int stupidityPoints;
    public int totalPoints;
}

[Serializable]
public class GameSessionMigrated
{
    public string id;
    public GameModeMigrated mode;
    public string username;
    public string detectedPersonality;
    public List<SpinResultMigrated> spins;
    public int maxSpins;
    public bool isComplete;
    public FinalScoreMigrated finalScore;
}

[Serializable]
public class WheelHistoryEntryMigrated
{
    public string sessionId;
    public GameModeMigrated mode;
    public string username;
    public long date;
    public List<SpinResultMigrated> spins;
    public FinalScoreMigrated finalScore;
}

[Serializable]
public class WheelSegmentMigrated
{
    public string text;
    public string color;
    public bool isRed;
    public int doom;
}

[Serializable]
public class PersonalityDataMigrated
{
    public string name;
    public string trait;
    public string ending; // "hammer" | "pokeball" | "scp" | "glitch"
}

[Serializable]
public class GameModeDataMigrated
{
    public string name;
    public string description;
    public int maxSpins;
    public string icon;
}

public static class GameStateMigrated
{
    // Wheel segments - migrated from WHEEL_SEGMENTS
    public static readonly List<WheelSegmentMigrated> WHEEL_SEGMENTS = new List<WheelSegmentMigrated>
    {
        new WheelSegmentMigrated { text = "DO IT", color = "#FF0000", isRed = true, doom = 30 },
        new WheelSegmentMigrated { text = "REGRET IT", color = "#1A1A1F", isRed = false, doom = 80 },
        new WheelSegmentMigrated { text = "CHAOS", color = "#FF0000", isRed = true, doom = 100 },
        new WheelSegmentMigrated { text = "VIBES ONLY", color = "#1A1A1F", isRed = false, doom = 20 },
        new WheelSegmentMigrated { text = "ASK AGAIN", color = "#FF0000", isRed = true, doom = 50 },
        new WheelSegmentMigrated { text = "ABSOLUTELY", color = "#1A1A1F", isRed = false, doom = 10 },
        new WheelSegmentMigrated { text = "NEVER EVER", color = "#FF0000", isRed = true, doom = 90 },
        new WheelSegmentMigrated { text = "COSMIC YES", color = "#1A1A1F", isRed = false, doom = 15 },
        new WheelSegmentMigrated { text = "RUN AWAY", color = "#FF0000", isRed = true, doom = 85 },
        new WheelSegmentMigrated { text = "EMBRACE IT", color = "#1A1A1F", isRed = false, doom = 40 },
        new WheelSegmentMigrated { text = "DOOM AWAITS", color = "#FF0000", isRed = true, doom = 95 },
        new WheelSegmentMigrated { text = "MAYBE...", color = "#1A1A1F", isRed = false, doom = 55 }
    };

    // AI answer templates - migrated from AI_ANSWER_TEMPLATES
    public static readonly Dictionary<string, string[]> AI_ANSWER_TEMPLATES = new Dictionary<string, string[]>
    {
        { "positive", new string[] {
            "The cosmic forces align in your favor... this time.",
            "The wheel has spoken: fortune smiles upon your foolish endeavor.",
            "Against all odds, the universe says YES. Don't waste it.",
            "The stars whisper approval. But they're always watching...",
            "Proceed, mortal. The wheel finds your chaos... amusing."
        }},
        { "negative", new string[] {
            "The void laughs at your question. The answer is NO.",
            "The wheel has deemed this path... unwise. Very unwise.",
            "Your fate is sealed with rejection. Accept it.",
            "The cosmic balance requires your disappointment today.",
            "NAH. The wheel doesn't even need to think about this one."
        }},
        { "chaotic", new string[] {
            "The wheel spins into madness... DO IT BUT REGRET IT IMMEDIATELY.",
            "CHAOS DEMANDS: Do the opposite of what you think is right.",
            "The answer exists in a quantum state of YES and NO simultaneously.",
            "Your question has angered the ancient forces. Prepare for consequences.",
            "The wheel grants permission but revokes happiness."
        }},
        { "cryptic", new string[] {
            "Ask yourself: what would your past self think? Now do the opposite.",
            "The answer lies within the 7th dimension. Good luck accessing that.",
            "When the moon is full and the stars align... still no.",
            "The prophecy speaks of one who asks this question. It doesn't end well.",
            "Your ancestors are watching. They're disappointed either way."
        }}
    };

    // Known personalities - migrated from KNOWN_PERSONALITIES
    public static readonly Dictionary<string, PersonalityDataMigrated> KNOWN_PERSONALITIES = new Dictionary<string, PersonalityDataMigrated>
    {
        { "pewdiepie", new PersonalityDataMigrated { name = "PewDiePie", trait = "Swedish chaos energy", ending = "hammer" }},
        { "mrbeast", new PersonalityDataMigrated { name = "MrBeast", trait = "Philanthropic madness", ending = "pokeball" }},
        { "markiplier", new PersonalityDataMigrated { name = "Markiplier", trait = "Dramatic screaming", ending = "scp" }},
        { "jacksepticeye", new PersonalityDataMigrated { name = "Jacksepticeye", trait = "Irish luck (or lack thereof)", ending = "hammer" }},
        { "dream", new PersonalityDataMigrated { name = "Dream", trait = "Speedrun destiny", ending = "glitch" }},
        { "tommyinnit", new PersonalityDataMigrated { name = "TommyInnit", trait = "Chaotic child energy", ending = "hammer" }},
        { "technoblade", new PersonalityDataMigrated { name = "Technoblade", trait = "Never dies... or does he?", ending = "scp" }},
        { "ninja", new PersonalityDataMigrated { name = "Ninja", trait = "Gamer rage mode", ending = "glitch" }},
        { "pokimane", new PersonalityDataMigrated { name = "Pokimane", trait = "Chat decides your fate", ending = "pokeball" }},
        { "xqc", new PersonalityDataMigrated { name = "xQc", trait = "Incomprehensible speed", ending = "glitch" }},
        { "ludwig", new PersonalityDataMigrated { name = "Ludwig", trait = "Subathon suffering", ending = "hammer" }},
        { "valkyrae", new PersonalityDataMigrated { name = "Valkyrae", trait = "Among Us paranoia", ending = "scp" }},
        { "corpse", new PersonalityDataMigrated { name = "Corpse Husband", trait = "Deep voice doom", ending = "scp" }},
        { "sykkuno", new PersonalityDataMigrated { name = "Sykkuno", trait = "Suspiciously wholesome", ending = "pokeball" }},
        { "waya", new PersonalityDataMigrated { name = "Waya", trait = "Vibe coding master", ending = "hammer" }},
        { "wayacreate", new PersonalityDataMigrated { name = "WayaCreate", trait = "The creator themselves", ending = "hammer" }}
    };

    // Game modes - migrated from GAME_MODES
    public static readonly Dictionary<GameModeMigrated, GameModeDataMigrated> GAME_MODES = new Dictionary<GameModeMigrated, GameModeDataMigrated>
    {
        { GameModeMigrated.Classic, new GameModeDataMigrated { name = "Classic", description = "One question, one fate. Simple chaos.", maxSpins = 1, icon = "🎯" }},
        { GameModeMigrated.Chaos, new GameModeDataMigrated { name = "Chaos Chain", description = "6 spins of escalating doom. Each answer feeds the next.", maxSpins = 6, icon = "🔥" }},
        { GameModeMigrated.Fate, new GameModeDataMigrated { name = "Wheel of Fate", description = "The wheel knows who you are... or does it?", maxSpins = 6, icon = "⚡" }},
        { GameModeMigrated.Rapid, new GameModeDataMigrated { name = "Rapid Fire", description = "Auto-spin madness. No time to think.", maxSpins = 6, icon = "💨" }}
    };

    // Utility functions - migrated from original TypeScript

    public static string GenerateId()
    {
        return Guid.NewGuid().ToString("N")[..8];
    }

    public static FinalScoreMigrated CalculateFinalScore(List<SpinResultMigrated> spins, GameModeMigrated mode)
    {
        if (spins == null || spins.Count == 0)
        {
            return new FinalScoreMigrated { stars = 0, regretLevel = 0, creativityPoints = 0, stupidityPoints = 0, totalPoints = 0 };
        }

        float avgDoom = spins.Average(s => s.doom);
        int questionLength = spins.Sum(s => s.question?.Length ?? 0);
        
        // Stars based on inverse doom (less doom = more stars)
        int stars = Mathf.RoundToInt((100 - avgDoom) / 20);
        
        // Regret level is doom percentage
        int regretLevel = Mathf.RoundToInt(avgDoom);
        
        // Creativity based on question variety and length
        int creativityPoints = Mathf.Min(100, Mathf.RoundToInt(questionLength / (float)spins.Count * 2));
        
        // Stupidity based on doom acceptance
        int stupidityPoints = Mathf.RoundToInt(avgDoom * 0.8f + UnityEngine.Random.Range(0f, 20f));
        
        // Total points with mode multiplier
        float modeMultiplier = mode switch
        {
            GameModeMigrated.Chaos => 2f,
            GameModeMigrated.Fate => 1.5f,
            GameModeMigrated.Rapid => 1.2f,
            _ => 1f
        };
        
        int totalPoints = Mathf.RoundToInt((creativityPoints + (100 - regretLevel) + stars * 20) * modeMultiplier);
        
        return new FinalScoreMigrated
        {
            stars = Mathf.Clamp(stars, 0, 5),
            regretLevel = regretLevel,
            creativityPoints = creativityPoints,
            stupidityPoints = stupidityPoints,
            totalPoints = totalPoints
        };
    }

    public static string GetAIAnswer(int doom, GameModeMigrated mode)
    {
        string category;
        
        if (mode == GameModeMigrated.Chaos)
        {
            category = "chaotic";
        }
        else if (doom < 30)
        {
            category = "positive";
        }
        else if (doom > 70)
        {
            category = "negative";
        }
        else
        {
            category = UnityEngine.Random.Range(0, 2) == 0 ? "cryptic" : "chaotic";
        }
        
        if (AI_ANSWER_TEMPLATES.TryGetValue(category, out string[] templates))
        {
            return templates[UnityEngine.Random.Range(0, templates.Length)];
        }
        
        return "The wheel is silent...";
    }

    public static PersonalityDataMigrated DetectPersonality(string username)
    {
        if (string.IsNullOrEmpty(username)) return null;
        
        string normalized = new string(username.ToLower().Where(char.IsLetterOrDigit).ToArray());
        
        if (KNOWN_PERSONALITIES.TryGetValue(normalized, out PersonalityDataMigrated personality))
        {
            return personality;
        }
        
        // Check for partial matches
        foreach (var kvp in KNOWN_PERSONALITIES)
        {
            if (normalized.Contains(kvp.Key) || kvp.Key.Contains(normalized))
            {
                return kvp.Value;
            }
        }
        
        return null;
    }
}
