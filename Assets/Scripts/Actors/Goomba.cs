using UnityEngine;

public class Goomba : MonoBehaviour {

	private GoombaDieAnim goombaDie;
	private Patrol patrol;
	private Idle idle;
	private float heightHalf;
	
	void Awake () {
		goombaDie = GetComponent<GoombaDieAnim>();
		patrol = GetComponent<Patrol>();
		idle = GetComponent<Idle>();

#if UNITY_EDITOR		
		if (GetComponentInChildren<AnimateTiledTexture>() == null)
			Debug.LogWarning("Can't get height from renderer component");
		else
#endif
			heightHalf = GetComponentInChildren<AnimateTiledTexture>().renderer.bounds.size.y / 2f;
	}
	
	void OnDestroy () {
		GameObjectTools.ChipmunkBodyDestroy(gameObject);
	}
	
	public static bool beginCollisionWithPowerUp (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		Goomba goomba = shape1.GetComponent<Goomba>();
		PowerUp powerUp = shape2.GetComponent<PowerUp>();
		
		if (goomba.goombaDie.isDying() || !powerUp.isLethal())
			return false; // avoid the collision to continue since this frame
		else {
			Destroy(powerUp.gameObject);
			goomba.goombaDie.die();
		}
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return false;
	}
	
	public static bool beginCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		Goomba goomba = shape1.GetComponent<Goomba>();
		Player player = shape2.GetComponent<Player>();
		
		if (goomba.goombaDie.isDying() || player.isDying())
			return false; // avoid the collision to continue since this frame
		
		goomba.patrol.stopPatrol();
		goomba.idle.setIdle(true);
		
		// if collides from top then kill the goomba
		if (GameObjectTools.isHitFromAbove(goomba.transform.position.y + goomba.heightHalf, shape2, arbiter)) {
			goomba.goombaDie.die();
			// makes the killer jumps a little upwards
			Jump jump = shape2.GetComponent<Jump>();
			if (jump != null)
				jump.forceJump(player.lightJumpVelocity * 1.8f);
		}
		// kills Mario
		else {
			arbiter.Ignore(); // avoid the collision to continue since this frame
			LevelManager.Instance.loseGame(true); // force die animation
		}
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return true;
	}
}
