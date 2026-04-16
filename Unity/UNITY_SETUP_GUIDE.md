# Unity Setup Guide - Waya's Wheel of Regret

## 🎯 Complete Beginner's Guide to Unity Components

This guide will walk you through setting up the entire Unity project step by step, even if you haven't used Unity in a long time.

## 📋 Prerequisites

1. **Unity Hub** installed
2. **Unity 2022.3 LTS** or later
3. Your project files from the migration

---

## 🚀 Step 1: Create New Unity Project

1. Open Unity Hub
2. Click **"New project"**
3. Select **"2D URP"** template (important for UI)
4. Project name: `WayaWheelOfRegret`
5. Location: Choose your folder
6. Click **"Create project"**

---

## 📂 Step 2: Project Structure Setup

### Create Folders
In the **Project** window (usually at bottom), create these folders:
```
Assets/
├── Scripts/          (for C# files)
├── Scenes/           (for Unity scenes)
├── Prefabs/          (for reusable objects)
├── Materials/        (for colors/textures)
├── Fonts/           (for text fonts)
└── UI/              (for UI elements)
```

**How to create folders:**
- Right-click in Project window → **Create** → **Folder**
- Name it as shown above

---

## 📄 Step 3: Add All Scripts

1. Copy all `.cs` files from the migration to `Assets/Scripts/`
2. Unity will automatically detect and compile them

**Scripts you should have:**
- `GameState.cs`
- `GameStateMigrated.cs`
- `GameManager.cs`
- `GameManagerMigrated.cs`
- `WheelOfRegret.cs`
- `WheelOfRegretMigrated.cs`
- `SoundManager.cs`
- `PersonalityDetector.cs`
- `AIAnswerGenerator.cs`
- `MigrationTest.cs`
- `CursorUnityTest.cs`
- Plus all UI scripts (LoadingScreen, MainMenu, etc.)

---

## 🎬 Step 4: Create All Scenes

### Create 5 Scenes:
1. **LoadingScene** - Splash screen
2. **MenuScene** - Main menu
3. **GameScene** - Main game
4. **ResultsScene** - Results presentation
5. **CreditsScene** - Credits

**How to create scenes:**
1. Go to **File** → **New Scene**
2. Choose **"Basic (Built-in)"** template
3. Save it: **File** → **Save As**
4. Navigate to `Assets/Scenes/`
5. Name it exactly as shown above
6. Repeat for all 5 scenes

---

## 🎮 Step 5: Setup LoadingScene

### Add Canvas:
1. In LoadingScene, right-click in **Hierarchy** → **UI** → **Canvas**
2. Select the Canvas
3. In Inspector, set **Canvas Scaler** → **UI Scale Mode** to **"Scale With Screen Size"**
4. Reference Resolution: `1920 x 1080`
5. Match: `0.5` (width and height)

### Add LoadingScreen Component:
1. Right-click Canvas → **Create Empty**
2. Name it `LoadingScreen`
3. With `LoadingScreen` selected, click **"Add Component"** in Inspector
4. Search for `LoadingScreen` and add it

### Add UI Elements:
1. Right-click LoadingScreen → **UI** → **Image** (name it `AppIcon`)
2. Right-click LoadingScreen → **UI** → **Text - TextMeshPro** (name it `Title`)
3. Right-click LoadingScreen → **UI** → **Image** (name it `ProgressBar`)
4. Right-click LoadingScreen → **UI** → **Text - TextMeshPro** (name it `Percentage`)
5. Right-click LoadingScreen → **UI** → **Text - TextMeshPro** (name it `TipText`)

### Connect Components:
1. Select the `LoadingScreen` GameObject
2. In Inspector, drag each UI element to the corresponding field:
   - Drag `AppIcon` to **App Icon** field
   - Drag `Title` to **Title Text** field
   - Drag `ProgressBar` to **Progress Bar** field
   - etc.

---

## 🏠 Step 6: Setup MenuScene

### Create Canvas (same as Step 5)

### Add MainMenu Component:
1. Create empty GameObject under Canvas named `MainMenu`
2. Add **MainMenu** component
3. Create UI elements:
   - **Image** for `AppIcon`
   - **Text - TextMeshPro** for `Title`
   - **Input Field - TextMeshPro** for `UsernameInput`
   - **Button** for `StartButton`
   - **Button** for `SettingsButton`
   - **Button** for `HistoryButton`

### Create Game Mode Buttons:
1. Create 4 buttons for game modes
2. Name them: `ClassicModeButton`, `ChaosModeButton`, `FateModeButton`, `RapidModeButton`
3. Add Text - TextMeshPro children for button labels

### Connect Everything:
1. Select `MainMenu` GameObject
2. Drag all UI elements to their respective fields in the Inspector

---

## 🎰 Step 7: Setup GameScene (Most Important)

### Create Canvas (same as before)

### Add GameScreen Component:
1. Create empty GameObject named `GameScreen`
2. Add **GameScreen** component

