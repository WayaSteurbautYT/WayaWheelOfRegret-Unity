using System;
using System.Collections.Generic;
using System.Linq;

public enum GameMode { Classic, Chaos, Fate, Rapid }

[Serializable] public class SpinResult { public string id; public string question; public string answer; public int doom; public long timestamp; public int spinIndex; }

[Serializable] public class FinalScore { public int stars; public int regretLevel; public int creativityPoints; public int stupidityPoints; public int totalPoints; }

[Serializable] public class GameSession { public string id; public GameMode mode; public string username; public string detectedPersonality; public List<SpinResult> spins; public int maxSpins; public bool isComplete; public FinalScore finalScore; }

[Serializable] public class WheelHistoryEntry { public string sessionId; public GameMode mode; public string username; public long date; public List<SpinResult> spins; public FinalScore finalScore; }

[Serializable] public class WheelSegment { public string text; public string color; public bool isRed; public int doom; }

[Serializable] public class PersonalityData { public string name; public string trait; public string ending; }

[Serializable] public class GameModeData { public string name; public string description; public int maxSpins; public string icon; }

public static class GameState
{
    public static readonly List<WheelSegment> WHEEL_SEGMENTS = new List<WheelSegment>
    {
        new WheelSegment { text = "DO IT", color = "#FF0000", isRed = true, doom = 30 },
        new WheelSegment { text = "REGRET IT", color = "#1A1A1F", isRed = false, doom = 80 },
        new WheelSegment { text = "CHAOS", color = "#FF0000", isRed = true, doom = 100 },
        new WheelSegment { text = "VIBES ONLY", color = "#1A1A1F", isRed = false, doom = 20 },
        new WheelSegment { text = "ASK AGAIN", color = "#FF0000", isRed = true, doom = 50 },
        new WheelSegment { text = "ABSOLUTELY", color = "#1A1A1F", isRed = false, doom = 10 },
        new WheelSegment { text = "NEVER EVER", color = "#FF0000", isRed = true, doom = 90 },
        new WheelSegment { text = "COSMIC YES", color = "#1A1A1F", isRed = false, doom = 15 },
        new WheelSegment { text = "RUN AWAY", color = "#FF0000", isRed = true, doom = 85 },
        new WheelSegment { text = "EMBRACE IT", color = "#1A1A1F", isRed = false, doom = 40 },
        new WheelSegment { text = "DOOM AWAITS", color = "#FF0000", isRed = true, doom = 95 },
        new WheelSegment { text = "MAYBE...", color = "#1A1A1F", isRed = false, doom = 55 }
    };

    public static readonly Dictionary<string, string[]> AI_ANSWERS = new Dictionary<string, string[]>
    {
        { "positive", new string[] { "The cosmic forces align in your favor...", "The wheel has spoken: fortune smiles...", "Against all odds, the universe says YES.", "The stars whisper approval...", "Proceed, mortal..." } },
        { "negative", new string[] { "The void laughs at your question. NO.", "The wheel has deemed this path unwise.", "Your fate is sealed with rejection.", "The cosmic balance requires disappointment.", "NAH. The wheel doesn't need to think." } },
        { "chaotic", new string[] { "DO IT BUT REGRET IT IMMEDIATELY.", "Do the opposite of what you think is right.", "The answer exists in quantum state...", "Your question angered the ancient forces.", "The wheel grants permission but revokes happiness." } },
        { "cryptic", new string[] { "Ask yourself: what would your past self think?", "The answer lies in the 7th dimension.", "When the moon is full and stars align...", "The prophecy speaks of one who asks this...", "Your ancestors are watching..." } }
    };

