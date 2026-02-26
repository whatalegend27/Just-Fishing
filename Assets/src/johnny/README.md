# The Boat

The boat is the location from which the player fishes.  
It is also the object that controls weather and terrain generation, as it is the hub for the player's actions while at sea.

## Global Difficulty and Terrain Generation

Global difficulty is a value used for terrain generaiton. It is calculated using:
- Time of Day
- Current Weather
- Random offset
- Player-set difficulty

This value is then used as a seed to determine the types of terrain the player encounters.

## Boat Movement

To simulate a realistic boating experience, the boat will be randomly shifted by the current of the water. This is part of what determines the boat's interaction with terrain objects generated using the global difficulty algorithm.

## Background and Audio

The boat controls the scene backgrounds while at sea. Both audio and backgrounds respond to weather and time of day.