### Create Wheel:
1. Create empty GameObject named `WheelContainer`
2. Under it, create `Wheel` (empty GameObject)
3. Add **Image** component to `Wheel` (this will be the wheel)
4. Add **WheelOfRegret** component to `WheelContainer`

### Create Wheel Visuals:
1. Select the `Wheel` Image
2. Set **Source Image** to a circle sprite (or create one)
3. Set **Color** to dark gray (`#1A1A1F`)
4. Add segment dividers (create small Image objects as children)

### Add Input System:
1. Create **Input Field - TextMeshPro** for questions
2. Create **Button** for spinning
3. Create **Image** for result panel

### Connect Components:
1. Select `GameScreen`
2. Connect all UI elements in Inspector
3. Select `WheelContainer`
4. Connect wheel components to `WheelOfRegret` script

---

## 🏆 Step 8: Setup ResultsScene

### Create Canvas and ResultsScreen component
### Create presentation UI elements
### Connect all components

---

## 📜 Step 9: Setup CreditsScene

### Create Canvas and CreditsScreen component
### Create starfield background
### Create rolling credits text
### Connect components

---

## 🔧 Step 10: Add GameManager

### In Every Scene:
1. Create empty GameObject named `GameManager`
2. Add **GameManager** component
3. Create empty GameObject named `SoundManager`
4. Add **SoundManager** component

### Make Persistent:
1. Select `GameManager` GameObject
2. In Inspector, check **"Don't Destroy On Load"** (or add script)

---

## ⚙️ Step 11: Configure Build Settings

1. Go to **File** → **Build Settings**
2. Click **"Add Open Scenes"** to add current scene
3. Click **"Add Open Scenes"** for each of the 5 scenes
4. Drag scenes to order:
   0. LoadingScene
   1. MenuScene
   2. GameScene
   3. ResultsScene
   4. CreditsScene

---

## 🧪 Step 12: Add Test Components

### Add Migration Test:
1. In any scene (MenuScene is good)
2. Create empty GameObject named `TestRunner`
3. Add **MigrationTest** component
4. Add **CursorUnityTest** component
5. In Inspector, assign the `WheelComponent` field if needed

---

## 🎯 Step 13: Test Everything

1. Click **Play** button (top center)
2. Check Console for test results
3. All tests should pass
4. Navigate through scenes to test functionality

---

## 🔍 How to Add Components (Detailed)

### Method 1: Inspector Panel
1. Select GameObject in Hierarchy
2. Look at Inspector panel (right side)
3. Click **"Add Component"** button
4. Type script name (e.g., `GameManager`)
5. Click to add

### Method 2: Drag and Drop
1. Find script in Project window
2. Drag script onto GameObject in Hierarchy
3. Component automatically added

### Method 3: Component Menu
1. Select GameObject
2. Click **Component** in top menu
3. Navigate to **Scripts** → find your script

---

## 🔌 How to Connect References

### Connecting UI Elements:
1. Select the GameObject with your script (e.g., `MainMenu`)
2. In Inspector, find your script component
3. Look for fields like `App Icon`, `Start Button`, etc.
4. **Drag** UI elements from Hierarchy to these fields
5. Field should show the element name when connected

### Connecting Scripts:
1. Some scripts need references to other scripts
2. Drag GameObjects (like `GameManager`) to script fields
3. Make sure the target GameObject has the required component

---

## 🎨 Visual Setup Tips

### Colors:
- Background: `#0A0A0F` (deep black)
- Cards: `#1A1A1F` (dark gray)
- Red: `#FF0000`
- Purple: `#8B00FF`

### Fonts:
- Use **TextMeshPro** for all text
- Import a bold font for headings
- Use monospace for numbers

### Images:
- Create simple circle sprite for wheel
- Use Unity's built-in shapes for UI elements
- Add shadows and glow effects

---

## 🚀 Final Steps

1. **Save All Scenes** (Ctrl+S)
2. **Test in Play Mode**
3. **Check Console** for errors
4. **Run Migration Tests** (should all pass)
5. **Build Project** (File → Build Settings → Build)

---

## 🆘 Troubleshooting

### Common Issues:
- **"Component not found"**: Make sure script is in Scripts folder
- **"Reference missing"**: Drag correct GameObject to field
- **"UI not showing"**: Check Canvas settings and anchors
- **"Script not working"**: Check Console for error messages

### Getting Help:
1. Check Console window (bottom) for red errors
2. Make sure all references are connected
3. Verify scripts are in correct folder
4. Test with MigrationTest component

---

## 🎮 You're Ready!

Once you complete these steps, you'll have:
- ✅ Complete Unity project
- ✅ All scenes properly configured
- ✅ Working game mechanics
- ✅ Test suite to verify everything
- ✅ Ready to build and play

Start with Step 1 and work through each step. Take your time - Unity can be complex but this guide breaks it down simply!
