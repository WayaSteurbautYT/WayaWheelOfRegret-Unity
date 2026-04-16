using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

// Comprehensive test to verify Cursor Unity integration is working properly
// This script creates and tests Unity components to validate the setup

public class CursorUnityTest : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] private bool runTestsOnStart = true;
    [SerializeField] private bool createTestObjects = true;
    [SerializeField] private bool verboseLogging = true;

    [Header("Test Results")]
    [SerializeField] private List<string> passedTests = new List<string>();
    [SerializeField] private List<string> failedTests = new List<string>();

    private void Start()
    {
        if (runTestsOnStart)
        {
            RunAllTests();
        }
    }

    [ContextMenu("Run All Cursor Unity Tests")]
    public void RunAllTests()
    {
        Debug.Log("=== CURSOR UNITY INTEGRATION TEST SUITE ===");
        Debug.Log("Testing migration from Next.js to Unity...");
        
        passedTests.Clear();
        failedTests.Clear();

        // Test 1: Basic Unity functionality
        TestUnityBasics();

        // Test 2: Script compilation
        TestScriptCompilation();

        // Test 3: Component creation
        TestComponentCreation();

        // Test 4: Migrated functionality
        TestMigratedFunctionality();

        // Test 5: File system operations
        TestFileSystemOperations();

        // Test 6: Unity-specific features
        TestUnitySpecificFeatures();

        // Test 7: Integration test
        TestFullIntegration();

        // Report results
        ReportTestResults();
    }

    private void TestUnityBasics()
    {
        LogTest("Testing Unity Basics...");

        try
        {
            // Test GameObject creation
            GameObject testObj = new GameObject("CursorTestObject");
            if (testObj == null)
            {
                throw new System.Exception("Failed to create GameObject");
            }

            // Test component addition
            var testComponent = testObj.AddComponent<CursorUnityTest>();
            if (testComponent == null)
            {
                throw new Exception("Failed to add component");
            }

            // Test transform operations
            testObj.transform.position = Vector3.one;
            if (testObj.transform.position != Vector3.one)
            {
                throw new Exception("Transform operation failed");
            }

            // Cleanup
            DestroyImmediate(testObj);

            PassTest("Unity Basics");
        }
        catch (System.Exception e)
        {
            FailTest("Unity Basics", e.Message);
        }
    }

    private void TestScriptCompilation()
    {
        LogTest("Testing Script Compilation...");

        try
        {
            // Test that migrated scripts compile and can be instantiated
            var gameStateTest = new GameObject().AddComponent<MigrationTest>();
            if (gameStateTest == null)
            {
                throw new Exception("MigrationTest component failed to compile");
            }

            // Test GameStateMigrated static class
            int segmentCount = GameStateMigrated.WHEEL_SEGMENTS.Count;
            if (segmentCount != 12)
            {
                throw new Exception($"Expected 12 segments, got {segmentCount}");
            }

            // Test ID generation
            string testId = GameStateMigrated.GenerateId();
            if (string.IsNullOrEmpty(testId) || testId.Length != 8)
            {
                throw new Exception("ID generation failed");
            }

            PassTest("Script Compilation");
        }
        catch (System.Exception e)
        {
            FailTest("Script Compilation", e.Message);
        }
    }

    private void TestComponentCreation()
    {
        LogTest("Testing Component Creation...");

        try
        {
            if (createTestObjects)
            {
                // Create GameManagerMigrated
                GameObject gameManagerObj = new GameObject("GameManagerMigrated_Test");
                var gameManager = gameManagerObj.AddComponent<GameManagerMigrated>();
                if (gameManager == null)
                {
                    throw new Exception("Failed to create GameManagerMigrated");
                }

                // Create WheelOfRegretMigrated
                GameObject wheelObj = new GameObject("WheelOfRegretMigrated_Test");
                var wheel = wheelObj.AddComponent<WheelOfRegretMigrated>();
                if (wheel == null)
                {
                    throw new Exception("Failed to create WheelOfRegretMigrated");
                }

                // Create MigrationTest
                GameObject testObj = new GameObject("MigrationTest_Test");
                var testComponent = testObj.AddComponent<MigrationTest>();
                if (testComponent == null)
                {
                    throw new Exception("Failed to create MigrationTest");
                }

                // Cleanup in play mode
                if (Application.isPlaying)
                {
                    Destroy(gameManagerObj);
                    Destroy(wheelObj);
                    Destroy(testObj);
                }
                else
                {
                    DestroyImmediate(gameManagerObj);
                    DestroyImmediate(wheelObj);
                    DestroyImmediate(testObj);
                }
            }

            PassTest("Component Creation");
        }
        catch (System.Exception e)
        {
            FailTest("Component Creation", e.Message);
        }
    }

    private void TestMigratedFunctionality()
    {
        LogTest("Testing Migrated Functionality...");

        try
        {
            // Test personality detection
            var personality = GameStateMigrated.DetectPersonality("pewdiepie");
            if (personality == null || personality.name != "PewDiePie")
            {
                throw new Exception("Personality detection failed");
            }

            // Test AI answers
            string answer = GameStateMigrated.GetAIAnswer(50, GameModeMigrated.Classic);
            if (string.IsNullOrEmpty(answer))
            {
                throw new Exception("AI answer generation failed");
            }

            // Test score calculation
            var testSpins = new List<SpinResultMigrated>
            {
                new SpinResultMigrated { id = "1", question = "Test", answer = "Answer", doom = 50, timestamp = System.DateTime.Now.Ticks, spinIndex = 0 }
            };
            var score = GameStateMigrated.CalculateFinalScore(testSpins, GameModeMigrated.Classic);
            if (score == null || score.totalPoints < 0)
            {
                throw new Exception("Score calculation failed");
            }

            // Test game modes
            var gameMode = GameStateMigrated.GAME_MODES[GameModeMigrated.Chaos];
            if (gameMode == null || gameMode.maxSpins != 6)
            {
                throw new Exception("Game mode data incorrect");
            }

            PassTest("Migrated Functionality");
        }
        catch (System.Exception e)
        {
            FailTest("Migrated Functionality", e.Message);
        }
    }

    private void TestFileSystemOperations()
    {
        LogTest("Testing File System Operations...");

        try
        {
            // Test persistent data path
            string persistentPath = Application.persistentDataPath;
            if (string.IsNullOrEmpty(persistentPath))
            {
                throw new Exception("Persistent data path is null");
            }

            // Test file creation
            string testFilePath = Path.Combine(persistentPath, "cursor_test.txt");
            File.WriteAllText(testFilePath, "Cursor Unity Integration Test");
            
            if (!File.Exists(testFilePath))
            {
                throw new Exception("Failed to create test file");
            }

            // Test file reading
            string content = File.ReadAllText(testFilePath);
            if (content != "Cursor Unity Integration Test")
            {
                throw new Exception("File content mismatch");
            }

            // Cleanup
            File.Delete(testFilePath);

            PassTest("File System Operations");
        }
        catch (System.Exception e)
        {
            FailTest("File System Operations", e.Message);
        }
    }

    private void TestUnitySpecificFeatures()
    {
        LogTest("Testing Unity-Specific Features...");

        try
        {
            // Test Scene management
            int currentSceneCount = SceneManager.sceneCount;
            if (currentSceneCount <= 0)
            {
                throw new Exception("No scenes loaded");
            }

            // Test Application properties
            string appName = Application.productName;
            if (string.IsNullOrEmpty(appName))
            {
                throw new Exception("Application name is null");
            }

            // Test Input system (if available)
            if (Input.anyKey)
            {
                // Any key is pressed - this is just testing the Input class works
            }

            // Test Time system
            float deltaTime = Time.deltaTime;
            if (deltaTime < 0)
            {
                throw new Exception("Invalid delta time");
            }

            PassTest("Unity-Specific Features");
        }
        catch (System.Exception e)
        {
            FailTest("Unity-Specific Features", e.Message);
        }
    }

    private void TestFullIntegration()
    {
        LogTest("Testing Full Integration...");

        try
        {
            // Create a complete test scenario
            GameObject testGameObj = new GameObject("FullIntegrationTest");
            
            // Add GameManager
            var gameManager = testGameObj.AddComponent<GameManagerMigrated>();
            
            // Test starting a game
            gameManager.StartGame(GameModeMigrated.Classic, "CursorTestUser");
            
            if (gameManager.currentSession == null)
            {
                throw new Exception("Failed to start game session");
            }

            // Test adding a spin
            gameManager.AddSpin("Should I use Cursor for Unity?", "The cosmic forces align...", 25);
            
            if (gameManager.currentSession.spins.Count != 1)
            {
                throw new Exception("Failed to add spin to session");
            }

            // Test saving
            gameManager.SaveGameData();

            // Test loading
            gameManager.LoadGameData();

            // Cleanup
            if (Application.isPlaying)
            {
                Destroy(testGameObj);
            }
            else
            {
                DestroyImmediate(testGameObj);
            }

            PassTest("Full Integration");
        }
        catch (System.Exception e)
        {
            FailTest("Full Integration", e.Message);
        }
    }

    private void PassTest(string testName)
    {
        passedTests.Add(testName);
        if (verboseLogging)
        {
            Debug.Log($"✅ {testName} - PASSED");
        }
    }

    private void FailTest(string testName, string errorMessage)
    {
        failedTests.Add($"{testName}: {errorMessage}");
        Debug.LogError($"❌ {testName} - FAILED: {errorMessage}");
    }

    private void LogTest(string testName)
    {
        if (verboseLogging)
        {
            Debug.Log($"🧪 {testName}");
        }
    }

    private void ReportTestResults()
    {
        Debug.Log("\n=== CURSOR UNITY INTEGRATION TEST RESULTS ===");
        Debug.Log($"Total Tests: {passedTests.Count + failedTests.Count}");
        Debug.Log($"Passed: {passedTests.Count}");
        Debug.Log($"Failed: {failedTests.Count}");

        if (passedTests.Count > 0)
        {
            Debug.Log("\n✅ PASSED TESTS:");
            foreach (string test in passedTests)
            {
                Debug.Log($"  • {test}");
            }
        }

        if (failedTests.Count > 0)
        {
            Debug.LogError("\n❌ FAILED TESTS:");
            foreach (string test in failedTests)
            {
                Debug.LogError($"  • {test}");
            }
        }

        bool allPassed = failedTests.Count == 0;
        
        if (allPassed)
        {
            Debug.Log("\n🎉 ALL TESTS PASSED!");
            Debug.Log("✅ Cursor Unity Integration is working perfectly!");
            Debug.Log("✅ Next.js to Unity migration was successful!");
            Debug.Log("✅ Your Cursor Unity setup is ready for development!");
        }
        else
        {
            Debug.LogError("\n⚠️ SOME TESTS FAILED!");
            Debug.LogError("❌ Check the failed tests above for issues.");
            Debug.LogError("❌ Your Cursor Unity setup may need configuration.");
        }

        Debug.Log("=== END TEST RESULTS ===");

        // Create test report file
        CreateTestReport();
    }

    private void CreateTestReport()
    {
        try
        {
            string reportPath = Path.Combine(Application.persistentDataPath, "cursor_unity_test_report.txt");
            
            using (StreamWriter writer = new StreamWriter(reportPath))
            {
                writer.WriteLine("CURSOR UNITY INTEGRATION TEST REPORT");
                writer.WriteLine($"Generated: {System.DateTime.Now}");
                writer.WriteLine($"Unity Version: {Application.unityVersion}");
                writer.WriteLine($"Platform: {Application.platform}");
                writer.WriteLine();
                
                writer.WriteLine($"Total Tests: {passedTests.Count + failedTests.Count}");
                writer.WriteLine($"Passed: {passedTests.Count}");
                writer.WriteLine($"Failed: {failedTests.Count}");
                writer.WriteLine();

                if (passedTests.Count > 0)
                {
                    writer.WriteLine("PASSED TESTS:");
                    foreach (string test in passedTests)
                    {
                        writer.WriteLine($"  ✓ {test}");
                    }
                    writer.WriteLine();
                }

                if (failedTests.Count > 0)
                {
                    writer.WriteLine("FAILED TESTS:");
                    foreach (string test in failedTests)
                    {
                        writer.WriteLine($"  ✗ {test}");
                    }
                    writer.WriteLine();
                }

                bool allPassed = failedTests.Count == 0;
                writer.WriteLine($"OVERALL RESULT: {(allPassed ? "SUCCESS" : "FAILURE")}");
                
                if (allPassed)
                {
                    writer.WriteLine();
                    writer.WriteLine("🎉 Cursor Unity Integration is working correctly!");
                    writer.WriteLine("🚀 Your Next.js to Unity migration was successful!");
                }
            }

            Debug.Log($"📄 Test report saved to: {reportPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create test report: {e.Message}");
        }
    }

    // Individual test methods for debugging
    [ContextMenu("Test Unity Basics Only")]
    public void TestUnityBasicsOnly()
    {
        passedTests.Clear();
        failedTests.Clear();
        TestUnityBasics();
        ReportTestResults();
    }

    [ContextMenu("Test Migrated Scripts Only")]
    public void TestMigratedScriptsOnly()
    {
        passedTests.Clear();
        failedTests.Clear();
        TestScriptCompilation();
        TestMigratedFunctionality();
        ReportTestResults();
    }

    [ContextMenu("Test File System Only")]
    public void TestFileSystemOnly()
    {
        passedTests.Clear();
        failedTests.Clear();
        TestFileSystemOperations();
        ReportTestResults();
    }
}
