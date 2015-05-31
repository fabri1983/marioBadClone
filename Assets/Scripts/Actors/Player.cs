#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
#define UNITY_4_AND_LATER
#endif
using UnityEngine;

public class Player : MonoBehaviour, IPausable, IPowerUpAble, IMortalFall, IInCollisionCP {
	
	public float walkVelocity = 10f;
	public float lightJumpVelocity = 40f;
	public float gainJumpFactor = 1.4f;
	
	private Jump jump;
	private PlayerWalk walk;
	private PlayerDieAnim dieAnim;
	private Crouch crouch;
	private Idle idle;
	private Teleportable teleportable;
	private PowerUp powerUp;
	private LookDirections lookDirections;
	private bool exitedFromScenery;
	private ChipmunkSegmentQueryInfo qinfo;
	private ChipmunkBody body;
	private float walkVelBackup, signCollision;
	private bool doNotResume;
	private bool inCollision;
	
	/// the position where the bullets start firing
	private Transform firePivot;
	private Vector2 rightFireDir, leftFireDir, fireDir;
	
	// I guess 3.2f is half the size of Player's renderer plus few units more so the query ratio is the shortest possible
	private static Vector2 queryOffset = Vector2.up * -3.2f;
	private static string collisionGroupSkip;
	private static uint collisionLayers;
	
	private static Player instance = null;
	private static bool duplicated = false; // usefull to avoid onDestroy() execution on duplicated instances being destroyed
	
	public static Player Instance {
        get {
            if (instance == null) {
				instance = GameObject.FindObjectOfType(typeof(Player)) as Player;
				if (instance == null) {
					// instantiate the entire prefab. Don't assign to the instance variable because it is then assigned in Awake()
					GameObject.Instantiate(Resources.Load("Prefabs/Mario"));
				}
			}
            return instance;
        }
    }
	
