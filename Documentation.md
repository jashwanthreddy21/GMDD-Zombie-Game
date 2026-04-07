# Zomboid Survival - Comprehensive Game Documentation

## 1. Game Overview

**Zomboid Survival** is a first-person shooter (FPS) zombie survival game developed in Unity 3D. Players must navigate a procedurally generated terrain-based environment, eliminate all zombie enemies, and manage limited resources including ammunition and flashlight battery power. The game emphasizes tension through resource scarcity, dynamic AI enemy behavior, and atmospheric procedural audio.

### Key Features
- **Survival Mechanics:** Limited flashlight power that decays over time, requiring battery pickups.
- **Combat System:** Raycasting-based shooting with three weapon types (Pistol, Shotgun, Carbine).
- **AI Enemies:** Zombies using Unity's NavMesh for intelligent pathfinding and pursuit.
- **Procedural Audio:** All sound effects and music generated algorithmically using Python scripts.
- **VR Compatibility:** UI designed to work with VR headsets.
- **Dynamic Loot:** Ammo and battery items spawn randomly on terrain surfaces using physics raycasts.

### Platform Support
- PC (Windows), Mac, Linux standalone executables.
- Customizable for other platforms via Unity build settings.

### Development Context
- **Engine:** Unity 2019.1.14f1 (upgraded to 6000.4.0f1 in current version).
- **Inspiration:** GameDev.tv Unity course, but mechanics and AI implemented from scratch.
- **License:** MIT License.
- **Repository:** Open-source on GitHub.

## 2. Gameplay Mechanics

### Objective
The primary goal is to eliminate all zombies in the terrain. The game ends when the player defeats all enemies or dies from zombie attacks.

### Player Controls
- **Movement:** WASD keys for walking, Left Shift for sprinting.
- **Camera/Look:** Mouse movement.
- **Shooting:** Left mouse button.
- **Weapon Switching:** Mouse scroll wheel or number keys (1, 2, 3).
- **Aiming:** Right mouse button (aim-down-sights with FOV reduction).
- **Interaction:** Automatic pickup when colliding with items.

### Resource Management
#### Flashlight System
- **Decay Mechanics:** Flashlight intensity decreases by `lightDecay` (0.1) per second, angle decreases by `angleDecay` (1) per second until minimum angle (40 degrees).
- **Recharge:** Collect battery pickups to restore angle to 90 degrees and add 1 intensity.
- **Gameplay Impact:** Low visibility gives zombies an advantage in close-range attacks.
- **Implementation:** Managed by `FlashLightSystem.cs` with configurable decay parameters.

#### Ammunition System
- **Weapon Types:**
  - **Pistol:** Balanced range and damage, uses `Bullets`.
  - **Shotgun:** High damage at close range, spread pattern, uses `Shells`.
  - **Carbine:** Long-range precision weapon, uses `Rockets`.
- **Ammo Types:** Separate reserves managed by `Ammo.cs` with `AmmoSlot` array.
- **Pickup Mechanics:** Ammo items spawn dynamically on terrain, add specified amount to inventory.
- **Display:** Real-time ammo count shown in HUD via TextMeshPro.

### Combat System
#### Shooting Mechanics
- **Raycasting:** Instant hit detection from camera position forward, range 100 units.
- **Hit Effects:** Particle systems for muzzle flash and impact effects, destroyed after 0.1 seconds.
- **Damage:** Configurable per weapon (30 default), applied directly to enemy health.
- **Fire Rate:** Cooldown system (0.5 seconds default) prevents rapid firing.
- **Ammo Consumption:** Reduces current ammo by 1 per shot.

#### Enemy AI
- **Detection Range:** Zombies enter "provoked" state when player is within 5 units.
- **States:**
  - **Idle:** Stationary until provoked.
  - **Chase:** NavMesh pathfinding to pursue player, sets destination to player position.
  - **Attack:** Close-range damage when within NavMesh stopping distance.
- **Audio Cues:** Zombie roars when provoked or damaged.
- **Animation:** Triggers for movement and attack states, death animation on defeat.
- **Health:** 100 hitpoints default, broadcasts "OnDamageTaken" message.

### Environmental Interaction
- **Terrain Navigation:** Unity terrain with baked NavMesh for AI pathfinding.
- **Item Spawning:** Physics raycasts from 15 units above random positions ensure items rest naturally on surfaces.
- **Safety Systems:** Fall protection teleports player back to spawn if Y position drops below 10.
- **Footsteps:** Velocity-based audio with different intervals (0.5s walk, 0.3s sprint), random pitch variation.

## 3. Technical Architecture

