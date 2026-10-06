# Unity Spatial Puzzle Mechanics

A VR spatial puzzle prototype developed in Unity and inspired by the interaction mechanics of *Cubism*.

The project explores object manipulation, rotational snapping, socket-based placement and physics-driven puzzle validation in a three-dimensional environment.

## Gameplay Systems

### Spatial object interaction
Puzzle pieces can be manipulated and rotated by the player using Meta XR interaction components.

Object orientation is normalized to 90-degree increments to support predictable spatial placement.

### Puzzle validation
Puzzle completion is evaluated using physics queries and socket occupancy checks.

The system verifies that:

- required socket positions are occupied,
- relevant puzzle elements are located within the target structure,
- completion conditions are satisfied before progressing.

### Level progression
A lightweight level manager handles:

- level spawning,
- puzzle completion state,
- progression persistence using `PlayerPrefs`,
- win effects and audio feedback.

### Interactive menu
Snap-based interactive objects are also used as menu controls for:

- selecting levels,
- loading saved progress,
- exiting the application.

## Tech Stack

- Unity 6
- C#
- Meta XR SDK
- Oculus Interaction SDK
- OpenXR
- Unity XR Interaction Toolkit
- Unity Physics

## Code Structure

Custom gameplay logic is located in:

```text
Assets/Scripts/
├── AutoOrientSnap.cs
├── MagicSocket.cs
└── PuzzleLogic.cs
```
AutoOrientSnap

Handles rotational normalization of interactable puzzle elements.

MagicSocket

Maps XR snap interactions to application actions such as level loading and menu navigation.

PuzzleLogic

Handles puzzle validation, level lifecycle, progress persistence and completion feedback.

Project Status
This repository represents a gameplay prototype focused on spatial interaction and puzzle mechanics rather than a production-ready game.
The project was later cleaned and documented as part of my gameplay programming portfolio.

