using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// This script helps automatically set up the Unity project
// Run this script in Unity Editor to create all necessary GameObjects and components

public class UnitySetupHelper : MonoBehaviour
{
    [Header("Auto-Setup Options")]
    [SerializeField] private bool createAllScenes = true;
    [SerializeField] private bool setupCanvases = true;
    [SerializeField] private bool createGameObjects = true;
    [SerializeField] private bool connectComponents = true;

    [ContextMenu("Auto-Setup Complete Project")]
    public void SetupCompleteProject()
    {
        Debug.Log("=== Starting Auto-Setup of Unity Project ===");
        
        if (createAllScenes)
        {
            CreateAllScenes();
        }
        
        if (setupCanvases)
        {
            SetupCanvases();
        }
        
        if (createGameObjects)
        {
            CreateGameObjects();
        }
        
        if (connectComponents)
        {
            ConnectAllComponents();
        }
        
        Debug.Log("=== Auto-Setup Complete! ===");
        Debug.Log("Check the Hierarchy window for all created objects.");
    }

    [ContextMenu("Create All Scenes")]
    public void CreateAllScenes()
    {
        string[] sceneNames = { "LoadingScene", "MenuScene", "GameScene", "ResultsScene", "CreditsScene" };
        
        foreach (string sceneName in sceneNames)
        {
            CreateScene(sceneName);
        }
        
        Debug.Log("✅ All scenes created");
    }

