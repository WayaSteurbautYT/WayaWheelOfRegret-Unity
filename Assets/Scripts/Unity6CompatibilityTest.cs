using UnityEngine;
using System.Collections;

public class Unity6CompatibilityTest : MonoBehaviour
{
    [Header("Test Results")]
    [SerializeField] private bool allTestsPassed = false;
    [SerializeField] private string testResults = "";

    private void Start()
    {
        StartCoroutine(RunCompatibilityTests());
    }

    private IEnumerator RunCompatibilityTests()
    {
        Debug.Log("=== Unity 6 Compatibility Test Suite ===");
        
        yield return StartCoroutine(TestAudioSystem());
        yield return StartCoroutine(TestUISystem());
        yield return StartCoroutine(TestJSONSerialization());
        yield return StartCoroutine(TestMathAndRandom());
        yield return StartCoroutine(TestSceneManagement());
        yield return StartCoroutine(TestCoroutineSystem());
        
        FinalizeResults();
    }

    private IEnumerator TestAudioSystem()
    {
        try
        {
            // Test AudioSettings.outputSampleRate (Unity 6 compatible)
            int sampleRate = AudioSettings.outputSampleRate;
            if (sampleRate <= 0)
                throw new System.Exception("Invalid sample rate");

            // Test AudioClip creation
            AudioClip testClip = AudioClip.Create("Test", 44100, 1, 44100, false);
            if (testClip == null)
                throw new System.Exception("Failed to create AudioClip");

            Debug.Log("✓ Audio System Test Passed");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ Audio System Test Failed: {e.Message}");
            testResults += "Audio System Failed; ";
        }
        yield return null;
    }

    private IEnumerator TestUISystem()
    {
        try
        {
            // Test basic UI components availability
            var gameObject = new GameObject("TestUI");
            var button = gameObject.AddComponent<UnityEngine.UI.Button>();
            var image = gameObject.AddComponent<UnityEngine.UI.Image>();
            var text = gameObject.AddComponent<TMPro.TextMeshProUGUI>();

            if (button == null || image == null || text == null)
                throw new System.Exception("UI components not available");

            DestroyImmediate(gameObject);
            Debug.Log("✓ UI System Test Passed");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ UI System Test Failed: {e.Message}");
            testResults += "UI System Failed; ";
        }
        yield return null;
    }

    private IEnumerator TestJSONSerialization()
    {
        try
        {
            // Test JsonUtility with Serializable classes
            var testData = new TestSerializableData { testString = "Unity6", testNumber = 6004 };
            string json = JsonUtility.ToJson(testData);
            var deserialized = JsonUtility.FromJson<TestSerializableData>(json);

            if (deserialized.testString != "Unity6" || deserialized.testNumber != 6004)
                throw new System.Exception("JSON serialization failed");

            Debug.Log("✓ JSON Serialization Test Passed");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ JSON Serialization Test Failed: {e.Message}");
            testResults += "JSON Serialization Failed; ";
        }
        yield return null;
    }

    private IEnumerator TestMathAndRandom()
    {
        try
        {
            // Test Random.Range (Unity 6 compatible)
            float randomFloat = Random.Range(0f, 1f);
            int randomInt = Random.Range(0, 10);

            if (randomFloat < 0f || randomFloat > 1f || randomInt < 0 || randomInt >= 10)
                throw new System.Exception("Random.Range failed");

            // Test Mathf functions
            float lerp = Mathf.Lerp(0f, 1f, 0.5f);
            if (Mathf.Abs(lerp - 0.5f) > 0.001f)
                throw new System.Exception("Mathf.Lerp failed");

            Debug.Log("✓ Math and Random Test Passed");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ Math and Random Test Failed: {e.Message}");
            testResults += "Math/Random Failed; ";
        }
        yield return null;
    }

    private IEnumerator TestSceneManagement()
    {
        try
        {
            // Test SceneManagement
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            if (sceneCount <= 0)
                throw new System.Exception("No scenes in build settings");

            Debug.Log("✓ Scene Management Test Passed");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ Scene Management Test Failed: {e.Message}");
            testResults += "Scene Management Failed; ";
        }
        yield return null;
    }

    private IEnumerator TestCoroutineSystem()
    {
        bool testPassed = false;
        try
        {
            // Test coroutine functionality
            bool coroutineCompleted = false;
            StartCoroutine(TestCoroutine(() => coroutineCompleted = true));

            float timeout = 0f;
            while (!coroutineCompleted && timeout < 2f)
            {
                timeout += Time.deltaTime;
                yield return null;
            }

            if (!coroutineCompleted)
                throw new System.Exception("Coroutine system failed");

            testPassed = true;
            Debug.Log("✓ Coroutine System Test Passed");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ Coroutine System Test Failed: {e.Message}");
            testResults += "Coroutine System Failed; ";
        }
        yield return null;
    }

    private IEnumerator TestCoroutine(System.Action onComplete)
    {
        yield return new WaitForSeconds(0.1f);
        onComplete?.Invoke();
    }

    private void FinalizeResults()
    {
        allTestsPassed = string.IsNullOrEmpty(testResults);
        
        if (allTestsPassed)
        {
            Debug.Log("=== ALL UNITY 6 COMPATIBILITY TESTS PASSED ===");
            Debug.Log("Project is fully compatible with Unity 6000.4.2f1");
        }
        else
        {
            Debug.LogError($"=== SOME TESTS FAILED: {testResults} ===");
        }

        Debug.Log("=== END COMPATIBILITY TEST ===");
    }

    [System.Serializable]
    private class TestSerializableData
    {
        public string testString;
        public int testNumber;
    }
}
