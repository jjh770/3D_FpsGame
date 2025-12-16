# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a 3D FPS game built with Unity 6000.0.60f1 using URP (Universal Render Pipeline). The project features player movement, weapon systems, AI-controlled monsters with state machines, and an object pooling system for performance optimization.

## Key Unity Packages

- **Unity Input System** (1.14.2): Used for player input handling
- **Cinemachine** (2.10.4): Camera control system
- **AI Navigation** (2.0.9): NavMesh-based pathfinding
- **DOTween** (via Resources): Animation and tweening

## Project Structure

The codebase follows a numbered folder organization pattern:

```
Assets/
├── 01.Scenes/          - Unity scenes
├── 02.Scripts/         - All C# scripts (see below)
├── 03.Prefabs/         - Prefabs
├── 04.Images/          - UI images and sprites
├── 05.Models/          - 3D models
├── 06.Sounds/          - Audio files
├── 07.Animations/      - Animation clips and controllers
├── 08.Fonts/           - Font assets
├── 09.Materials/       - Materials
├── 10.ETC/             - Miscellaneous assets
└── Resources/          - Resources loaded at runtime
```

## Code Architecture

### Scripts Organization (Assets/02.Scripts/)

```
02.Scripts/
├── Camera/           - Camera control (CameraFollow, CameraRotate)
├── CommonConfig/     - ScriptableObject configs (movement, gravity)
├── Environment/      - Environment objects
├── Interface/        - Core interfaces (IDamageable, IKnockbackable, IWeapon)
├── Manager/          - Singleton managers (GameManager, ObjectPool)
├── Monster/          - Monster AI and components
├── Player/           - Player components
├── Stat/             - Stat system (ResourceStat, ValueStat, ConsumableStat)
├── UI/HUD/           - HUD elements
├── Weapon/           - Weapon system (Weapon, Bomb, WeaponDataSO)
└── Grammer/          - Example/test scripts (ignore)
```

### Core Architectural Patterns

#### Component-Based Design
Both Player and Monster entities use composition with specialized components:

**Player Components:**
- `Player` (main): Implements `IDamageable`, coordinates components
- `PlayerMove`: Movement and jumping (uses `CharacterController`)
- `PlayerGunFire`: Gun weapon firing
- `PlayerBombs`: Bomb throwing
- `PlayerStats`: Health, stamina, ammo tracking
- `PlayerRotate`: Character rotation
- `GravityController`: Custom gravity handling

**Monster Components:**
- `Monster` (main): Implements `IDamageable`, `IKnockbackable`
- `MonsterAI`: State machine (Idle, Patrol, Trace, Comeback, Attack, Hit, Death)
- `MonsterMove`: Movement logic
- `MonsterCombat`: Combat and damage handling
- `MonsterStats`: Stats using ValueStat system

#### ScriptableObject-Based Configuration
Configuration is separated using ScriptableObjects:
- `WeaponDataSO`: Immutable weapon templates (damage, cooldown, recoil, ammo)
- `CharacterMoveConfigSO`: Shared movement configs (stamina costs)
- `GravityConfigSO`: Gravity settings

#### Stat System
Three types of stats in `Assets/02.Scripts/Stat/`:
- `ResourceStat`: Countable resources (ammo, bombs) with max/current tracking
- `ValueStat`: Simple float values (speed, damage)
- `ConsumableStat`: Depleting resources (health, stamina) with max/current

#### Object Pooling
`ObjectPool` singleton manages reusable objects (bombs, VFX):
- `Spawn(prefab, position, rotation)`: Get pooled object
- `Despawn(obj, delay)`: Return to pool
- Dictionary-based pools keyed by prefab name

#### Interface-Driven Interactions
- `IDamageable`: `TryTakeDamage(float damage)` - Implemented by Player, Monster
- `IKnockbackable`: `TakeKnockback(Vector3 direction, float amount)` - Implemented by Monster
- `IWeapon`: Common weapon interface

### Monster AI State Machine
Monsters use a coroutine-based state machine in `MonsterAI.cs`:
- **Idle**: Wait or transition to patrol
- **Patrol**: Wander around (currently being implemented)
- **Trace**: Chase player when in detect range
- **Comeback**: Return to spawn position
- **Attack**: Attack when in range
- **Hit**: Stagger on damage
- **Death**: Death state

State transitions based on distance calculations updated via `CheckDistance()` coroutine.

### Player Input
Currently uses legacy Input Manager (`Input.GetAxis`, `Input.GetButton`). InputSystem_Actions.inputactions file exists but may not be fully integrated.

## Development Workflow

### Opening the Project
1. Open in Unity Hub with Unity 6000.0.60f1
2. Main scene: `Assets/01.Scenes/FPSScene.unity`

### Building the Project
- Build through Unity Editor: File → Build Settings
- Platform support configured in ProjectSettings/EditorBuildSettings.asset

### Testing
- No dedicated test suite currently
- Test in Play Mode within Unity Editor

### Git Workflow
- Main branch: `main`
- Feature branches follow pattern: `JJH_FeatureName` (e.g., `JJH_Patrol`, `JJH_BombKnockback`)
- Commits follow conventional commits: `Feat:`, `Fix:`, `Refactor:`
- Pull requests are used for merging features

## Important Conventions

### Naming Conventions
- Private fields: `_camelCase` with underscore prefix
- SerializeField: `[SerializeField] private Type _variableName;`
- Public properties: `PascalCase` (often readonly getters for private fields)
- Folders: Numbered prefixes (01., 02., etc.)

### Code Organization
- Keep components focused on single responsibilities
- Use `RequireComponent` attribute for dependencies
- Initialize components in `Awake()`, set references in `Start()`
- Use `GetComponent<T>()` for component dependencies

### ScriptableObjects
- Create via `[CreateAssetMenu]` attribute
- Store in appropriate numbered folders
- Name with `SO` suffix (e.g., `WeaponDataSO`)

### Event Handling
See `Monster.cs` for event pattern:
- Subscribe in `Awake()`: `_combat.OnDeath += HandleDeath;`
- Unsubscribe in `OnDestroy()`: `_combat.OnDeath -= HandleDeath;`

## Common Tasks

### Adding New Weapon
1. Create new `WeaponDataSO` asset (Right-click → Create → Weapon System → Weapon Data)
2. Configure weapon properties (damage, cooldown, recoil)
3. Reference in weapon-firing scripts

### Adding New Monster State
1. Add state to `EMonsterState` enum
2. Add case in `MonsterAI.HandleMonsterState()` switch
3. Implement state method
4. Add transitions in relevant state methods

### Using Object Pool
```csharp
// Spawning
GameObject obj = ObjectPool.Instance.Spawn(prefab, position, rotation);

// Returning
ObjectPool.Instance.Despawn(obj);
ObjectPool.Instance.Despawn(obj, 2f); // with delay
```

### Creating Stat-Based Component
Use existing stat types from `Assets/02.Scripts/Stat/`:
```csharp
[SerializeField] private ResourceStat _ammo = new ResourceStat(30);
[SerializeField] private ConsumableStat _health = new ConsumableStat(100f);
[SerializeField] private ValueStat _moveSpeed = new ValueStat(5f);
```

## Current Development Status

Based on recent commits:
- Patrol state for monsters is in development (branch: `JJH_Patrol`)
- Bomb knockback system recently implemented
- Monster responsibility separation refactoring completed
- Gravity and consumable stat configs separated
