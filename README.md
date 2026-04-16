# Waya's Wheel of Regret - Unity Version

**Complete Unity conversion of the original Next.js/v0 project**

![Unity](https://img.shields.io/badge/Unity-2022.3+-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-8.0+-239120?logo=c-sharp)
![License](https://img.shields.io/badge/License-MIT-green)

## Quick Start

1. Clone this repository
2. Open in Unity 2022.3 LTS or later
3. Press Play

## Game Features

- **4 Game Modes**: Classic, Chaos Chain, Wheel of Fate, Rapid Fire
- **12-Segment Wheel**: Animated with realistic physics
- **AI Answers**: Dynamic responses based on doom levels
- **Personality Detection**: Special endings for 15+ celebrities
- **Sound System**: Procedural audio (no external files needed)
- **Save System**: Persistent settings and game history

## How to Play

1. Enter your username (optional but unlocks special endings!)
2. Select a game mode:
   - **Classic**: 1 spin, simple fate
   - **Chaos Chain**: 6 spins of escalating doom
   - **Wheel of Fate**: Detects your personality for special endings
   - **Rapid Fire**: Auto-spin mode
3. Ask any question
4. SPIN THE WHEEL! 
5. Get your cosmic answer
6. See your results with stars and scoring

## Special Endings

The wheel detects famous YouTubers and personalities:
- **PewDiePie** - Hammer ending
- **MrBeast** - Pokeball ending
- **Markiplier** - SCP Foundation ending
- **Dream** - Glitch ending
- Jacksepticeye, Ninja, Pokimane, and 10+ more!

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

## Testing

The project includes a comprehensive test suite. Press Play in any scene and check the Console window to see test results. All tests should pass:

- [x] GameState functionality
- [x] Personality detection
- [x] Score calculation
- [x] AI answer generation
- [x] Game modes
- [x] GameManager
- [x] Sound system

## Build

Build for any platform from Unity's Build Settings:
- Windows (.exe)
- Mac (.app)
- WebGL (Browser)
- Android (.apk)
- iOS (.ipa)

## Requirements

- Unity 2022.3 LTS or later
- TextMeshPro package (included with Unity)
- No external assets required

## Original Project

This is the complete Unity conversion of the original "Waya's Wheel of Regret" created with Next.js/v0. All game mechanics, features, and special endings have been faithfully recreated.

## Creator

**WayaCreate** - [YouTube Channel](https://www.youtube.com/@WayaCreate)

*Original concept: Bad Decisions Everywhere*  
*Unity conversion: Vibe Coding with AI*

---

## License

This project is open source and available under the [MIT License](LICENSE).

---

**Ready to spin the wheel of regret? Clone, open, and play!**
