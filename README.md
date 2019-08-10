**Test Audio Mod** is a temporary Stardew Valley mod used to prototype the [SMAPI](https://smapi.io/)
audio API.

## Usage
1. Install [SMAPI](https://smapi.io/).
2. Compile the mod to automatically copy it into your game's `Mods` folder.
3. Launch the game.

Controls:

action             | control         | notes
:----------------- | :-------------- | :----
disable audio      | `1`             |
play `SinWave` cue | `2`             | Supports pitch changes.
play `wind` cue    | `3`             | Supports frequency changes. Vanilla cue only works after loading a save.
change source      | `Tab`           | Toggles between the vanilla game sound and an equivalent `.ogg` file.
change pitch       | `left`, `right` |
change frequency   | `up`, `down`    | Not implemented for mod sounds yet.
