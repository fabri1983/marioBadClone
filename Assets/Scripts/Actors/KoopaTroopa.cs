using UnityEngine;

public class KoopaTroopa : MonoBehaviour, IPausable, IMortalFall {
	
	public bool jumpInLoop = false;
	public float jumpSpeed = 20f;

	private KoopaTroopaDieAnim dieAnim;
	private Patrol patrol;
	private Chase chase;
	private Jump jump;
	private ChipmunkShape shape;

	void Awake () {
		jump = GetComponent<Jump>();
		dieAnim = GetComponent<KoopaTroopaDieAnim>();
		patrol = GetComponent<Patrol>();
		chase = GetComponent<Chase>();
		shape = GetComponent<ChipmunkShape>();
		
		PauseGameManager.Instance.register(this);
		
		// set forever jumping if enabled
		if (jumpInLoop && jump != null) {
			jump.setForeverJump(true);
			jump.setForeverJumpSpeed(jumpSpeed);
		}
	}
	
	void OnDestroy () {
		PauseGameManager.Instance.remove(this);
	}
	
	/**
	 * Self implementation for destroy since using GamObject.Destroy() has a performance hit in android.
	 */
	private void destroy () {
		shape.enabled = false; // makes the shape to be removed from the space
		GameObjectTools.ChipmunkBodyDestroy(GetComponent<ChipmunkBody>());
		gameObject.SetActiveRecursively(false);
		PauseGameManager.Instance.remove(this);
	}
	
	public void pause () {
		gameObject.SetActiveRecursively(false);
	}
	
	public void resume () {
		gameObject.SetActiveRecursively(true);
	}
	
	public bool isSceneOnly () {
		return true;
	}
	
	private void die () {
		stop();
		if (dieAnim.isHidden())
			destroy();
		else
			dieAnim.hide();
	}
	
	public void dieWhenFalling () {
		stop();
		destroy();
	}
	
	private void stop () {
		chase.stopChasing();
		patrol.stopPatrol();
		if (jump != null)
			jump.setForeverJump(false);
	}
	
	public static bool beginCollisionWithPowerUp (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		KoopaTroopa koopa = shape1.GetComponent<KoopaTroopa>();
		PowerUp powerUp = shape2.GetComponent<PowerUp>();
		
		if (koopa.dieAnim.isDying() || !powerUp.isLethal())
			return false; // avoid the collision to continue since this frame
		else {
			powerUp.Invoke("destroy", 0f); // a replacement for Destroy
			koopa.die(); // might hide or kill the koopa
		}
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return false;
	}
	
	public static bool beginCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		KoopaTroopa koopa = shape1.GetComponent<KoopaTroopa>();
		Player player = shape2.GetComponent<Player>();
		
		if (koopa.dieAnim.isDying() || player.isDying()) {
			arbiter.Ignore(); // avoid the collision to continue since this frame
			return false; // avoid the collision to continue since this frame
		}
		
		koopa.stop();
		bool collisionFromAbove = GameObjectTools.isHitFromAbove(koopa.transform.position.y, shape2.body, arbiter);
		
		if (collisionFromAbove) {
			// hide the koopa troopa or stop the bouncing of the hidden koopa
			if (!koopa.dieAnim.isHidden() || koopa.dieAnim.isBouncing())
				koopa.dieAnim.hide();
			// kills the koopa
			else
				koopa.die();
			// makes the player jumps a little upwards
			Vector2 theVel = shape2.body.velocity;
			theVel.y = player.lightJumpVelocity;
			shape2.body.velocity = theVel;
		}
		// koopa starts bouncing
		else if (koopa.dieAnim.isHidden() && !koopa.dieAnim.isBouncing()){
			koopa.dieAnim.bounce(Mathf.Sign(koopa.transform.position.x - player.transform.position.x));
		}
		// kills Player
		else {
			arbiter.Ignore(); // avoid the collision to continue since this frame
			LevelManager.Instance.loseGame(true); // force die animation
		}
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return true;
	}
}
