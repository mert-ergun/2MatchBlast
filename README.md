# 2-Match Blast Game: Unity-based Portfolio Project

#### Overview
A Unity-based 2-Match Blast game inspired by popular mobile puzzle games, developed to showcase game development skills. This project was created without prior Unity or game development experience, demonstrating rapid learning and implementation abilities.

#### Features
- Match and blast mechanics: Tap 2 same-color cubes to match and eliminate them.
- Engaging user interface design.
- Dynamic block generation system.
- Special TNT blocks for enhanced gameplay.
- Particle effects and basic animation system.
- Patterned level designs.
- Detailed in-code documentation.
- Level progression system.

#### Design Patterns Implemented
This project showcases the use of several key design patterns to ensure code modularity, scalability, and maintainability:

- **Singleton Pattern**: Utilized for various game managers (e.g., Grid Manager, LevelSaver) to ensure single instance availability during gameplay.
- **Object Pooling**: Implemented for efficient management of game objects, reducing instantiation and destruction overhead.
- **Factory Method**: Employed in the creation of blocks and particles, allowing for flexible object type extensions.
- **State Pattern**: Manages different game states (idle, falling, finished) to control player move applicability.

Developed by Mert ERGÜN