	void Awake () {
		if (instance != null && instance != this) {
			duplicated = true;
			Destroy(gameObject);
		}
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
			initialize();
		}
	}
	
	void Start () {
		//resetPlayer(); // invoke after getting action components
		PauseGameManager.Instance.register(this as IPausable, gameObject);
	}
	
	private void initialize () {
		jump = GetComponent<Jump>();
		walk = GetComponent<PlayerWalk>();
		firePivot = transform.FindChild("FirePivot");
		teleportable = GetComponent<Teleportable>();
		dieAnim = GetComponent<PlayerDieAnim>();
		crouch = GetComponent<Crouch>();
		idle = GetComponent<Idle>();
		lookDirections = GetComponent<LookDirections>();
		body = GetComponent<ChipmunkBody>();
		
		walkVelBackup = walkVelocity;
		rightFireDir.x = 1f;
		rightFireDir.y = -0.5f;
		leftFireDir.x = -1f;
		leftFireDir.y = -0.5f;
		fireDir = rightFireDir;
		
		collisionGroupSkip = GetComponent<ChipmunkShape>().collisionGroup;
		collisionLayers = unchecked((uint)~(1 << gameObject.layer)); // all layers except Player's layer
	}
	
	void OnDestroy () {
		GameObjectTools.ChipmunkBodyDestroy(body);
		// this is to avoid nullifying or destroying static variables. Intance variables can be destroyed before this check
		if (duplicated) {
			duplicated = false; // reset the flag for next time
			return;
		}
		PauseGameManager.Instance.remove(this as IPausable);
		instance = null;
	}
	
	public bool InCollision {
		get {return inCollision;}
		set {inCollision = value;}
	}
	
	public bool DoNotResume {
		get {return doNotResume;}
		set {doNotResume = value;}
	}
	
	public void beforePause () {}
	
	public void afterResume () {}
	
	public bool isSceneOnly () {
		// used for allocation in subscriber lists managed by PauseGameManager
		return false;
	}
	
	// Update is called once per frame
	void Update () {
		
		// This sets the correct jump status when the player without jumping enters on free fall state.
		// Also corrects a sprite animation flickering when walking because the animation starts again 
		// constantly after jump.resetStatus()
		if (exitedFromScenery && !jump.IsJumping()) {
			// check if there is no shape below us
			Vector2 end = body.position + queryOffset;
			Chipmunk.SegmentQueryFirst(body.position, end, collisionLayers, collisionGroupSkip, out qinfo);
			// if no handler it means no hit
			if (System.IntPtr.Zero == qinfo._shapeHandle)
				// set state as if were jumping
				jump.resetStatus();
		}
		
		bool isIdle = true;
		
		// jump
		if (Gamepad.isA()) {
			walk.stopWalking(); // resets walk behavior
			jump.jump(lightJumpVelocity);
			// apply gain jump power. Only once per jump (handled in Jump component)
			if (Gamepad.isHardPressed(EnumButton.A))
				jump.applyGain(gainJumpFactor); 
			isIdle = false;
		}
		
		// power up action
		if (powerUp != null && powerUp.ableToUse())
			powerUp.action(gameObject);
		
		// move
		walk.enableWalking();
		if (Gamepad.isLeft()) {
			walk.walk(-walkVelocity);
			fireDir = leftFireDir;
			isIdle = false;
			// enable move speed after a wall collision if intended moving direction changes
			if (signCollision > 0f)
				restoreWalkVel();
		}
		else if (Gamepad.isRight()) {
			walk.walk(walkVelocity);
			fireDir = rightFireDir;
			isIdle = false;
			// enable move speed after a wall collision if intended moving direction changes
			if (signCollision < 0f)
				restoreWalkVel();
		}
		else
			// if no movement input then set correct internal status
			walk.stopWalking();

		// crouch
		if (Gamepad.isDown()) {
			crouch.crouch();
			isIdle = false;
		}
		else
			crouch.noCrouch();
		
		// look upwards/downwards
		if (!jump.IsJumping()) {
			if (Gamepad.isUp()) {
				lookDirections.lookUpwards();
				isIdle = false;
			}
			else
				lookDirections.restore();
		}
		else
			lookDirections.lockYWhenJumping();
		
		// finally only if not doing any action then set idle state
		if (isIdle)
			idle.setIdle(false);
	}
	
	public void dieWhenFalling () {
		dieAnim.restorePlayerProps();
		LevelManager.Instance.loseGame(false);
	}
	
	public void toogleEnabled (bool val) {
#if UNITY_4_AND_LATER
		gameObject.SetActive(val);
#else
		gameObject.SetActiveRecursively(val);
#endif
	}
	
	public void die () {
		// disable this componenet
		this.enabled = false;
		// dump to zero velocity
		Vector2 v = body.velocity;
		v.x =0;
		body.velocity = v;
		// do animation
		dieAnim.startAnimation();
	}
	
	public bool isDying () {
		return dieAnim.isDying();
	}
	
	public bool isJumping () {
		return jump.IsJumping();
	}
	
	public void forceJump () {
		// used when the player kills an enemy from above or similar situations
		jump.forceJump(lightJumpVelocity);
	}
	
	public void resetPlayer () {
		this.enabled = true; // enable Update() and OnGUI()
		if (teleportable != null)
			teleportable.teleportReset();
		setPowerUp(null);
		jump.resetStatus();
		walk.reset();
		body.velocity = Vector2.zero;
		walkVelocity = walkVelBackup;
	}
	
	public void setPowerUp (PowerUp pPowerUp) {
		powerUp = pPowerUp;
	}
	
	public Transform getFirePivot () {
		return firePivot;
	}
	
	public Vector3 getFireDir () {
		return fireDir;
	}
	
	public void restoreWalkVel () {
		walkVelocity = walkVelBackup;
	}

	public void locateAt (Vector2 pos) {
		body.position = pos;
	}

	public static bool beginCollisionWithScenery (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
		// The order of the arguments matches the order in the function name.
		arbiter.GetShapes(out shape1, out shape2);
		
		Player player = shape1.getOwnComponent<Player>();
		/*if (player.isDying())
			return false; // stop collision with scenery since this frame*/
		
		player.exitedFromScenery = false;
		
		// avoid ground penetration (Y axis)
		// NOTE: to solve this Chipmunk has the property collisionBias and/or minPenetrationForPenalty
		Vector2 thePos = player.body.position;
		float depth = arbiter.GetDepth(0);
		thePos.y -= depth;
		player.body.position = thePos;
		
		// if isn't a grounded surface then stop velocity and avoid getting inside the object
		if (GameObjectTools.isWallHit(arbiter)) {
			// get sign direction to know what offset apply to body
			player.signCollision = -Mathf.Sign(player.transform.position.x - shape2.transform.position.x);
			// set moving velocity close to 0 so player can't move against the wall but can change direction of movement
			player.walkVelocity = 0.001f;
			// move back to the contact point and a little more
			thePos = player.body.position;
			thePos.x += player.signCollision * (depth - 0.01f);
			player.body.position = thePos;
		}
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next fixed step.
		return true;
	}
	
	public static void endCollisionWithScenery (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
		// The order of the arguments matches the order in the function name.
		arbiter.GetShapes(out shape1, out shape2);
		
		shape1.getOwnComponent<Player>().exitedFromScenery = true;
	}
	
	public static bool beginCollisionWithUnlockSensor (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		Player player = shape1.getOwnComponent<Player>();
		player.restoreWalkVel();
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next fixed step.
		return false;
	}

	public static bool beginCollisionWithOneway (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
		// The order of the arguments matches the order in the function name.
		arbiter.GetShapes(out shape1, out shape2);

		// if collision starts from below then proceed to the oneway platform logic
		if (GameObjectTools.isHitFromBelow(arbiter)) {
			shape1.GetComponent<ClimbDownOnPlatform>().setTraversingUpwards(true);
			return true; // return true so the PreSolve condition continues
		}
		// collisiion from above, then player is on platform
		else {
			shape1.GetComponent<ClimbDownOnPlatform>().handleLanding();
		}

		// oneway platform logic was not met
		return false;
	}

	public static bool presolveCollisionWithOneway (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
		// The order of the arguments matches the order in the function name.
		arbiter.GetShapes(out shape1, out shape2);

		// if collision was from below then continue with oneway platform logic
		if (GameObjectTools.isHitFromBelow(arbiter)) {
			arbiter.Ignore();
			return false;
		}
		// if player wants to climb down (once it is over the platform) then disable the collision to start free fall
		if (shape1.GetComponent<ClimbDownOnPlatform>().isPullingDown()) {
			arbiter.Ignore();
			return false;
		}

		// let the collision happens
		return true;
	}

	public static void endCollisionWithOneway (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
		// The order of the arguments matches the order in the function name.
		arbiter.GetShapes(out shape1, out shape2);

		// If was traversing the platform from below then the player is over the platform.
		// Correct inner state is treated by the invoked method
		shape1.GetComponent<ClimbDownOnPlatform>().handleSeparation();
	}
}
