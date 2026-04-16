using System;
using UnityEngine;
using System.Collections.Generic;

public class TestSuite : MonoBehaviour
{
    [Header("Test Results")]
    [SerializeField] private List<string> passedTests = new List<string>();
    [SerializeField] private List<string> failedTests = new List<string>();

    private void Start()
    {
        Debug.Log("=== Starting Complete Unity Project Test Suite ===");
        RunAllTests();
    }

    [ContextMenu("Run All Tests")]
    public void RunAllTests()
    {
        passedTests.Clear();
        failedTests.Clear();

        TestGameState();
        TestPersonalityDetection();
        TestScoreCalculation();
        TestAIAnswers();
        TestGameModes();
        TestGameManager();
        TestSoundSystem();

        ReportResults();
    }

    private void TestGameState()
    {
        try
        {
            // Test wheel segments
            if (GameState.WHEEL_SEGMENTS.Count != 12)
                throw new System.Exception($"Expected 12 segments, got {GameState.WHEEL_SEGMENTS.Count}");

            // Test first segment
            var first = GameState.WHEEL_SEGMENTS[0];
            if (first.text != "DO IT" || first.doom != 30)
                throw new System.Exception($"First segment incorrect: {first.text}");

            // Test ID generation
            string id1 = GameState.GenerateId();
            string id2 = GameState.GenerateId();
            if (string.IsNullOrEmpty(id1) || id1 == id2)
                throw new System.Exception("ID generation failed");

            passedTests.Add("GameState");
        }
        catch (System.Exception e)
        {
            failedTests.Add($"GameState: {e.Message}");
        }
    }

    private void TestPersonalityDetection()
    {
        try
        {
            // Test known personality
            var pewdiepie = PersonalityDetector.DetectPersonality("pewdiepie");
            if (pewdiepie == null || pewdiepie.name != "PewDiePie")
                throw new System.Exception("PewDiePie detection failed");

            // Test case insensitive
            var mrbeast = PersonalityDetector.DetectPersonality("MRBEAST");
            if (mrbeast == null || mrbeast.name != "MrBeast")
                throw new System.Exception("Case insensitive failed");

            // Test unknown user
            var unknown = PersonalityDetector.DetectPersonality("randomuser");
            if (unknown != null)
                throw new System.Exception("Unknown user should return null");

            passedTests.Add("Personality Detection");
        }
        catch (System.Exception e)
        {
            failedTests.Add($"Personality Detection: {e.Message}");
        }
    }

    private void TestScoreCalculation()
    {
        try
        {
            var testSpins = new List<SpinResult>
            {
                new SpinResult { id = "1", question = "Test", answer = "Answer", doom = 50, timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(), spinIndex = 0 }
            };

            var score = GameState.CalculateFinalScore(testSpins, GameMode.Classic);
            if (score == null || score.totalPoints < 0 || score.stars < 0 || score.stars > 5)
                throw new System.Exception("Invalid score calculation");

            // Test chaos mode multiplier
            var chaosScore = GameState.CalculateFinalScore(testSpins, GameMode.Chaos);
            if (chaosScore.totalPoints <= score.totalPoints)
                throw new System.Exception("Chaos mode should have higher score");

            passedTests.Add("Score Calculation");
        }
        catch (System.Exception e)
        {
            failedTests.Add($"Score Calculation: {e.Message}");
        }
    }

    private void TestAIAnswers()
    {
        try
        {
            string positive = GameState.GetAIAnswer(20, GameMode.Classic);
            string negative = GameState.GetAIAnswer(80, GameMode.Classic);
            string chaos = GameState.GetAIAnswer(50, GameMode.Chaos);

            if (string.IsNullOrEmpty(positive) || string.IsNullOrEmpty(negative) || string.IsNullOrEmpty(chaos))
                throw new System.Exception("AI answers returned null/empty");

            passedTests.Add("AI Answers");
        }
        catch (System.Exception e)
        {
            failedTests.Add($"AI Answers: {e.Message}");
        }
    }

    private void TestGameModes()
    {
        try
        {
            var modes = GameState.GAME_MODES;
            if (modes.Count != 4)
                throw new System.Exception($"Expected 4 game modes, got {modes.Count}");

            // Test classic mode
            var classic = modes[GameMode.Classic];
            if (classic.maxSpins != 1 || classic.name != "Classic")
                throw new System.Exception("Classic mode data incorrect");

            // Test chaos mode
            var chaos = modes[GameMode.Chaos];
            if (chaos.maxSpins != 6 || chaos.name != "Chaos Chain")
                throw new System.Exception("Chaos mode data incorrect");

            passedTests.Add("Game Modes");
        }
        catch (System.Exception e)
        {
            failedTests.Add($"Game Modes: {e.Message}");
        }
    }

    private void TestGameManager()
    {
        try
        {
            if (GameManager.Instance == null)
                throw new System.Exception("GameManager instance not found");

            // Test starting a game
            GameManager.Instance.StartGame(GameMode.Classic, "TestUser");
            if (GameManager.Instance.currentSession == null)
                throw new System.Exception("Failed to start game session");

            // Test adding a spin
            GameManager.Instance.AddSpin("Test question", "Test answer", 25);
            if (GameManager.Instance.currentSession.spins.Count != 1)
                throw new System.Exception("Failed to add spin");

            passedTests.Add("GameManager");
        }
        catch (System.Exception e)
        {
            failedTests.Add($"GameManager: {e.Message}");
        }
    }

    private void TestSoundSystem()
    {
        try
        {
            if (SoundManager.Instance == null)
                throw new System.Exception("SoundManager instance not found");

            // Test sound methods (they won't actually play without audio source, but should not error)
            SoundManager.Instance.PlayClick();
            SoundManager.Instance.PlaySuccess();
            SoundManager.Instance.SetMusicEnabled(true);
            SoundManager.Instance.SetSfxEnabled(false);

            passedTests.Add("Sound System");
        }
        catch (System.Exception e)
        {
            failedTests.Add($"Sound System: {e.Message}");
        }
    }

    private void ReportResults()
    {
        Debug.Log("\n=== UNITY PROJECT TEST RESULTS ===");
        Debug.Log($"Total Tests: {passedTests.Count + failedTests.Count}");
        Debug.Log($"Passed: {passedTests.Count}");
        Debug.Log($"Failed: {failedTests.Count}");

        if (passedTests.Count > 0)
        {
            Debug.Log("\n=== PASSED TESTS ===");
            foreach (string test in passedTests)
            {
                Debug.Log($"  [PASS] {test}");
            }
        }

        if (failedTests.Count > 0)
        {
            Debug.LogError("\n=== FAILED TESTS ===");
            foreach (string test in failedTests)
            {
                Debug.LogError($"  [FAIL] {test}");
            }
        }

        bool allPassed = failedTests.Count == 0;
        
        if (allPassed)
        {
            Debug.Log("\n=== ALL TESTS PASSED! ===");
            Debug.Log("Unity project is ready to play!");
            Debug.Log("Game should work perfectly.");
        }
        else
        {
            Debug.LogError("\n=== SOME TESTS FAILED ===");
            Debug.LogError("Check the errors above and fix issues.");
        }

        Debug.Log("=== END TEST RESULTS ===");
    }
}
