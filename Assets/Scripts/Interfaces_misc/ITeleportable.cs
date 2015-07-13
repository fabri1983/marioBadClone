using UnityEngine;

/**
 * Defines the signature to properly handle a game object for teleporting
 */
public interface ITeleportable
{
	
	void setReadyForTeleport (bool val);
	bool isReadyForTeleport ();
	
	void setTeleporting (bool val);
	bool isTeleporting ();
	
	bool isOnTeleportPlace ();
	void setOnTeleportPlace (bool val);
	
	void setOnJump (bool val);
	bool isOnJump ();
	
	void setDirEntrance (Vector3 dir);
	
	/**
	 * To set whatever you need in the gameobject that will be teleported.
	 * Enable/disable: gravity, user input, actions, etc.
	 */
	void toogleTargetState (bool val);
	
	/**
	 * Returns the original layer the game object had right before the teleport effect
	 */
	int getOriginalLayer ();
}

