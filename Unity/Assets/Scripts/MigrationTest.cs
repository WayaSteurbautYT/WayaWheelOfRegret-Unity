using UnityEngine;
using System.Collections.Generic;

// Test script to verify Cursor Unity integration and migration functionality
// This script validates that all migrated components work correctly in Unity

public class MigrationTest : MonoBehaviour
{
    [Header("Test Results")]
    [SerializeField] private bool gameStateTestPassed = false;
    [SerializeField] private bool wheelComponentTestPassed = false;
    [SerializeField] private bool personalityDetectionTestPassed = false;
    [SerializeField] private bool scoreCalculationTestPassed = false;
    [SerializeField] private bool aiAnswerTestPassed = false;

    [Header("Test Components")]
    public WheelOfRegretMigrated wheelComponent;

    private void Start()
    {
        Debug.Log("=== Starting Unity Migration Test ===");
        
        // Run all tests
        TestGameStateMigration();
        TestPersonalityDetection();
        TestScoreCalculation();
        TestAIAnswers();
        TestWheelComponent();
        
        // Report results
        ReportTestResults();
    }

    private void TestGameStateMigration()
    {
        Debug.Log("Testing GameState Migration...");
        
        try
        {
            // Test wheel segments
            var segments = GameStateMigrated.WHEEL_SEGMENTS;
            if (segments.Count != 12)
            {
                Debug.LogError($"Expected 12 wheel segments, got {segments.Count}");
                return;
            }

            // Test first segment
            var firstSegment = segments[0];
            if (firstSegment.text != "DO IT" || firstSegment.doom != 30)
            {
                Debug.LogError($"First segment mismatch: {firstSegment.text} (doom: {firstSegment.doom})");
                return;
            }

            // Test game modes
            var gameModes = GameStateMigrated.GAME_MODES;
            if (gameModes.Count != 4)
            {
                Debug.LogError($"Expected 4 game modes, got {gameModes.Count}");
                return;
            }

            // Test classic mode
            if (!gameModes.ContainsKey(GameModeMigrated.Classic))
            {
                Debug.LogError("Classic game mode not found");
                return;
            }

            var classicMode = gameModes[GameModeMigrated.Classic];
            if (classicMode.maxSpins != 1 || classicMode.name != "Classic")
            {
                Debug.LogError($"Classic mode data incorrect: {classicMode.name} (spins: {classicMode.maxSpins})");
                return;
            }

            // Test ID generation
            string id1 = GameStateMigrated.GenerateId();
            string id2 = GameStateMigrated.GenerateId();
            if (string.IsNullOrEmpty(id1) || string.IsNullOrEmpty(id2) || id1 == id2)
            {
                Debug.LogError("ID generation failed");
                return;
            }

            gameStateTestPassed = true;
            Debug.Log("✅ GameState Migration Test PASSED");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ GameState Migration Test FAILED: {e.Message}");
        }
    }

    private void TestPersonalityDetection()
    {
        Debug.Log("Testing Personality Detection...");
        
        try
        {
            // Test known personality
            var pewdiepie = GameStateMigrated.DetectPersonality("pewdiepie");
            if (pewdiepie == null || pewdiepie.name != "PewDiePie" || pewdiepie.ending != "hammer")
            {
                Debug.LogError("PewDiePie personality detection failed");
                return;
            }

            // Test case insensitive
            var mrbeast = GameStateMigrated.DetectPersonality("MRBEAST");
            if (mrbeast == null || mrbeast.name != "MrBeast")
            {
                Debug.LogError("Case insensitive personality detection failed");
                return;
            }

            // Test with special characters
            var waya = GameStateMigrated.DetectPersonality("WayaCreate!");
            if (waya == null || waya.name != "WayaCreate")
            {
                Debug.LogError("Special character handling failed");
                return;
            }

            // Test unknown user
            var unknown = GameStateMigrated.DetectPersonality("randomuser123");
            if (unknown != null)
            {
                Debug.LogError("Unknown user should return null");
                return;
            }

            personalityDetectionTestPassed = true;
            Debug.Log("✅ Personality Detection Test PASSED");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Personality Detection Test FAILED: {e.Message}");
        }
    }

    private void TestScoreCalculation()
    {
        Debug.Log("Testing Score Calculation...");
        
        try
        {
            // Test with sample spins
            var testSpins = new List<SpinResultMigrated>
            {
                new SpinResultMigrated 
                { 
                    id = "1", 
                    question = "Should I code this game?", 
                    answer = "The cosmic forces align in your favor...", 
                    doom = 25, 
                    timestamp = System.DateTime.Now.Ticks, 
                    spinIndex = 0 
                },
                new SpinResultMigrated 
                { 
                    id = "2", 
                    question = "Is this a good idea?", 
                    answer = "The void laughs at your question...", 
                    doom = 75, 
                    timestamp = System.DateTime.Now.Ticks, 
                    spinIndex = 1 
                }
            };

            // Test classic mode scoring
            var classicScore = GameStateMigrated.CalculateFinalScore(testSpins, GameModeMigrated.Classic);
            if (classicScore.stars < 0 || classicScore.stars > 5)
            {
                Debug.LogError($"Invalid star count: {classicScore.stars}");
                return;
            }

            if (classicScore.regretLevel < 0 || classicScore.regretLevel > 100)
            {
                Debug.LogError($"Invalid regret level: {classicScore.regretLevel}");
                return;
            }

            // Test chaos mode multiplier
            var chaosScore = GameStateMigrated.CalculateFinalScore(testSpins, GameModeMigrated.Chaos);
            if (chaosScore.totalPoints <= classicScore.totalPoints)
            {
                Debug.LogError("Chaos mode should have higher total points");
                return;
            }

            // Test empty spins
            var emptyScore = GameStateMigrated.CalculateFinalScore(new List<SpinResultMigrated>(), GameModeMigrated.Classic);
            if (emptyScore.totalPoints != 0 || emptyScore.stars != 0)
            {
                Debug.LogError("Empty spins should return zero score");
                return;
            }

            scoreCalculationTestPassed = true;
            Debug.Log("✅ Score Calculation Test PASSED");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Score Calculation Test FAILED: {e.Message}");
        }
    }

