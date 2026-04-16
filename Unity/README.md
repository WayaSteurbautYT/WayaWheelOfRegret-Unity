# Waya's Wheel of Regret - Unity Version

A Unity conversion of the original Next.js "Wheel of Regret" game created by WayaCreate. This version brings the cosmic chaos wheel to Unity with full game mechanics, special endings, and personality detection.

## Game Features

### 🎮 Game Modes
- **Classic**: One question, one fate
- **Chaos Chain**: 6 spins of escalating doom with chaotic AI answers
- **Wheel of Fate**: 6 spins with celebrity personality detection and special endings
- **Rapid Fire**: 6 spins with auto-spin option

### 🔮 Core Features
- Animated 12-segment wheel with realistic physics
- AI-generated answers based on doom levels
- Results presentation with scoring system
- Special endings for detected personalities
- Sound system with synthesized audio
- Settings persistence and game history
- Mobile and desktop responsive UI

### 🌟 Special Endings
- **Hammer** (Danganronpa style): For personalities like PewDiePie, Jacksepticeye
- **Pokeball**: For MrBeast, Pokimane, Sykkuno
- **SCP Foundation**: For Markiplier, Technoblade, Valkyrae
- **Glitch**: For Dream, Ninja, xQc

## Installation & Setup

### Prerequisites
- Unity 2022.3 LTS or later
- TextMeshPro package (included with Unity)
- UI Toolkit (optional, for enhanced UI)

### Project Setup

1. **Create New Unity Project**
   ```
   - Open Unity Hub
   - Create new project using 2D URP template
   - Name: "WayaWheelOfRegret"
   ```

2. **Import Scripts**
   ```
   - Copy all scripts from Unity/Assets/Scripts/ to your project's Assets/Scripts/
   - Unity will automatically create the necessary folder structure
   ```

3. **Scene Setup**
   Create the following scenes in your project:
   ```
   - LoadingScene
   - MenuScene  
   - GameScene
   - ResultsScene
   - CreditsScene
   ```

4. **Configure Build Settings**
   ```
   - File → Build Settings
   - Add all 5 scenes to build
   - Set LoadingScene as index 0 (first scene)
   ```

## Scene Configuration

### LoadingScene
- Add `GameManager` prefab (create empty GameObject with GameManager.cs)
- Add `SoundManager` prefab (empty GameObject with SoundManager.cs)
- Add `LoadingScreen` component to Canvas
- Configure UI components: app icon, progress bar, tip text

### MenuScene
- Main menu with game mode selection
- Username input field
- Settings and History modals
- Background particle effects

### GameScene
- Wheel spinner component
- Question input system
- Result display panels
- Mobile navigation (bottom bar)
- Desktop sidebar

### ResultsScene
- Presentation system for results
- Star rating animation
- Special ending effects
- Final score display

### CreditsScreen
- Rolling credits animation
- Starfield background
- YouTube link button

## Required Assets

### UI Elements
- App icon (512x512 PNG)
- Wheel sprite (1024x1024 with 12 segments)
- Pointer triangle sprite
- Font: Space Grotesk (or similar bold font)
- Font: Monospace for numbers

### Audio
All audio is generated programmatically through the SoundManager using Web Audio API-style synthesis. No external audio files required.

## Controls & Input

### Desktop
- Mouse click for all interactions
- Keyboard input for username field
- ESC to return to menu

### Mobile
- Touch input for all buttons
- On-screen keyboard for username
- Bottom navigation bar for spin history

## Game Mechanics

### Wheel Spinning
- 5-8 full rotations over 4 seconds
- Cubic easing for realistic deceleration
- Tick sounds that decrease in frequency
- Lightning effects during spin

### Scoring System
```
Stars: (100 - AverageDoom) / 20 (0-5 stars)
Regret Level: Average doom percentage
Creativity Points: Question length × 2 (max 100)
Stupidity Points: AverageDoom × 0.8 + Random(0-20)
Total Points: (Creativity + (100-Regret) + Stars×20) × ModeMultiplier
```

### Mode Multipliers
- Classic: 1.0x
- Chaos: 2.0x
- Fate: 1.5x
- Rapid: 1.2x

## Personality Detection

The system detects usernames and applies special endings:
- Normalizes usernames (lowercase, alphanumeric only)
- Checks exact matches first
- Falls back to partial matches
- Supports common variations (removes "create", "yt", etc.)

## Build Settings

### Target Platforms
- **Windows**: Standalone executable
- **Mac**: Universal build
- **WebGL**: Browser version (recommended for web deployment)
- **Android**: Mobile version
- **iOS**: Mobile version

### WebGL Build Settings
```
- Compression Format: Brotli
- Memory Size: 256MB
- Template: Minimal
- Linker Target: Faster Build
```

## Performance Considerations

### Optimizations
- Use object pooling for particle effects
- Limit concurrent animations
- Compress UI textures
- Use sprite atlases for icons

### Memory Management
- Game history limited to 20 sessions
- Audio clips generated on-demand
- Scene unloading for memory efficiency

## Troubleshooting

### Common Issues

**Wheel not spinning**
- Check WheelSpinner component references
- Ensure button onClick events are set
- Verify input field is properly configured

**No sound**
- Check SoundManager instance
- Verify audio sources are added
- Check master volume settings

**Save system not working**
- Verify write permissions
- Check Application.persistentDataPath
- Ensure JSON serialization is working

**UI not responding**
- Check Canvas scaler settings
- Verify raycast target settings
- Check graphic raycaster component

### Debug Mode
Enable debug logging by setting:
```csharp
#define DEBUG_MODE
```
At the top of GameManager.cs to see detailed console output.

## Customization

### Adding New Personalities
Edit `PersonalityDetector.cs`:
```csharp
{"newusername", new PersonalityData { 
    name = "DisplayName", 
    trait = "Description", 
    endingType = EndingType.Hammer 
}}
```

### Modifying Wheel Segments
Edit `WHEEL_SEGMENTS` in `GameState.cs` to change:
- Segment text
- Colors  
- Doom values

### Custom Sounds
Modify `SoundManager.cs` to:
- Change audio synthesis parameters
- Add new sound effects
- Adjust volume and timing

## Credits

**Created by**: Waya Steurbaut  
**Channel**: WayaCreate on YouTube  
**Development**: Vibe Coding with AI  
**Original Concept**: Bad Decisions Everywhere  

## License

This project is for educational and entertainment purposes. Feel free to modify and distribute while giving credit to the original creator.

## Support

For issues, feature requests, or questions:
- Check the troubleshooting section above
- Visit WayaCreate on YouTube
- Leave a comment on the original project video

---

**Version**: 1.0.0  
**Build**: 2024.1  
**Engine**: Unity 2022.3 LTS
