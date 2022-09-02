===========================================================================================
FULL DOCUMENTATION

https://docs.google.com/document/d/1GD1SX7ngjsjMEPrrhI3WL4i57nwL-qKu26ZcXrtBNqg/

============================================================================================
INSTRUCTIONS

I. Explaining the variables and names:
	
	* Raycasts:
		- Ray Length: the length of the distance between the center of your camera to the desired object; both raycasts' scripts don't necessarily have the same Ray Lengths, but I would recommend you set them
		with the same value. For the size of conventional doors, and your player, the raycast length should vary between 1-5f.

	* Door Attributes:

		- isDoorRotateClockwise: Some doors rotate clockwise, some doors rotate anti-clockwise, and you can choose the direction of rotation of the door however you want.
		- maxAngleOpen: The maximum angle that the door can open.
 
	* Types of Doors:

		- None: Can be opened and closed, but cannot be fully closed (like closing a door missing a bolt).
		- Non-Closable: Can be opened but cannot be closed; the door when being opened to a certain angle (startToAutoRotateAngle) will be automatically opened to its maximum angle.
		- Closable: Can be opened and closed. Setting canBeOpenedAgain = true will allow player to open the door again (with the same direction), 
		and when setting both canBeOpenedAgain = true & rotateOtherwiseWhenClosed = true the door will rotate otherwise when it is opened again.
		- Autoclose: Can be opened and automatically closed with a determined speed (speedAutoClose) when the player enters a trigger (triggerCloseDoor).

	* Forces:

		- Open Force: The force applied to the door when the door is first interacted (opened).
		- Near Force 1: The force applied when the player moves or looks closer to the (initial area of the) door after opening the door. (The player must be close 
		enough to the door by a certain distance characterized by each door.)
		- Near Force 2: The force applied when the player moves or looks really close to the (initial area of the) door.
		note: For the size of conventional doors, and your player, the forces should vary between 0-1f.

	* Distances and Lengths: 

		- distanceToDoorToApplyNearForce1: This is actually the radius of the sphere collider of the camera (which is a trigger). When the door enters the trigger, applying Near Force 1 
		after opening the door will be allowed.
		- raycastLengthToDoorToApplyNearForce2: This is a float that when the length between the center of the camera and the door is lower than this number, Near Force 2 is applied to
		the door (after opening the door and should be after being applied Near Force 1).
		(You might question why I chose different names and different measures for them, imagine when opening the door from the side.)

---------------------------- 
II. Steps to use this asset:

	1.   Add DoorHinge, DoorTrigger, and Opened tag to your tag list, and add Interact, Detect to your layer list (if they are not included yet). Make sure your player's tag is Player.

	2.1. Add DoorInteractionChecker, DetectRaycast, DoorRaycast to the player's camera.

	2.2. Set Layer Mask Interact of Door Raycast to Interact and that of Detect Raycast to Detect.

	2.3. Assign the image of your crosshair to Crosshair variable of Door Raycast.

	3.1. Duplicate the Door Wing (not the whole door with its frame), remove or unable the mesh renderer of the duplicated door (Door Trigger). (We just need its shape.)

	3.2. Set the Door Wing's tag to DoorHinge and its layer to Interact. Set the Door Trigger's tag to DoorTrigger and its layer to Detect.

	3.3. Add Box Collider (or Mesh Collider) component to Door Trigger and set isTrigger to true.

	3.4. Add Box Collider (or Mesh Collider) component to your Door Wing

	3.5. There are two ways to do this step:
		 - Add Hinge Joint component to Door Wing and modify the anchor, if needed, to be as precise as possible. Other than that, you shouldn't modify any other things in the component.
		 - Add Rigidbody component to your Door Frame (and you should be adding Mesh Collider to it too) and set Is Kinematic to true, after that, add Hinge Joint component to Door Wing and
		 attach the Rigidbody of the Door Frame to the Door Wing's Hinge Joint's Connected Body.

	3.6. Add Door script to the Door Wing.

	3.7. Choose the rotation direction of your door.

	3.8. Assign the maximum angle that your door can open.

	3.9. In case the player's camera ISN'T the ONLY camera with MainCamera tag, you should assign DoorRaycast of your camera to the Door Raycast variable.

	3.10.Assign the desired intensity of the forces you want to apply to the door.

	3.11.Assign the desired distances and lengths of the player and the camera to the door. 

	3.12.Determine the specific attributes of your door type. If your door type is Autoclose, create a trigger and add DoorCloseTrigger component to it, then assign it to triggerCloseDoor
	of the door.

	3.13.Add AudioSource component to your Door if you want to use sound effects when opening the door.

	3.14.Set the audio clips which will play when the door is being opened, closed, or pushed.

	*I have modified some attributes from the scripts of the original standard assets (the height, the radius of the character, the field of view of the camera (60 -> 50)) for the sake of 
	avoiding fouls, and you would find yourself having to modify those values in most cases. (Also a fov of 50 is really suitable for horror games!)

---------------------------- 
III. Caveats

	- This method now can only be used when the door is vertical (in which vertical direction of the door is parallel to the Y-axis; it is how most doors should be). I will continue to develop
	this and update in the future.

---------------------------- 