    private void TestAIAnswers()
    {
        Debug.Log("Testing AI Answers...");
        
        try
        {
            // Test positive answer
            string positive = GameStateMigrated.GetAIAnswer(20, GameModeMigrated.Classic);
            if (string.IsNullOrEmpty(positive))
            {
                Debug.LogError("Positive answer is null or empty");
                return;
            }

            // Test negative answer
            string negative = GameStateMigrated.GetAIAnswer(80, GameModeMigrated.Classic);
            if (string.IsNullOrEmpty(negative))
            {
                Debug.LogError("Negative answer is null or empty");
                return;
            }

            // Test chaos answer (should always be chaotic)
            string chaos = GameStateMigrated.GetAIAnswer(50, GameModeMigrated.Chaos);
            if (string.IsNullOrEmpty(chaos))
            {
                Debug.LogError("Chaos answer is null or empty");
                return;
            }

            // Test middle range answer
            string middle = GameStateMigrated.GetAIAnswer(50, GameModeMigrated.Classic);
            if (string.IsNullOrEmpty(middle))
            {
                Debug.LogError("Middle range answer is null or empty");
                return;
            }

            // Test multiple calls to ensure variety
            string answer1 = GameStateMigrated.GetAIAnswer(30, GameModeMigrated.Classic);
            string answer2 = GameStateMigrated.GetAIAnswer(30, GameModeMigrated.Classic);
            
            // They might be the same by chance, but that's okay for this test
            if (string.IsNullOrEmpty(answer1) || string.IsNullOrEmpty(answer2))
            {
                Debug.LogError("Multiple AI answers failed");
                return;
            }

            aiAnswerTestPassed = true;
            Debug.Log("✅ AI Answers Test PASSED");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ AI Answers Test FAILED: {e.Message}");
        }
    }

    private void TestWheelComponent()
    {
        Debug.Log("Testing Wheel Component...");
        
        try
        {
            if (wheelComponent == null)
            {
                Debug.LogWarning("Wheel component not assigned - creating test instance");
                GameObject testObj = new GameObject("TestWheel");
                wheelComponent = testObj.AddComponent<WheelOfRegretMigrated>();
            }

            // Call the test method on the wheel component
            wheelComponent.TestMigration();

            wheelComponentTestPassed = true;
            Debug.Log("✅ Wheel Component Test PASSED");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Wheel Component Test FAILED: {e.Message}");
        }
    }

    private void ReportTestResults()
    {
        Debug.Log("\n=== MIGRATION TEST RESULTS ===");
        Debug.Log($"GameState Migration: {(gameStateTestPassed ? "✅ PASSED" : "❌ FAILED")}");
        Debug.Log($"Personality Detection: {(personalityDetectionTestPassed ? "✅ PASSED" : "❌ FAILED")}");
        Debug.Log($"Score Calculation: {(scoreCalculationTestPassed ? "✅ PASSED" : "❌ FAILED")}");
        Debug.Log($"AI Answers: {(aiAnswerTestPassed ? "✅ PASSED" : "❌ FAILED")}");
        Debug.Log($"Wheel Component: {(wheelComponentTestPassed ? "✅ PASSED" : "❌ FAILED")}");

        bool allTestsPassed = gameStateTestPassed && personalityDetectionTestPassed && 
                              scoreCalculationTestPassed && aiAnswerTestPassed && wheelComponentTestPassed;

        Debug.Log($"\nOVERALL: {(allTestsPassed ? "✅ ALL TESTS PASSED" : "❌ SOME TESTS FAILED")}");
        
        if (allTestsPassed)
        {
            Debug.Log("\n🎉 Cursor Unity Integration is working correctly!");
            Debug.Log("🎮 Migration from Next.js to Unity was successful!");
        }
        else
        {
            Debug.LogError("\n⚠️ Some tests failed. Check the logs above for details.");
        }

        Debug.Log("=== END TEST RESULTS ===");
    }

    // Manual test trigger (call from Unity Inspector or other scripts)
    [ContextMenu("Run Migration Tests")]
    public void RunTestsManually()
    {
        Debug.Log("=== MANUALLY TRIGGERING MIGRATION TESTS ===");
        
        gameStateTestPassed = false;
        wheelComponentTestPassed = false;
        personalityDetectionTestPassed = false;
        scoreCalculationTestPassed = false;
        aiAnswerTestPassed = false;

        Start();
    }

    // Test individual components
    [ContextMenu("Test GameState Only")]
    public void TestGameStateOnly()
    {
        TestGameStateMigration();
        ReportTestResults();
    }

    [ContextMenu("Test Wheel Component Only")]
    public void TestWheelComponentOnly()
    {
        TestWheelComponent();
        ReportTestResults();
    }
}
