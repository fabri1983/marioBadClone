using UnityEngine;

public class Goomba : MonoBehaviour, IPausable, IMortalFall, IReloadable {

	private const float TIMING_DIE = 0.3f;
	
	private GoombaDieAnim dieAnim;
	private Patrol patrol;
	private Idle idle;
	private ChipmunkBody body;
	private bool doNotResume;
	
	void Awake () {
		dieAnim = GetComponent<GoombaDieAnim>();
		patrol = GetComponent<Patrol>();
		idle = GetComponent<Idle>();
		body = GetComponent<ChipmunkBody>();
		
		patrol.setDir(1f); // initialize patrol direction
	}

	void Start () {
		ReloadableManager.Instance.register(this as IReloadable, transform.position);
		PauseGameManager.Instance.register(this as IPausable, gameObject);
	}

	void OnDestroy () {
		PauseGameManager.Instance.remove(this as IPausable);
		ReloadableManager.Instance.remove(this as IReloadable);
	}
	
	/**
	 * Self implementation for destroy since using GamObject.Destroy() has a performance hit in android.
	 */
	private void destroy () {
		//GameObjectTools.ChipmunkBodyDestroy(body, shape);
		GameObjectTools.setActive(gameObject, false);
	}
	
	public bool DoNotResume {
		get {return doNotResume;}
		set {doNotResume = value;}
	}
	
	public void beforePause () {}
	
	public void afterResume () {}
	
	public bool isSceneOnly () {
		// used for allocation in subscriber lists managed by PauseGameManager
		return true;
	}
	
	private void die () {
		patrol.stopPatrol();
		idle.setIdle(true);
		dieAnim.die();
		Invoke("destroy", TIMING_DIE); // a replacement for Destroy with time
	}
	
	public void dieWhenFalling () {
		die();
	}
	
	public void onReloadLevel (Vector3 spawnPos) {
		GameObjectTools.setActive(gameObject, true);
		
		patrol.enablePatrol();
		patrol.setDir(1f); // initialize patrol direction
		idle.setIdle(false);
		
		transform.position = spawnPos;
		body._UpdatedTransform(); // update the body position
	}
	
	public static bool beginCollisionWithPowerUp (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		Goomba goomba = shape1.getOwnComponent<Goomba>();
		PowerUp powerUp = shape2.getOwnComponent<PowerUp>();
		
		if (goomba.dieAnim.isDying())
			return false; // avoid the collision to continue since this frame
		else {
			powerUp.Invoke("destroy", 0f); // a replacement for Destroy
			goomba.die();
		}
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next fixed step.
		return false;
	}
	
	public static bool beginCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		Goomba goomba = shape1.getOwnComponent<Goomba>();
		Player player = shape2.getOwnComponent<Player>();
		
		if (goomba.dieAnim.isDying() || player.isDying()) {
			arbiter.Ignore(); // avoid the collision to continue since this frame
			return false; // avoid the collision to continue since this frame
		}
		
		goomba.idle.setIdle(true);
		
		// if collides from top then kill the goomba
		if (GameObjectTools.isHitFromAbove(goomba.transform.position.y, shape2.body, arbiter)) {
			goomba.die();
			// makes the player jumps a little upwards
			player.forceJump();
		}
		// kills Player
		else {
			goomba.patrol.stopPatrol();
			arbiter.Ignore(); // avoid the collision to continue since this frame
			LevelManager.Instance.loseGame(true);
		}
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next fixed step.
		return true;
	}
}
