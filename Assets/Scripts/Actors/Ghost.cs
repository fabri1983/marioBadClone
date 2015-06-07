#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
#define UNITY_4_AND_LATER
#endif
using UnityEngine;

public class Ghost : MonoBehaviour, IPausable, IMortalFall {
	
	public float flySpeed = 6f;
	public float flyRange = 6f;
	
	private bool doNotResume;
	private Fly fly;
	private Chase chase;
	private ChipmunkBody body;
	private ChipmunkShape shape;
	private WalkAbs targetWalkComp;
	
	// Use this for initialization
	void Awake () {
		fly = GetComponent<Fly>();
		chase = GetComponent<Chase>();
		body = GetComponent<ChipmunkBody>();
		shape = GetComponent<ChipmunkShape>();
	}
	
	void Start () {
		body.Sleep();
		fly.setAutomaticFly(true, flyRange);
		fly.setSpeed(flySpeed);

		PauseGameManager.Instance.register(this as IPausable, gameObject);
	}
	
	void OnDestroy () {
		PauseGameManager.Instance.remove(this as IPausable);
	}
	
	/**
	 * Self implementation for destroy since using GamObject.Destroy() has a performance hit in android.
	 */
	private void destroy () {
		shape.enabled = false; // makes the shape to be removed from the space
		GameObjectTools.ChipmunkBodyDestroy(body);
#if UNITY_4_AND_LATER
		gameObject.SetActive(false);
#else
		gameObject.SetActiveRecursively(false);
#endif
		PauseGameManager.Instance.remove(this as IPausable);
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
		stop();
		destroy();
	}
	
	private void stop () {
		targetWalkComp = null;
		chase.stop();
		fly.stopFlying();
	}
	
	public void dieWhenFalling () {
		die();
	}
	
	// Update is called once per frame
	void Update () {
		// disable chasing if target is facing the ghost
		if (chase.getTarget() == null || isTargetFacingMe()) {
			stop();
			body.Sleep();
		}
		else {
			body.Activate();
			chase.enable();
			fly.fly();
		}
	}
	
	public bool isTargetFacingMe () {
		// if player is facing to the ghost then Dot product will be < 0. Only considers X coordinate
		Vector2 towardTarget = chase.getTarget().position - transform.position;
		if (targetWalkComp == null)
			targetWalkComp = chase.getTarget().GetComponent<WalkAbs>();
		return (targetWalkComp.getLookingDir().x * towardTarget.x) <= 2f; // give a little distant threshold
	}
	
	public static bool beginCollisionWithPowerUp (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		Ghost ghost = shape1.getOwnComponent<Ghost>();
		PowerUp powerUp = shape2.getOwnComponent<PowerUp>();
		
		powerUp.Invoke("destroy", 0f); // a replacement for Destroy
		ghost.die();
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next fixed step.
		return false;
	}
	
	public static bool beginCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		Ghost ghost = shape1.getOwnComponent<Ghost>();
		ghost.stop();
		
		// kills player
		arbiter.Ignore(); // avoid the collision to continue since this frame
		LevelManager.Instance.loseGame(true); // force die animation
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next fixed step.
		return false;
	}
}