    public static readonly Dictionary<string, PersonalityData> PERSONALITIES = new Dictionary<string, PersonalityData>
    {
        { "pewdiepie", new PersonalityData { name = "PewDiePie", trait = "Swedish chaos energy", ending = "hammer" } },
        { "mrbeast", new PersonalityData { name = "MrBeast", trait = "Philanthropic madness", ending = "pokeball" } },
        { "markiplier", new PersonalityData { name = "Markiplier", trait = "Dramatic screaming", ending = "scp" } },
        { "jacksepticeye", new PersonalityData { name = "Jacksepticeye", trait = "Irish luck", ending = "hammer" } },
        { "dream", new PersonalityData { name = "Dream", trait = "Speedrun destiny", ending = "glitch" } },
        { "tommyinnit", new PersonalityData { name = "TommyInnit", trait = "Chaotic child energy", ending = "hammer" } },
        { "technoblade", new PersonalityData { name = "Technoblade", trait = "Never dies...", ending = "scp" } },
        { "ninja", new PersonalityData { name = "Ninja", trait = "Gamer rage mode", ending = "glitch" } },
        { "pokimane", new PersonalityData { name = "Pokimane", trait = "Chat decides your fate", ending = "pokeball" } },
        { "xqc", new PersonalityData { name = "xQc", trait = "Incomprehensible speed", ending = "glitch" } },
        { "ludwig", new PersonalityData { name = "Ludwig", trait = "Subathon suffering", ending = "hammer" } },
        { "valkyrae", new PersonalityData { name = "Valkyrae", trait = "Among Us paranoia", ending = "scp" } },
        { "corpse", new PersonalityData { name = "Corpse Husband", trait = "Deep voice doom", ending = "scp" } },
        { "sykkuno", new PersonalityData { name = "Sykkuno", trait = "Suspiciously wholesome", ending = "pokeball" } },
        { "waya", new PersonalityData { name = "Waya", trait = "Vibe coding master", ending = "hammer" } },
        { "wayacreate", new PersonalityData { name = "WayaCreate", trait = "The creator", ending = "hammer" } }
    };

    public static readonly Dictionary<GameMode, GameModeData> GAME_MODES = new Dictionary<GameMode, GameModeData>
    {
        { GameMode.Classic, new GameModeData { name = "Classic", description = "One question, one fate", maxSpins = 1, icon = "🎯" } },
        { GameMode.Chaos, new GameModeData { name = "Chaos Chain", description = "6 spins of escalating doom", maxSpins = 6, icon = "🔥" } },
        { GameMode.Fate, new GameModeData { name = "Wheel of Fate", description = "The wheel knows who you are", maxSpins = 6, icon = "⚡" } },
        { GameMode.Rapid, new GameModeData { name = "Rapid Fire", description = "Auto-spin madness", maxSpins = 6, icon = "💨" } }
    };

    public static string GenerateId() => Guid.NewGuid().ToString("N")[..8];

    public static FinalScore CalculateFinalScore(List<SpinResult> spins, GameMode mode)
    {
        if (spins == null || spins.Count == 0) return new FinalScore { stars = 0, regretLevel = 0, creativityPoints = 0, stupidityPoints = 0, totalPoints = 0 };

        float avgDoom = spins.Average(s => s.doom);
        int questionLength = spins.Sum(s => s.question?.Length ?? 0);
        int stars = Mathf.Clamp(Mathf.RoundToInt((100 - avgDoom) / 20), 0, 5);
        int regretLevel = Mathf.RoundToInt(avgDoom);
        int creativity = Mathf.Min(100, Mathf.RoundToInt(questionLength / (float)spins.Count * 2));
        int stupidity = Mathf.RoundToInt(avgDoom * 0.8f + UnityEngine.Random.Range(0f, 20f));
        
        float multiplier = mode switch { GameMode.Chaos => 2f, GameMode.Fate => 1.5f, GameMode.Rapid => 1.2f, _ => 1f };
        int total = Mathf.RoundToInt((creativity + (100 - regretLevel) + stars * 20) * multiplier);
        
        return new FinalScore { stars = stars, regretLevel = regretLevel, creativityPoints = creativity, stupidityPoints = stupidity, totalPoints = total };
    }

    public static string GetAIAnswer(int doom, GameMode mode)
    {
        string category = mode == GameMode.Chaos ? "chaotic" : doom < 30 ? "positive" : doom > 70 ? "negative" : UnityEngine.Random.Range(0, 2) == 0 ? "cryptic" : "chaotic";
        if (AI_ANSWERS.TryGetValue(category, out string[] templates)) return templates[UnityEngine.Random.Range(0, templates.Length)];
        return "The wheel is silent...";
    }
}

public static class PersonalityDetector
{
    public static PersonalityData DetectPersonality(string username)
    {
        if (string.IsNullOrEmpty(username)) return null;
        string normalized = new string(username.ToLower().Where(char.IsLetterOrDigit).ToArray());
        if (GameState.PERSONALITIES.TryGetValue(normalized, out PersonalityData personality)) return personality;
        foreach (var kvp in GameState.PERSONALITIES) if (normalized.Contains(kvp.Key) || kvp.Key.Contains(normalized)) return kvp.Value;
        return null;
    }
}
