# Prefab: Background Manager

**Author:** Johnathan Van Vliet

*Copyright © 2026 Starfish Studios*
*Licensed under the MIT License.*

Unity version 6000.3.10f1 


---

## Features

- Adjust visuals based on multiple conditions:
  - Weather, set by an external script (sunny, cloudy, rainy, stormy).
  - Time, set and updated using `deltaTime`.
- Specifically adjusts:
  - Colors of background sprites.
  - Colors of the ocean backdrop.
  - Brightness of the sprites on the screen.
- Manages sounds caused by rain and thunder.
  - Randomizes pitch and volume of thunder sounds for variability.
- Moves a sun object across a curve in conjunction with the time of day.

All colors, objects, and audio sources can be set using Unity's hierarchy.

---

## Controls
None. The player doesn't interact directly with the elements of the prefab.

---

## How to Implement:
1. Drag the prefab onto the hierarchy of the secne.
2. Add helper assets for the rain particles, thunder and rain sounds, and weather control.
3. Set settings in the inspector:
  - Set the day duration in real-life seconds, if applicable.
  - Add colors to allow for time-based dynamic shading.
  - Add renderers for each element that needs shifting.
  - Add the renderer for the backdrop, rain particle, and clouds.
  - Add the sun object.
  - Add the sunrise and sunset points. These are the start and end of the sun's arc.
  - Set the arc height of the sun.
  - Set the rain sound controller.

---

## Composition and Helper Prefabs

### Scripts Used
`backgroundManager.cs` was used on the prefab.

### Helper Prefabs
`Rain`: Rain particles.
`RainSound`: Sound for the rain.
`ThunderSound`: Sound for the thunder.
`WeatherController`: Controls the weather.
  - Note: The current weather can be controlled by setting the "Current Weather" field to 'Sunny', 'Cloudy', 'Rainy', or 'Stormy' in the hierarchy.

## Common Questions

#### Where should I place the sunrise and sunset points?
These should be placed just off the screen slightly above the top of the water.

#### How tall should the sun's arc go?
Anywhere between `1.00` and `2.00` is fine. Any higher will go far off the screen and any lower will look bad.

#### Why is it not working?
Make sure everything is set in the inspector.

## Common Problems

#### The sun isn't appearing.
Make sure the sun's arc is set and that the layer is set to be in front of the backdrop but behind the clouds.

#### The sun clips into the water when it's rising/setting.
Raise the `SunrisePoint` and `SunsetPoint` until this stops happening.

#### A certain object isn't changing color
Make sure to add additional objects to the "Background Renderers" field in the inspector.

---

## Images

### Weather is Sunny
![An example of the prefab when the weather is sunny](readme-images/SunnyExample.png)

### Weather is Cloudy
![An example of the prefab when the weather is cloudy](readme-images/CloudyExample.png)

### Weather is Rainy or Stormy
![An example of the prefab when the weather is stormy or rainy](readme-images/StormyExample.png)

