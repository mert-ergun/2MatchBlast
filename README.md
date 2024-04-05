# Dream Games Software Engineering Case Study 2024

## Overview
A Unity-based 2-Match Blast game like Blast Toon, Toy Blast etc. developed for Dream Games internship application, without any previous unity or game development experience.

## Features
- Tap 2 same-color cubes to match and BLAST! them.
- A simple and engaging user interface design.
- Dynamic block generation mechanic.
- TNT blocks to BLAST! MORE!!
- Particle and basic animation system.
- Patterned designs.
- Detailed documentation in source code.
- Level progression.

## Design Patterns
In the development of this project, several design patterns were employed to ensure code modularity, scalability, and maintainability:
- Singleton Pattern: Used for various game managers like Grid Manager, LevelSaver etc. Ensures only a single instance is available during gameplay.
- Object Pooling: Implemented for efficient management of game objects, reducing the overhead of instantiating and destroying objects frequently. (I learned about it in the presentation given by Mr. Hakan Sağlam at METU)
- Factory Method: Employed in the creation of blocks and particles, allowing for flexibility in extending the types of objects created without modifying the client code.
- State Pattern: Manages the game's different states (idle, falling, finished) to ensure player moves are applicable.

## By Mert ERGÜN
