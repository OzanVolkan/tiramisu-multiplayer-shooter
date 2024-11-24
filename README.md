# Multiplayer Shooting Game
 A multiplayer shooter game case study based on the classic Pong game.


https://github.com/user-attachments/assets/41feef97-5da9-43db-9dd1-200aacc9a450


 ## Features

### Matchmaking

● Players are automatically matched and can start the game once connected.

### Real-time Gameplay

● Player avatars, bullet locations, and health are synchronized between players.

### Weapons

● Players can switch between two weapons (Kar98 and AK-47) with different firing rates, damage, and bullet speeds.

### Game Mechanics

● Players start with 10 health points, and the goal is to reduce the opponent's health to zero.

### End Game

● The game ends when a player's health reaches zero, and a "Rematch" option is available.

### Controls

● 1-2: Switch Weapons

● Space: Fire

● Arrow keys: Move

### Project Architecture

● The networking library based on **Photon PUN2**.

● **Observer Design Pattern** and **Singleton Design Pattern** are implemented.

● **Inheritance** is used to manage weapon and bullet classes.

● **Object Pooling** is applied for bullet creation and management.

### Additional Notes

● Developed using **Unity version 2022.3.16**


