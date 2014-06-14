using UnityEngine;

public class Goomba : MonoBehaviour {

	private GoombaDieAnim dieAnim;
	private Patrol patrol;
	private Idle idle;
	private ChipmunkBody body;
	private ChipmunkShape shape;
	
	private const float TIMING_DIE = 0.3f;
	
	void Awake () {
		dieAnim = GetComponent<GoombaDieAnim>();
		patrol = GetComponent<Patrol>();
		idle = GetComponent<Idle>();
		body = GetComponent<ChipmunkBody>();
		shape = GetComponent<ChipmunkShape>();
	}
	
	void OnDestroy () {	
		if (body != null) {
			body.enabled = false;
			// registering a disable body will remove it from the list
			ChipmunkInterpolationManager._Register(body);
		}
	}
	
	/**
	 * Self implementaiton for destroy since using GamObject.Destroy() when running game since it has a performance hit in android.
	 */
	private void destroy () {
		body.enabled = false; // makes the body to be removed from the space
		shape.enabled = false; // makes the shape to be removed from the space
		gameObject.SetActiveRecursively(false);
	}
	
	public static bool beginCollisionWithPowerUp (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		Goomba goomba = shape1.GetComponent<Goomba>();
		PowerUp powerUp = shape2.GetComponent<PowerUp>();
		
		if (goomba.dieAnim.isDying() || !powerUp.isLethal())
			return false; // avoid the collision to continue since this frame
		else {
			Destroy(powerUp.gameObject);
			goomba.dieAnim.start();
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
		
		if (goomba.dieAnim.isDying() || player.isDying()) {
			arbiter.Ignore(); // avoid the collision to continue since this frame
			return false; // avoid the collision to continue since this frame
		}
		
		goomba.patrol.stopPatrol();
		goomba.idle.setIdle(true);
		
		// if collides from top then kill the goomba
		if (GameObjectTools.isHitFromAbove(goomba.transform.position.y, arbiter)) {
			goomba.dieAnim.start();
			goomba.Invoke("destroy", TIMING_DIE); // a replacement for Destroy with time
			// makes the killer jumps a little upwards
			Vector2 theVel = shape2.body.velocity;
			theVel.y = player.lightJumpVelocity;
			shape2.body.velocity = theVel;
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
