using UnityEngine;

public class Ghost : MonoBehaviour, IPausable {
	
	public float flySpeed = 6f;
	public float flyRange = 6f;
	
	private Fly fly;
	private Chase chase;
	private ChipmunkBody body;
	private ChipmunkShape shape;
	private float velScape;
	
	// Use this for initialization
	void Awake () {
		fly = GetComponent<Fly>();
		chase = GetComponent<Chase>();
		body = GetComponent<ChipmunkBody>();
		shape = GetComponent<ChipmunkShape>();
		
		PauseGameManager.Instance.register(this);
		velScape = Mathf.Sqrt(body.mass * -Chipmunk.gravity.y);
	}
	
	void Start () {
		fly.setAutomaticFly(true, flyRange);
		fly.setSpeed(flySpeed);
	}
	
	void OnDestroy () {
		PauseGameManager.Instance.remove(this);
	}
	
	/**
	 * Self implementation for destroy since using GamObject.Destroy() when running game since it has a performance hit in android.
	 */
	private void destroy () {
		shape.enabled = false; // makes the shape to be removed from the space
		GameObjectTools.ChipmunkBodyDestroy(body);
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
		chase.stopChasing();
		fly.stopFlying();
		destroy();
	}
	
	void FixedUpdate () {
		// avoid falling down when idle
		if (chase.getTarget() == null || isTargetFacingMe())
			cancelGravityForce();
	}
	
	// Update is called once per frame
	void Update () {
		
		// disable chasing if target is facing the ghost
		if (chase.getTarget() == null || isTargetFacingMe()) {
			fly.stopFlying();
			chase.stopChasing();
		}
		else {
			chase.enableChasing();
			fly.fly();
		}
	}
	
	private void cancelGravityForce () {

		Vector2 v = body.velocity;
		v.y = velScape * Time.fixedDeltaTime;
		body.velocity = v;
	}
	
	public bool isTargetFacingMe () {
		// if player is walking to the ghost then Dot product will be < 0
		Vector2 ghostVector = chase.getTarget().position - transform.position;
		return Vector2.Dot(chase.getTarget().GetComponent<WalkAbs>().getLookingDir(), ghostVector) <= 0f;
	}
	
	public static bool beginCollisionWithPowerUp (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		Ghost ghost = shape1.GetComponent<Ghost>();
		PowerUp powerUp = shape2.GetComponent<PowerUp>();
		
		if (powerUp.isLethal()) {
			powerUp.Invoke("destroy", 0f); // a replacement for Destroy
			ghost.die();
		}
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return false;
	}
	
	public static bool beginCollisionWithPlayer (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    arbiter.GetShapes(out shape1, out shape2);
		
		Ghost ghost = shape1.GetComponent<Ghost>();
		ghost.chase.stopChasing();
		ghost.fly.stopFlying();
		
		// kills Mario
		arbiter.Ignore(); // avoid the collision to continue since this frame
		LevelManager.Instance.loseGame(true); // force die animation
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return false;
	}
}
