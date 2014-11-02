using UnityEngine;

public class Player : MonoBehaviour, IPowerUpAble, IPausable, IMortalFall {
	
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
	private LookUpwards lookUpwards;
	private bool ableToPause;
	private bool exitedFromScenery;
	
	/// the position where the bullets start firing
	private Transform firePivot;
	private Vector2 rightFireDir, leftFireDir, fireDir;
	
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
		if (instance != null && instance != this)
			Destroy(this.gameObject);
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		
		PauseGameManager.Instance.register(this);
		
		// deactivate to avoid falling in empty scene
		setActive(false);
		
		// action components
		jump = GetComponent<Jump>();
		walk = GetComponent<PlayerWalk>();
		firePivot = transform.FindChild("FirePivot");
		teleportable = GetComponent<Teleportable>();
		dieAnim = GetComponent<PlayerDieAnim>();
		crouch = GetComponent<Crouch>();
		idle = GetComponent<Idle>();
		lookUpwards = GetComponent<LookUpwards>();
		body = GetComponent<ChipmunkBody>();
		
		walkVelBackup = walkVelocity;
		rightFireDir.x = 1f;
		rightFireDir.y = -0.5f;
		leftFireDir.x = -1f;
		leftFireDir.y = -0.5f;
		fireDir = rightFireDir;
	}
	
	// Use this for initialization
	void Start () {
		// invoke after getting action components
		resetPlayer();
	}
	
	void OnDestroy () {
		GameObjectTools.ChipmunkBodyDestroy(body);
		PauseGameManager.Instance.remove(this);
		instance = null;
	}
	
	public void pause () {
		if (ableToPause)
			gameObject.SetActiveRecursively(false);
	}
	
	public void resume () {
		if (ableToPause)
			gameObject.SetActiveRecursively(true);
	}
	
	public bool isSceneOnly () {
		// used for allocation in subscriber lists managed by PauseGameManager
		return false;
	}
	
	// Update is called once per frame
	void Update () {
		
		// this set the correct jump status when the player without jumping enters on free fall state
		if (exitedFromScenery && !jump.IsJumping()) {
			// check if there is no shape below us
			ChipmunkSegmentQueryInfo qinfo;
			Vector2 end = body.position + Vector2.up * -3.2f;
			Chipmunk.SegmentQueryFirst(body.position, end, (uint)(1 << gameObject.layer), "", out qinfo);
			// if no handler it means no hit
			if (System.IntPtr.Zero == qinfo._shapeHandle)
				jump.resetStatus();
		}
		
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
		else
			crouch.noCrouch();
		
		// look upwards
		if ((Gamepad.isUp() || Input.GetAxis("Vertical") > 0.1f) && !walk.isWalking() && !jump.IsJumping()) {
			lookUpwards.lookUpwards();
			isIdle = false;
		}
		else
			lookUpwards.stop();
		
		if (isIdle)
			idle.setIdle(false);
	}
	
	public void dieWhenFalling () {
		dieAnim.restorePlayerProps();
		LevelManager.Instance.loseGame(false);
	}
	
	public void setActive (bool active) {
		gameObject.SetActiveRecursively(active);
		ableToPause = active;
	}
	
	public void die () {
		this.enabled = false;
		Vector2 v = body.velocity;
		v.x =0;
		body.velocity = v;
		dieAnim.startAnimation();
	}
	
	public bool isDying () {
		return dieAnim.isDying();
	}
	
	public void forceJump () {
		// used when the player kills an enemy from above
		jump.forceJump(0f); // this set the jumping state as true
		Vector2 v = body.velocity;
		v.y = lightJumpVelocity;
		body.velocity = v;
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
	
	public static bool beginCollisionWithScenery (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		Player player = shape2.GetComponent<Player>();
		/*if (player.isDying())
			return false; // stop collision with scenery since this frame*/
		
		player.exitedFromScenery = false;
		
		// avoid ground penetration (Y axis)
		// NOTE: to solve this Chipmunk has the property collisionBias and/or minPenetrationForPenalty
		Vector2 thePos = player.body.position;
		thePos.y += -arbiter.GetDepth(0);
		player.body.position = thePos;
		
		// if isn't a grounded surface then stop velocity and avoid getting inside the object
		if (GameObjectTools.isWallHit(arbiter)) {
			// get sign direction to know what offset apply to body
			player.signCollision = -Mathf.Sign(player.transform.position.x - shape1.transform.position.x);
			// set moving velocity close to 0 so player can't move against the wall but can change direction of movement
			player.walkVelocity = 0.001f;
			// move back to the contact point and a little more
			thePos = player.body.position;
			thePos = player.body.position;
			thePos.x += player.signCollision * (arbiter.GetDepth(0) - 0.01f);
			player.body.position = thePos;
		}
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return true;
	}
	
	public static void endCollisionWithScenery (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		shape2.GetComponent<Player>().exitedFromScenery = true;
	}
	
	public static bool beginCollisionWithUnlockSensor (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		Player player = shape1.GetComponent<Player>();
		player.restoreWalkVel();
		
		// Returning false from a begin callback means to ignore the collision response for these two colliding shapes 
		// until they separate. Also for current frame. Ignore() does the same but next frame.
		return false;
	}
}
