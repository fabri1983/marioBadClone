using UnityEngine;

public class KoopaTroopa : MonoBehaviour, IPausable, IMortalFall {
	
	public bool jumpInLoop = false;
	public float jumpSpeed = 20f;

	private Bounce bounce;
	private Patrol patrol;
	private Chase chase;
	private Jump jump;
	private Hide _hide;
	private ChipmunkShape shape;

	void Awake () {
		jump = GetComponent<Jump>();
		bounce = GetComponent<Bounce>();
		patrol = GetComponent<Patrol>();
		chase = GetComponent<Chase>();
		_hide = GetComponent<Hide>();
		shape = GetComponent<ChipmunkShape>();
		
		PauseGameManager.Instance.register(this, gameObject);
		
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
#if UNITY_4_AND_LATER
		gameObject.SetActive(false);
#else
		gameObject.SetActiveRecursively(false);
#endif
		PauseGameManager.Instance.remove(this);
	}
	
	public void pause () {}
	
	public void resume () {}
	
	public bool isSceneOnly () {
		// used for allocation in subscriber lists managed by PauseGameManager
		return true;
	}
	
	private void hide () {
		stop();
		chase.setOperable(false); // chase is not allowed to work anymore
		_hide.hide();
	}
	
	private void stop () {
		bounce.stop();
		chase.stopChasing();
		patrol.stopPatrol();
		if (jump != null)
			jump.setForeverJump(false);
	}
	
	private void die () {
		stop();
		destroy();
	}
	
	public void dieWhenFalling () {
		die();
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
		koopa.stop();
		
		// hide or kill the koopa
		if (koopa._hide.isHidden())
			koopa.die();
		else
			koopa.hide();
		
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
			else if (!koopa._hide.isHidden() || koopa.bounce.isBouncing())
				koopa.hide();
			// kills the koopa
			else {
				koopa.die();
			}
			// makes the player jumps a little upwards
			player.forceJump();
		}
		// koopa starts bouncing
		else if (koopa._hide.isHidden() && !koopa.bounce.isBouncing()){
			koopa.stop();
			koopa.bounce.bounce(Mathf.Sign(koopa.transform.position.x - player.transform.position.x));
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
		bool hidden1 = koopa1._hide.isHidden();
		bool hidden2 = koopa2._hide.isHidden();
		bool bouncing1 = koopa1.bounce.isBouncing();
		bool bouncing2 = koopa2.bounce.isBouncing();
		
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
