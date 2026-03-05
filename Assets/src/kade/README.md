# Stats/Storyline

User stats are data that impact the gameplay. Such examples of stats would be risk factor and hunger. The storyline would be the main gameplay loop for the game.

## Examples of Stats

-Risk: Risk is added upon when illegal activity is performed, called by CalculateRisk() in Scripts/StatCalculation.cs
- CalculateRisk(action)
    - action: The illegal action that the player is performing, valid values are "steal", "nightFish", and "blackMarket"
    - Make sure that if a player is performing these actions that this function is called
