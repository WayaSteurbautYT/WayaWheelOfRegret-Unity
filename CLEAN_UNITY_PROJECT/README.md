# Waya's Wheel of Regret - Unity Version

**Complete Unity conversion of the original v0/Next.js project**

## Quick Start

1. Clone this repository
2. Open in Unity 2022.3 LTS or later
3. Press Play

## Project Structure

```
Assets/
Scripts/
  GameManager.cs          # Main game management
  GameState.cs             # Core game data and logic
  WheelOfRegret.cs         # Wheel spinning mechanics
  MainMenu.cs              # Main menu UI
  SoundManager.cs          # Audio system
  ResultsScreen.cs         # Results presentation
  CreditsScreen.cs         # Credits sequence
  TestSuite.cs             # Automated testing

Scenes/
  LoadingScene.unity       # Splash screen
  MenuScene.unity          # Main menu
  GameScene.unity          # Main game
  ResultsScene.unity       # Results screen
  CreditsScene.unity       # Credits
```

## Features

- **4 Game Modes**: Classic, Chaos Chain, Wheel of Fate, Rapid Fire
- **12-Segment Wheel**: Animated with realistic physics
- **AI Answers**: Dynamic responses based on doom levels
- **Personality Detection**: Special endings for 15+ celebrities
- **Sound System**: Procedural audio (no external files needed)
- **Save System**: Persistent settings and game history

## Testing

The project includes a comprehensive test suite. Press Play in any scene and check the Console window to see test results.

## Build

Build for Windows, Mac, WebGL, Android, or iOS from Unity's Build Settings.

## Original Project

This is the Unity conversion of the original Next.js/v0 project created by WayaCreate.

---

**Original Creator**: WayaCreate on YouTube  
**Unity Conversion**: Vibe Coding with AI
