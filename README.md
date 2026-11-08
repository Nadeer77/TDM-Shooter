# 🔫 TDM Shooter

A third-person multiplayer shooting game developed using **Unity and C#**, featuring 1v1 combat, real-time multiplayer gameplay, and Photon PUN networking.

## 🎮 Project Overview

TDM Shooter is a third-person multiplayer shooting game where two players compete against each other in a small combat arena. Players can create or join multiplayer rooms using a room ID and fight until one player reaches the maximum death count of five. The player who reaches five deaths loses, while the opponent wins the match.

## ✨ Features

- **Third-Person Gameplay:** Control a character in a 3D shooting environment.
- **1v1 Multiplayer:** Two players compete against each other in real-time.
- **Room Creation:** Create multiplayer rooms for other players to join.
- **Join Room by ID:** Join an existing multiplayer room using its room ID.
- **Player Connection Messages:** Displays messages when players enter or leave a room, visible to both connected players.
- **Player Movement:** Move and control the character during combat.
- **Aiming System:** Aim weapons at opponents.
- **Raycast Shooting:** Uses raycasting to detect weapon hits.
- **Weapon System:** Shoot and damage the opposing player.
- **Health System:** Tracks player health during combat.
- **Multiplayer Synchronization:** Synchronizes player actions and gameplay events across connected clients using Photon PUN and RPC methods.
- **Death Counter:** Tracks each player's deaths during the match.
- **Win/Lose Condition:** The first player to reach five deaths loses the match, and the opponent wins.

## 🕹️ How to Play

1. Launch the game.
2. One player creates a multiplayer room.
3. The second player joins using the room ID.
4. Both players enter the combat arena.
5. Move, aim, and shoot at the opponent.
6. Defeat the opponent while avoiding getting eliminated.
7. The first player to reach five deaths loses the match.

## 🛠️ Technologies Used

- Unity Engine
- C#
- Photon PUN 2
- Unity Physics
- Raycasting
- Photon RPC

## 💻 Platform

- PC
