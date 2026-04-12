# Alex's Doom

A first-person shooter built in Unity (C#), inspired by the classic Doom series.

## Requirements

- Unity 2022.3 LTS
- Universal Render Pipeline (URP)

## Project Structure

```
Assets/
  Scripts/
    Player/     — PlayerController, PlayerHealth
    Enemies/    — EnemyBase (abstract), enemy implementations
    Weapons/    — WeaponBase (abstract), weapon implementations
    UI/         — HUDController
    Level/      — GameManager
  Scenes/       — Game scenes
  Prefabs/      — Reusable prefabs
  Materials/    — Materials and shaders
  Textures/     — Texture assets
  Audio/        — Music and SFX
  Animations/   — Animation clips and controllers
```

## Getting Started

1. Open the project in Unity 2022.3 LTS or later
2. Open `Assets/Scenes/Level01` to start playing