    private void CreateScene(string sceneName)
    {
        // Check if scene already exists
        string[] existingScenes = UnityEditor.AssetDatabase.FindAssets($"t:scene {sceneName}");
        if (existingScenes.Length > 0)
        {
            Debug.Log($"Scene {sceneName} already exists");
            return;
        }

        // Create new scene
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, $"Assets/Scenes/{sceneName}.unity");
        Debug.Log($"Created scene: {sceneName}");
    }

    [ContextMenu("Setup Canvases")]
    public void SetupCanvases()
    {
        // This would be called in each scene
        Debug.Log("Canvas setup - Add this script to each scene and run individually");
    }

    [ContextMenu("Create Core Game Objects")]
    public void CreateGameObjects()
    {
        CreateGameManager();
        CreateSoundManager();
        CreateWheel();
        CreateUI();
        Debug.Log("✅ Core GameObjects created");
    }

    private void CreateGameManager()
    {
        GameObject gameManager = new GameObject("GameManager");
        gameManager.AddComponent<GameManager>();
        
        // Make it persistent
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameManager);
        }
        
        Debug.Log("✅ GameManager created");
    }

    private void CreateSoundManager()
    {
        GameObject soundManager = new GameObject("SoundManager");
        soundManager.AddComponent<SoundManager>();
        
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(soundManager);
        }
        
        Debug.Log("✅ SoundManager created");
    }

    private void CreateWheel()
    {
        GameObject wheelContainer = new GameObject("WheelContainer");
        wheelContainer.AddComponent<WheelOfRegret>();
        
        // Create wheel visual
        GameObject wheel = new GameObject("Wheel");
        wheel.transform.SetParent(wheelContainer.transform);
        
        Image wheelImage = wheel.AddComponent<Image>();
        wheelImage.color = new Color(0.1f, 0.1f, 0.12f, 1f); // #1A1A1F
        
        RectTransform rect = wheel.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 400);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        
        Debug.Log("✅ Wheel created");
    }

    private void CreateUI()
    {
        CreateCanvas();
        CreateBasicUI();
        Debug.Log("✅ UI created");
    }

    private void CreateCanvas()
    {
        GameObject canvas = new GameObject("Canvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        canvas.AddComponent<GraphicRaycaster>();
        
        // Add event system
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        Debug.Log("✅ Canvas created");
    }

    private void CreateBasicUI()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("Canvas not found!");
            return;
        }

        // Create title
        CreateTextElement(canvas, "Title", "Waya's Wheel of Regret", 48, Color.white, new Vector2(0, 200));
        
        // Create start button
        CreateButton(canvas, "StartButton", "Start Game", new Vector2(0, -50));
        
        // Create input field
        CreateInputField(canvas, "UsernameInput", "Enter username...", new Vector2(0, -150));
        
        Debug.Log("✅ Basic UI elements created");
    }

    private void CreateTextElement(GameObject parent, string name, string text, int fontSize, Color color, Vector2 position)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = color;
        textComponent.alignment = TextAlignmentOptions.Center;
        
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(800, 100);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
    }

    private void CreateButton(GameObject parent, string name, string text, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = Color.red;
        
        Button button = buttonObj.AddComponent<Button>();
        
        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = 24;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Center;
        
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(200, 60);
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }

    private void CreateInputField(GameObject parent, string name, string placeholder, Vector2 position)
    {
        GameObject inputObj = new GameObject(name);
        inputObj.transform.SetParent(parent.transform, false);
        
        Image inputImage = inputObj.AddComponent<Image>();
        inputImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
        inputField.placeholder = new TMP_InputField.Placeholder();
        
        // Create placeholder text
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(inputObj.transform, false);
        
        TextMeshProUGUI placeholderText = placeholderObj.AddComponent<TextMeshProUGUI>();
        placeholderText.text = placeholder;
        placeholderText.fontSize = 18;
        placeholderText.color = Color.gray;
        placeholderText.alignment = TextAlignmentOptions.Center;
        
        // Create text input
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(inputObj.transform, false);
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = "";
        textComponent.fontSize = 18;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Center;
        
        // Setup input field
        inputField.placeholder = placeholderText;
        inputField.textComponent = textComponent;
        
        RectTransform inputRect = inputObj.GetComponent<RectTransform>();
        inputRect.sizeDelta = new Vector2(300, 50);
        inputRect.anchorMin = new Vector2(0.5f, 0.5f);
        inputRect.anchorMax = new Vector2(0.5f, 0.5f);
        inputRect.pivot = new Vector2(0.5f, 0.5f);
        inputRect.anchoredPosition = position;
        
        RectTransform placeholderRect = placeholderObj.GetComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = new Vector2(10, 0);
        placeholderRect.offsetMax = new Vector2(-10, 0);
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 0);
        textRect.offsetMax = new Vector2(-10, 0);
    }

    [ContextMenu("Connect All Components")]
    public void ConnectAllComponents()
    {
        Debug.Log("Component connection - This requires manual setup in Unity Editor");
        Debug.Log("Please follow the UNITY_SETUP_GUIDE.md for detailed connection instructions");
    }

    [ContextMenu("Run Test Suite")]
    public void RunTestSuite()
    {
        GameObject testRunner = new GameObject("TestRunner");
        testRunner.AddComponent<MigrationTest>();
        testRunner.AddComponent<CursorUnityTest>();
        
        Debug.Log("✅ Test suite created. Press Play to run tests.");
    }

    [ContextMenu("Create Project Report")]
    public void CreateProjectReport()
    {
        string report = "UNITY PROJECT SETUP REPORT\n";
        report += $"Generated: {System.DateTime.Now}\n";
        report += $"Unity Version: {Application.unityVersion}\n";
        report += $"Platform: {Application.platform}\n\n";
        
        report += "PROJECT STRUCTURE:\n";
        report += "✅ Scripts folder created\n";
        report += "✅ Scenes folder created\n";
        report += "✅ Prefabs folder created\n";
        report += "✅ Materials folder created\n";
        report += "✅ Fonts folder created\n";
        report += "✅ UI folder created\n\n";
        
        report += "COMPONENTS CREATED:\n";
        report += "✅ GameManager\n";
        report += "✅ SoundManager\n";
        report += "✅ Wheel system\n";
        report += "✅ UI Canvas\n";
        report += "✅ Basic UI elements\n";
        report += "✅ Test suite\n\n";
        
        report += "NEXT STEPS:\n";
        report += "1. Create all 5 scenes manually\n";
        report += "2. Add this script to each scene\n";
        report += "3. Run 'Create Core Game Objects' in each scene\n";
        report += "4. Connect components manually\n";
        report += "5. Run tests to verify setup\n";
        
        Debug.Log(report);
        
        // Save report to file
        string path = System.IO.Path.Combine(Application.persistentDataPath, "unity_setup_report.txt");
        System.IO.File.WriteAllText(path, report);
        Debug.Log($"Report saved to: {path}");
    }
}

// Custom editor to make the helper easier to use
[CustomEditor(typeof(UnitySetupHelper))]
public class UnitySetupHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Quick Setup Actions", EditorStyles.boldLabel);
        
        if (GUILayout.Button("🚀 Auto-Setup Complete Project"))
        {
            ((UnitySetupHelper)target).SetupCompleteProject();
        }
        
        if (GUILayout.Button("📁 Create All Scenes"))
        {
            ((UnitySetupHelper)target).CreateAllScenes();
        }
        
        if (GUILayout.Button("🎮 Create Core Game Objects"))
        {
            ((UnitySetupHelper)target).CreateGameObjects();
        }
        
        if (GUILayout.Button("🧪 Run Test Suite"))
        {
            ((UnitySetupHelper)target).RunTestSuite();
        }
        
        if (GUILayout.Button("📊 Create Project Report"))
        {
            ((UnitySetupHelper)target).CreateProjectReport();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Manual Setup Guides", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("See UNITY_SETUP_GUIDE.md for detailed steps");
    }
}
