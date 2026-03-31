Fishing Mechanism

I am responsible for the fishing mechanism, which is the primary way the player interacts with the game world. My feature will handle the physics of the hook as it sinks and the logic for catching fish and bringing them back up to the boat. 

Fishing Physics and Movement 

	The hook will move underwater based on player input and the specific stats of the hook being used. I will program the movement to work on different platforms like PC and mobile so the controls feel smooth for every user. The 	line tension and sinking speed will be calculated to make the fishing experience feel natural. 

Hook Inheritance System 

	I am using inheritance to create different types of hooks that the player can choose from to change their strategy. Both hooks will share basic movement code but will have unique traits:

		1- Heavy Hook: This hook will have extra weight and a higher sink velocity. It is meant to reach deeper water faster and is better for catching big fish. 

		2- Small Hook: This hook is designed for precision. It will target specific smaller fish sizes and will be easier to move around obstacles.

Action Steps 

	1- Create the base hook logic that handles position and movement. 

	2- Build the collision system to detect when the hook touches a fish. 

	3- Implement the inheritance for Small and Heavy hooks to give players options. 

	4- Connect the hook to the inventory so caught fish are saved correctly. 

	5- Synchronize the movement logic so it works across different platforms like VR and Mobile.

Hook Controls:

	Space (Tap): Cast the hook from the rod tip into the water.

	Space (Hold): Reel the hook back up toward the surface. 

	A/D or Left/Right Arrows: Move the hook horizontally while underwater.



	

