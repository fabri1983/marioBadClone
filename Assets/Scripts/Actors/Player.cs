using UnityEngine;

public class Player : MonoBehaviour, IPausable, IPowerUpAble, IMortalFall, IReloadable {
	
	private static Player instance = null;
	private static bool duplicated = false; // usefull to avoid onDestroy() execution on duplicated instances being destroyed
	
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
	private ChipmunkBody body;
	private bool doNotResume;
	
	/// the position where the bullets start firing
	private Transform firePivot;
	private Vector2 rightFireDir, leftFireDir, fireDir;
	
	/// used for downwards ray casting
	private bool exitedFromScenery;
	private Vector2 queryOffset = Vector2.up * -3.2f;
	private string collisionGroupSkip;
	private uint collisionLayersSkip;
	
	public static Player Instance {
        get {
            if (instance == null) {
				instance = GameObject.FindObjectOfType(typeof(Player)) as Player;
				if (instance == null) {
					// instantiate the entire prefab. Don't assign to the instance variable because it is then assigned in Awake()
					GameObject.Instantiate(Resources.Load(KResources.MARIO));
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
		PauseGameManager.Instance.register(this as IPausable, gameObject);
		ReloadableManager.Instance.register(this as IReloadable, transform.position);
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
		
		rightFireDir.x = 1f;
		rightFireDir.y = -0.5f;
		leftFireDir.x = -1f;
		leftFireDir.y = -0.5f;
		fireDir = rightFireDir;
		
		collisionGroupSkip = GetComponent<ChipmunkShape>().collisionGroup;
		// not sure if ok: all layers except Player's layer
		collisionLayersSkip = unchecked((uint)(1 << KLayers.PLAYER));
		//collisionLayers = 0;
	}
	
	void OnDestroy () {
		GameObjectTools.ChipmunkBodyDestroy(body, GetComponent<ChipmunkShape>());
		// this is to avoid nullifying or destroying static variables. Intance variables can be destroyed before this check
		if (duplicated) {
			duplicated = false; // reset the flag for next time
			return;
		}
		PauseGameManager.Instance.remove(this as IPausable);
		instance = null;
	}
	
	public void onReloadLevel (Vector3 pos) {
		enabled = true; // enable Update() and OnGUI()
		if (teleportable != null)
			teleportable.reset();
		setPowerUp(null);
		jump.reset();
		if (walk.isLookingRight())
			fireDir = rightFireDir;
		else
			fireDir = leftFireDir;
		walk.reset();
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
		bool isIdle = true;
		
		// This sets the correct jump status when the player without jumping enters on free fall state.
		// Also corrects a sprite animation flickering when walking because the animation starts again 
		// constantly after jump.resetStatus()
		if (exitedFromScenery && !jump.isJumping()) {
			ChipmunkSegmentQueryInfo qinfo;
			// check if there is no shape below us
			Vector2 end = body.position + queryOffset;
			Chipmunk.SegmentQueryFirst(body.position, end, collisionLayersSkip, collisionGroupSkip, out qinfo);
			// if no handler it means no hit
			if (System.IntPtr.Zero == qinfo._shapeHandle) {
				jump.reset(); // set state as if were jumping
			}
		}
		
		// jump
		if (Gamepad.Instance.isA()) {
			walk.stop();
			jump.jump(lightJumpVelocity);
			// apply gain jump power. Only once per jump (handled in Jump component)
			if (Gamepad.Instance.isHardPressed(EnumButton.A))
				jump.applyGain(gainJumpFactor); 
			isIdle = false;
		}
		
		// power up action
		if (powerUp != null && powerUp.ableToUse())
			powerUp.action(gameObject);
		
		// walk
		if (Gamepad.Instance.isLeft()) {
			// is speed up button being pressed?
			if (Gamepad.Instance.isB()) walk.setGain(walk.speedUpFactor);
			else walk.setGain(1f);
			walk.walk(-walkVelocity);
			fireDir = leftFireDir;
		}
		else if (Gamepad.Instance.isRight()) {
			// is speed up button being pressed?
			if (Gamepad.Instance.isB()) walk.setGain(walk.speedUpFactor);
			else walk.setGain(1f);
			walk.walk(walkVelocity);
			fireDir = rightFireDir;
		}
		if (walk.isWalking())
			isIdle = false;

		// crouch
		if (Gamepad.Instance.isDown()) {
			crouch.crouch();
			isIdle = false;
		}
		else
			crouch.noCrouch();
		
		// look upwards/downwards
		if (!jump.isJumping()) {
			if (Gamepad.Instance.isUp()) {
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
	
	public void die () {
		// disable this componenet
		this.enabled = false;
		// do animation
		dieAnim.startAnimation();
	}
	
	public bool isDying () {
		return dieAnim.isDying();
	}
	
	public bool isJumping () {
		return jump.isJumping();
	}
	
	/// <summary>
	/// Used when the player kills an enemy from above or similar situations
	/// </summary>
	public void forceJump () {
		jump.forceJump(lightJumpVelocity);
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

	public void locateAt (Vector2 pos) {
		transform.position = pos;
		body._UpdatedTransform(); // update the body position
	}

	public static bool beginCollisionWithScenery (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
		// The order of the arguments matches the order in the function name.
		arbiter.GetShapes(out shape1, out shape2);
		
		Player player = shape1.getOwnComponent<Player>();
		if (player.isDying())
			return false; // stop collision with scenery since this frame
		
		player.exitedFromScenery = false;
		
		// Avoid ground penetration (Y axis). Another way: see collisionBias and/or minPenetrationForPenalty config in CollisionManagerCP.
		// Currently is being addressed by AirGroundControlUpdater and in WalkAbs
		/*if (GameObjectTools.isGrounded(arbiter)) {
			Vector2 thePos = player.body.position;
	        float depth = arbiter.GetDepth(0);
			thePos.y -= depth;
	        player.body.position = thePos;
		}*/
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next fixed step.
		return true;
	}
	
	public static void endCollisionWithScenery (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
		// The order of the arguments matches the order in the function name.
		arbiter.GetShapes(out shape1, out shape2);
		
		Player player = shape1.getOwnComponent<Player>();
		
		player.exitedFromScenery = true;
	}

	public static bool beginCollisionWithOneway (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
		// The order of the arguments matches the order in the function name.
		arbiter.GetShapes(out shape1, out shape2);

		// if collision starts from below then proceed to the oneway platform logic
		if (GameObjectTools.isHitFromBelow(arbiter)) {
			shape1.GetComponent<ClimbDownFromPlatform>().setTraversingUpwards(true);
			return true; // return true so the PreSolve condition continues
		}
		// collisiion from above, then player is on platform
		else {
			shape1.GetComponent<ClimbDownFromPlatform>().handleLanding();
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
		
		Player player = shape1.getOwnComponent<Player>();
		// if player wants to climb down (once it is over the platform) then disable the collision to start free fall
		if (player.GetComponent<ClimbDownFromPlatform>().isClimbingDown()) {
			player.jump.reset(); // set state as if were jumping
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

		Player player = shape1.getOwnComponent<Player>();
		//player.jump.resetStatus(); // set state as if were jumping
		player.exitedFromScenery = true;
		
		// If was traversing the platform from below then the player is over the platform.
		// Correct inner state is treated by the invoked method
		player.GetComponent<ClimbDownFromPlatform>().handleSeparation();
	}
}
