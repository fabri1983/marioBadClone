using UnityEngine;

/// <summary>
/// This class defines common behavior to Move action for enemies and the player.
/// Add as abstract those similar Monobehavior methods you will need to use, call them 
/// from real Monobehavior methods, implement them in subclass.
/// </summary>
public abstract class MoveAbs : AnimateTiledConfig {
	
	public abstract void move(Vector3 dirAndPow);
	public abstract void move(float val);
	public abstract bool isMoving ();
	public abstract void stopMoving ();
}
