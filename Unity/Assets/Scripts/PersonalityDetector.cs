using System;
using System.Collections.Generic;
using System.Linq;

public static class PersonalityDetector
{
    // Known personalities with their traits and ending types
    private static readonly Dictionary<string, PersonalityData> personalities = 
        new Dictionary<string, PersonalityData>(StringComparer.OrdinalIgnoreCase)
    {
        {"pewdiepie", new PersonalityData { name = "PewDiePie", trait = "Swedish chaos energy", endingType = EndingType.Hammer }},
        {"mrbeast", new PersonalityData { name = "MrBeast", trait = "Philanthropic madness", endingType = EndingType.Pokeball }},
        {"markiplier", new PersonalityData { name = "Markiplier", trait = "Dramatic screaming", endingType = EndingType.SCP }},
        {"jacksepticeye", new PersonalityData { name = "Jacksepticeye", trait = "Irish luck (or lack thereof)", endingType = EndingType.Hammer }},
        {"dream", new PersonalityData { name = "Dream", trait = "Speedrun destiny", endingType = EndingType.Glitch }},
        {"tommyinnit", new PersonalityData { name = "TommyInnit", trait = "Chaotic child energy", endingType = EndingType.Hammer }},
        {"technoblade", new PersonalityData { name = "Technoblade", trait = "Never dies... or does he?", endingType = EndingType.SCP }},
        {"ninja", new PersonalityData { name = "Ninja", trait = "Gamer rage mode", endingType = EndingType.Glitch }},
        {"pokimane", new PersonalityData { name = "Pokimane", trait = "Chat decides your fate", endingType = EndingType.Pokeball }},
        {"xqc", new PersonalityData { name = "xQc", trait = "Incomprehensible speed", endingType = EndingType.Glitch }},
        {"ludwig", new PersonalityData { name = "Ludwig", trait = "Subathon suffering", endingType = EndingType.Hammer }},
        {"valkyrae", new PersonalityData { name = "Valkyrae", trait = "Among Us paranoia", endingType = EndingType.SCP }},
        {"corpse", new PersonalityData { name = "Corpse Husband", trait = "Deep voice doom", endingType = EndingType.SCP }},
        {"sykkuno", new PersonalityData { name = "Sykkuno", trait = "Suspiciously wholesome", endingType = EndingType.Pokeball }},
        {"waya", new PersonalityData { name = "Waya", trait = "Vibe coding master", endingType = EndingType.Hammer }},
        {"wayacreate", new PersonalityData { name = "WayaCreate", trait = "The creator themselves", endingType = EndingType.Hammer }}
    };

    public static PersonalityData DetectPersonality(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        // Normalize username: lowercase, remove special characters and spaces
        string normalized = new string(username
            .ToLowerInvariant()
            .Where(c => char.IsLetterOrDigit(c))
            .ToArray());

        // Check for exact matches first
        if (personalities.TryGetValue(normalized, out PersonalityData exactMatch))
        {
            return exactMatch;
        }

        // Check for partial matches (contains)
        foreach (var kvp in personalities)
        {
            if (normalized.Contains(kvp.Key) || kvp.Key.Contains(normalized))
            {
                return kvp.Value;
            }
        }

        // Check for common variations
        string[] variations = {
            normalized.Replace("create", ""),
            normalized.Replace("yt", ""),
            normalized.Replace("tv", ""),
            normalized.Replace("gaming", ""),
            normalized.Replace("games", "")
        };

        foreach (string variation in variations)
        {
            if (string.IsNullOrWhiteSpace(variation)) continue;
            
            if (personalities.TryGetValue(variation, out PersonalityData variationMatch))
            {
                return variationMatch;
            }
        }

        return null; // No personality detected
    }

    // Get all available personalities (for debugging/testing)
    public static Dictionary<string, PersonalityData> GetAllPersonalities()
    {
        return new Dictionary<string, PersonalityData>(personalities);
    }
}

[Serializable]
public class PersonalityData
{
    public string name;
    public string trait;
    public EndingType endingType;
}
