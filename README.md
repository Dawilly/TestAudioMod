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
play `wind` cue    | `3`             | Supports frequency changes (Lowpass). Starts at 3kHz. Vanilla cue only works after loading a save.
play song sample   | `4`             | Mod source only. Plays a rendition of _stillness in the rain_ by MissCoriel.
play `flute` cue   | `5`             |
play `rain` cue    | `6`             | Supports frequency changes (Lowpass). Starts at 20kHz. Vanilla cue only works after loading a save.
play song sample   | `7`             | Mod source only. Plays a shorting rendition of _stillness in the rain_ by MissCoriel in .wav format.
change source      | `Tab`           | Toggles between the vanilla game sound and an equivalent `.ogg` file.
change pitch       | `left`, `right` |
change frequency   | `up`, `down`    | Changes the frequency cutoff/centre when a filter is applied.
