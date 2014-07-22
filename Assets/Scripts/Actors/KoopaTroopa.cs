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
		dieAnim.die();
		destroy();
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
	
	private void stopJumping () {
		jumpInLoop = false;
		jump.setForeverJump(false);
	}
	
	public static bool beginCollisionWithPowerUp (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		KoopaTroopa koopa = shape1.GetComponent<KoopaTroopa>();
		PowerUp powerUp = shape2.GetComponent<PowerUp>();
		
		powerUp.Invoke("destroy", 0f); // a replacement for Destroy
		// hide or kill the koopa
		if (koopa.dieAnim.isHidden())
			koopa.die();
		else
			koopa.dieAnim.hide();
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return false;
	}
	
	public static bool beginCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		KoopaTroopa koopa = shape1.GetComponent<KoopaTroopa>();
		Player player = shape2.GetComponent<Player>();
		
		if (player.isDying()) {
			arbiter.Ignore(); // avoid the collision to continue since this frame
			return false; // avoid the collision to continue since this frame
		}
		
		bool collisionFromAbove = GameObjectTools.isHitFromAbove(koopa.transform.position.y, shape2.body, arbiter);
		
		if (collisionFromAbove) {
			// if koopa was jumping then stop forever jumping
			if (koopa.jumpInLoop)
				koopa.stopJumping();
			// hide the koopa troopa or stop the bouncing of the hidden koopa
			else if (!koopa.dieAnim.isHidden() || koopa.dieAnim.isBouncing()) {
				koopa.stop();
				koopa.dieAnim.hide();
			}
			// kills the koopa
			else {
				koopa.stop();
				koopa.die();
			}
			// makes the player jumps a little upwards
			player.forceJump();
		}
		// koopa starts bouncing
		else if (koopa.dieAnim.isHidden() && !koopa.dieAnim.isBouncing()){
			koopa.stop();
			koopa.dieAnim.bounce(Mathf.Sign(koopa.transform.position.x - player.transform.position.x));
		}
		// kills Player
		else {
			koopa.stop();
			arbiter.Ignore(); // avoid the collision to continue since this frame
			LevelManager.Instance.loseGame(true); // force die animation
		}
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return true;
	}
	
	public static bool beginCollisionWithKoopaTroopa (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		KoopaTroopa koopa1 = shape1.GetComponent<KoopaTroopa>();
		KoopaTroopa koopa2 = shape2.GetComponent<KoopaTroopa>();
		bool hidden1 = koopa1.dieAnim.isHidden();
		bool hidden2 = koopa2.dieAnim.isHidden();
		bool bouncing1 = koopa1.dieAnim.isBouncing();
		bool bouncing2 = koopa2.dieAnim.isBouncing();
		
		// avoid koopa1 pushes hidden koopa2
		Chase chase = shape1.GetComponent<Chase>();
		if (chase != null && chase.isChasing()) {
			chase.stopChasing();
			chase.enableOperateWhenOutOfSensor();
		}
		// avoid koopa2 pushes hidden koopa1
		chase = shape2.GetComponent<Chase>();
		if (chase != null && chase.isChasing()) {
			chase.stopChasing();
			chase.enableOperateWhenOutOfSensor();
		}
		
		// is koopa above the other koopa?
		if (GameObjectTools.isGrounded(arbiter)) {
			if (!hidden1)
				koopa1.jump.forceJump(koopa1.jumpSpeed);
			else
				koopa2.jump.forceJump(koopa1.jumpSpeed);
			// NOTE: I assume here the isGrounded() works as expected
			return false; // avoid the collision to continue since this frame
		}
		// kills koopa 2
		else if (bouncing1 && !hidden2 && !bouncing2)
			koopa2.die();
		// kills koopa 1
		else if (bouncing2 && !hidden1 && !bouncing1)
			koopa1.die();
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return true;
	}
}
