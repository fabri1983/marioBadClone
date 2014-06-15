using UnityEngine;

public class Player : MonoBehaviour, IPowerUpAble {
	
	public float walkVelocity = 10f;
	public float lightJumpVelocity = 40f;
	public float gainJumpFactor = 1.4f;
	
	// Actions scripts
	private Jump jump;
	private PlayerWalk walk;
	private PlayerDieAnim dieAnim;
	private Crouch crouch;
	private Idle idle;
	private Teleportable teleportable;
	
	/// powerUp object
	private PowerUp powerUp;
	/// the position where the bullets start firing
	private Transform firePivot;
	private Vector3 rightFireDir, leftFireDir, fireDir;
	
	private ChipmunkBody body;
	private float walkVelBackup, signCollision;
	
	private static Player instance = null;
	
	public static Player Instance {
        get {
            if (instance == null) {
				instance = GameObject.FindObjectOfType(typeof(Player)) as Player;
				if (instance == null)
					// instantiate the entire prefab. Don't assign to the instance variable because it is then assigned in Awake()
					GameObject.Instantiate(Resources.Load("Prefabs/Mario"));
			}
            return instance;
        }
    }
	
	void Awake () {
		// in case the game object wasn't instantiated yet from another script
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		// deactivate to avoid falling in empty scene
		gameObject.SetActiveRecursively(false);
		
		// action components
		jump = GetComponent<Jump>();
		walk = GetComponent<PlayerWalk>();
		firePivot = transform.FindChild("FirePivot");
		teleportable = GetComponent<Teleportable>();
		dieAnim = GetComponent<PlayerDieAnim>();
		crouch = GetComponent<Crouch>();
		idle = GetComponent<Idle>();
		
		body = GetComponent<ChipmunkBody>();
		
		walkVelBackup = walkVelocity;
		rightFireDir = new Vector3(1f, -0.5f, 0f);
		leftFireDir = new Vector3(-1f, -0.5f, 0f);
		fireDir = rightFireDir;
	}
	
	// Use this for initialization
	void Start () {
		// invoke after having got action components
		resetPlayer();
	}
	
	void OnDestroy () {
		if (body != null) {
			body.enabled = false;
			// registering a disable body will remove it from the list
			ChipmunkInterpolationManager._Register(body);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (teleportable == null || !teleportable.isTeleporting()) {

			bool isIdle = true;
			
			// jump
			if (Gamepad.isA() || Input.GetButton("Jump")) {
				jump.jump(lightJumpVelocity);
				// apply gain jump power. Only once per jump (handled in Jump component)
				if (Gamepad.isHardPressed(Gamepad.BUTTONS.A))
					jump.applyGain(gainJumpFactor); 
				isIdle = false;
			}
			
			// power up action
			if (powerUp != null && powerUp.ableToUse())
				powerUp.action(gameObject);
			
			// move
			walk.enableWalking();
			if (Gamepad.isLeft() || Input.GetAxis("Horizontal") < -0.1f) {
				walk.walk(-walkVelocity);
				fireDir = leftFireDir;
				isIdle = false;
				// enable move speed after a wall collision if intended moving direction changes
				if (signCollision > 0f)
					restoreWalkVel();
			}
			else if (Gamepad.isRight() || Input.GetAxis("Horizontal") > 0.1f) {
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
			if (Gamepad.isDown() || Input.GetAxis("Vertical") < -0.1f) {
				crouch.crouch();
				isIdle = false;
			}
			
			if (isIdle)
				idle.setIdle(false);
		}
		
		// lose game if Player falls beyond a theshold
		if (transform.position.y <= LevelManager.ENDING_DIE_ANIM_Y_POS) {
			LevelManager.Instance.loseGame(false);
			return;
		}
	}
	
	/*########################### CLASS/INSTANCE METHODS #######################*/
	
	public void die () {
		this.enabled = false;
		dieAnim.startAnimation();
	}
	
	public bool isDying () {
		return dieAnim.isDying();
	}
	
	public void resetPlayer () {
		this.enabled = true; // enable Update() and OnGUI()
		if (teleportable != null)
			teleportable.teleportReset();
		setPowerUp(null);
		jump.reset();
		walk.reset();
		body.ResetForces();
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
	
	private void restoreWalkVel () {
		walkVelocity = walkVelBackup;
	}
	
	public static bool beginCollisionWithScenery (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		Player player = shape1.GetComponent<Player>();
		if (player.isDying())
			return false; // stop collision with scenery since this frame
		
		// if isn't a grounded surface then stop velocity and avoid getting inside the object
		if (GameObjectTools.isWallHit(arbiter)) {
			// get sign direction to know what offset apply to body
			player.signCollision = -Mathf.Sign(player.transform.position.x - shape2.transform.position.x);
			// set moving velocity close to 0 so player can't move against the wall but can change direction of movement
			player.walkVelocity = 0.001f;
			// move back to the contact point and a little more
			Vector2 thePos = player.body.position;
			thePos.x += player.signCollision * (arbiter.GetDepth(0) - 0.01f);
			player.body.position = thePos;
		}
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return true;
	}
	
	public static void endCollisionWithScenery (ChipmunkArbiter arbiter) {
		/*ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		Player player = shape1.GetComponent<Player>();
		if (player.isDying())
			return; // do nothing*/
	}
	
	public static bool beginCollisionWithUnlockSensor (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		Player player = shape1.GetComponent<Player>();
		if (!player.isDying())
			player.restoreWalkVel();
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return false;
	}
}
