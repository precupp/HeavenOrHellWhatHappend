# Heaven or Hell — Sky / Chapter Theme Spec (Step 5)

This folder contains skybox materials used when switching between Heaven and Hell on the shared blockout map.

## Heaven
- Skybox: `Sky_Heaven.mat` (blue-white gradient)
- Ambient: `(0.85, 0.88, 0.95)`
- Fog: off or very light blue-white
- Main light: warm white, intensity ~1.2
- Marked item VFX: golden

## Hell
- Skybox: `Sky_Hell.mat` (red-orange gradient)
- Ambient: `(0.25, 0.08, 0.05)`
- Fog: on, red-orange, density ~0.02
- Main light: warm red, intensity ~0.6
- Marked item VFX: red-orange

## Step 5 hookup
`ChapterThemeApplier.ApplyTheme(ChapterTheme)` should set:
- `RenderSettings.skybox`
- `RenderSettings.ambientLight`
- `RenderSettings.fog` / `fogColor` / `fogDensity`
- scene directional light color + intensity

Fade-out -> theme switch -> `MarkedItemSpawner.SpawnForChapter(...)` -> fade-in.
