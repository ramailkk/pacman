# Pac-Man

Ongoing implementation of Pac-Man based on [The Pac-Man Dossier by Jamie Pittman](https://pacman.holenet.info/) - Good read which covers all the base logic and structure to the game.

It's one of those projects I'd prefer to not to use AI and use my own fingers for a change.

## Status

The base game is roughly **85% complete**. Core game logic and ghost AI are working as intended, including idle states.

## What's Left
### 1. Ghost House Logic
How each ghost leaves the ghost house is currently governed by a local counter only, need to implement more changes (mentioned in dossier):
- Global dot counter mode
- Switching between Global and Local
- Separate Timer if global and local counters don't increment

### 2. Animations & Transitions
No animation, transition, or timing currently exists for key game events such as:
- Losing a life
- A ghost being eaten
- Advancing to a new level
- Game over
For now things happen rapidly

### 3. Graphics & Sound
Only a barebones debug GUI exists so far, built in RayLib with custom shapes, colors, and animations. The long-term plan is to replace this with the original sprites, sounds, and animations to emulate the original arcade version.

### 4. Code Cleanup
Some general cleaning and generalization needed between classes and states, wont be too much of a work hopefully.

## Reference
- [The Pac-Man Dossier by Jamie Pittman](https://pacman.holenet.info/)

## Built With

- RayLib (current debug rendering)
