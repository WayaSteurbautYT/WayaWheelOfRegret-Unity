using System;

public static class AIAnswerGenerator
{
    // Template categories for AI-generated answers
    private static readonly string[] positive = {
        "The cosmic forces align in your favor... this time.",
        "The wheel has spoken: fortune smiles upon your foolish endeavor.",
        "Against all odds, the universe says YES. Don't waste it.",
        "The stars whisper approval. But they're always watching...",
        "Proceed, mortal. The wheel finds your chaos... amusing."
    };

    private static readonly string[] negative = {
        "The void laughs at your question. The answer is NO.",
        "The wheel has deemed this path... unwise. Very unwise.",
        "Your fate is sealed with rejection. Accept it.",
        "The cosmic balance requires your disappointment today.",
        "NAH. The wheel doesn't even need to think about this one."
    };

    private static readonly string[] chaotic = {
        "The wheel spins into madness... DO IT BUT REGRET IT IMMEDIATELY.",
        "CHAOS DEMANDS: Do the opposite of what you think is right.",
        "The answer exists in a quantum state of YES and NO simultaneously.",
        "Your question has angered the ancient forces. Prepare for consequences.",
        "The wheel grants permission but revokes happiness."
    };

    private static readonly string[] cryptic = {
        "Ask yourself: what would your past self think? Now do the opposite.",
        "The answer lies within the 7th dimension. Good luck accessing that.",
        "When the moon is full and the stars align... still no.",
        "The prophecy speaks of one who asks this question. It doesn't end well.",
        "Your ancestors are watching. They're disappointed either way."
    };

    public static string GetAnswer(int doom, GameMode mode)
    {
        // If chaos mode, always use chaotic answers
        if (mode == GameMode.Chaos)
        {
            return chaotic[UnityEngine.Random.Range(0, chaotic.Length)];
        }

        // Determine answer category based on doom level
        if (doom < 30)
        {
            return positive[UnityEngine.Random.Range(0, positive.Length)];
        }
        else if (doom > 70)
        {
            return negative[UnityEngine.Random.Range(0, negative.Length)];
        }
        else
        {
            // 50% cryptic, 50% chaotic for middle ranges
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                return cryptic[UnityEngine.Random.Range(0, cryptic.Length)];
            }
            else
            {
                return chaotic[UnityEngine.Random.Range(0, chaotic.Length)];
            }
        }
    }

    // Get a random answer from any category (for testing/fallback)
    public static string GetRandomAnswer()
    {
        string[][] allCategories = { positive, negative, chaotic, cryptic };
        string[] selectedCategory = allCategories[UnityEngine.Random.Range(0, allCategories.Length)];
        return selectedCategory[UnityEngine.Random.Range(0, selectedCategory.Length)];
    }

    // Get answer based on specific wheel segment text
    public static string GetAnswerForSegment(string segmentText, int doom, GameMode mode)
    {
        // Some segments have special logic
        switch (segmentText.ToUpper())
        {
            case "DO IT":
            case "ABSOLUTELY":
                return doom < 50 ? GetAnswer(doom, mode) : "The wheel says YES, but your doom level suggests otherwise...";
                
            case "REGRET IT":
            case "NEVER EVER":
                return doom > 50 ? GetAnswer(doom, mode) : "The wheel says NO, but your doom level is surprisingly low...";
                
            case "CHAOS":
                return chaotic[UnityEngine.Random.Range(0, chaotic.Length)];
                
            case "MAYBE...":
                return cryptic[UnityEngine.Random.Range(0, cryptic.Length)];
                
            default:
                return GetAnswer(doom, mode);
        }
    }
}