### Project Structure
```
zomboid-survival/
├── Assets/
│   ├── Scripts/ (17 C# MonoBehaviours)
│   ├── Scenes/ (Asylum.unity main level, Sandbox.unity test)
│   ├── Prefabs/ (Weapons, Enemies, UI, Pickups, Effects)
│   ├── Materials/ (Skybox, Physical materials, Emissive textures)
│   ├── Animations/ (Enemy idle/attack placeholders)
│   ├── Resources/ (Procedural audio files)
│   └── Sounds/ (Generated .wav files)
├── Python Scripts/ (Audio generation algorithms)
├── Packages/ (Unity dependencies)
└── ProjectSettings/ (Engine configuration)
```

### Core Systems

#### Player Systems
- **PlayerHealth.cs:**
  - Manages hitpoints (default 100), takes damage and triggers death.
  - Spawns 8 extra batteries and ammo on start using raycasts from random positions.
  - Loads and plays BGM (looping, 2D sound, 0.4 volume) and footsteps (2D sound).
  - Footstep logic: Checks movement via CharacterController or Rigidbody, plays sounds at intervals.
- **PlayerFootsteps.cs:**
  - Dedicated footstep audio system with configurable intervals.
  - Supports both CharacterController and Rigidbody movement detection.
  - Random pitch (0.9-1.1) for realism.
- **PlayerFallSafety.cs:** Respawns player at spawn position + 5 Y if below threshold, resets velocity.
- **FlashLightSystem.cs:** Decreases light intensity and angle over time, restore methods for pickups.

#### Weapon Systems
- **Weapon.cs:** Core shooting with raycast, muzzle flash, hit effects, ammo reduction.
- **WeaponSwitcher.cs:** Cycles through child weapon GameObjects via scroll or keys 1-3.
- **WeaponZoom.cs:** Toggles FOV (60 to 20) and mouse sensitivity (2 to 0.5) on right click.
- **Ammo.cs/AmmoType.cs/AmmoPickup.cs:** Inventory system with enum types, pickup adds to ammo.

#### Enemy Systems
- **EnemyAI.cs:** NavMesh agent with state machine, faces target, Gizmo visualization.
- **EnemyAttack.cs:** Applies 40 damage on attack event, shows damage UI, plays attack sound.
- **EnemyHealth.cs:** Takes damage, broadcasts message, triggers death animation.

#### Game Loop & UI
- **DeathHandler.cs:** Enables game over canvas, pauses time, disables weapon switching, unlocks cursor.
- **DisplayDamage.cs:** Shows blood splatter canvas for 0.3 seconds on damage.
- **SceneLoader.cs:** Reloads scene 0 and resumes time for restart, quits application.
- **BatteryPickup.cs:** Restores flashlight on trigger enter with player tag.
- **AmmoPickup.cs:** Increases ammo on trigger enter.

### Procedural Audio Generation
All audio is synthetically generated using Python algorithms.

#### Scripts and Algorithms
- **better_audio.py:**
  - BGM: Minor-chord progression with exponential decay envelope.
  - Concrete footsteps: Harmonic synthesis.
- **get_sounds.py:**
  - Gunshot: White noise + thump with fast decay.
  - Footsteps: Thud + grit with exponential envelope.
  - Zombie Roar: FM synthesis with sine envelope.
  - Zombie Attack: Noise sweep + thud.
- **grass_audio.py:** Rumble modulated with sine waves.
- **get_bgm.py:** Drone + wind simulation.

#### Technical Details
- **Format:** 16-bit WAV, 44.1kHz, mono.
- **Synthesis:** Envelopes, modulation, noise bursts.
- **Output:** Placed in Resources/ and Sounds/.

### Dependencies and Packages
- AI Navigation, UI, Timeline, Physics, Audio modules.

## 4. Setup and Build Instructions

### Prerequisites
- Unity 2019.1.14f1+.
- Python 3.

### Setup Steps
1. Clone repository.
2. Open in Unity.
3. Load Asylum.unity.
4. Run Python scripts for audio.
5. Bake NavMesh.
6. Build for target platform.

### Configuration
- NavMesh: Default bake.
- Audio: Spatial settings.
- Physics: Raycast parameters.
- UI: TextMesh Pro.

## 5. Development Notes

### Design Philosophy
- Educational Unity fundamentals.
- Procedural content creation.
- Performance optimization.
- Accessibility.

### Limitations
- Placeholder animations.
- Fixed terrain.
- Limited weapons.

### Enhancements
- More weapons/AI.
- Procedural terrain.
- Multiplayer.

This documentation covers every aspect of Zomboid Survival in detail.
