using UnityEngine;

public class Player : MonoBehaviour, IPowerUpAble {
	
	public float moveVelocity = 10f;
	public float lightJumpVelocity = 40f;
	public float gainJumpFactor = 1.4f;
	
	// Actions scripts
	private Jump jump;
	private PlayerMove move;
	private PlayerDieAnim dieAnim;
	private Crouch crouch;
	private Idle idle;
	private Teleportable teleportable;
	/// powerUp object
	private PowerUp powerUp;
	/// the position where the bullets start firing
	private Transform firePivot;
	private Vector3 rightFireDir, leftFireDir, fireDir;
	
	private static Player instance = null;
	
	public static Player Instance {
        get {
            if (instance == null) {
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
		gameObject.active = false;
		
		// action components
		jump = GetComponent<Jump>();
		move = GetComponent<PlayerMove>();
		firePivot = transform.FindChild("FirePivot");
		teleportable = GetComponent<Teleportable>();
		dieAnim = GetComponent<PlayerDieAnim>();
		crouch = GetComponent<Crouch>();
		idle = GetComponent<Idle>();
		
		rightFireDir = new Vector3(1f, -0.5f, 0f);
		leftFireDir = new Vector3(-1f, -0.5f, 0f);
		fireDir = rightFireDir;
	}
	
	// Use this for initialization
	void Start () {
		// invoke after getting action components
		resetPlayer();
	}
	
	// Update is called once per frame
	void Update () {

		if (teleportable == null || !teleportable.isTeleporting() || !dieAnim.isDying()) {
			
			bool isIdle = true;
			
			// jump
			if (Gamepad.isA() || Input.GetKey(KeyCode.Space)) {
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
			if (Gamepad.isLeft() || Input.GetKey(KeyCode.LeftArrow)) {
				move.move(-moveVelocity);
				fireDir = leftFireDir;
				isIdle = false;
			}
			else if (Gamepad.isRight() || Input.GetKey(KeyCode.RightArrow)) {
				move.move(moveVelocity);
				fireDir = rightFireDir;
				isIdle = false;
			}
			// move script's internal status is modified when moving or in idle, and crouch needs to know realtime status
			else
				move.stopMoving();
			
			// crouch
			if (Gamepad.isDown() || Input.GetKey(KeyCode.DownArrow)) {
				crouch.crouch();
				isIdle = false;
			}
			
			if (isIdle)
				idle.setIdle(false);
		}
		
		// reset game if Mario falls
		if (transform.position.y <= LevelManager.ENDING_DIE_ANIM_Y_POS) {
			LevelManager.Instance.loseGame(false);
			return;
		}
	}
	
	/*########################### CLASS/INSTANCE METHODS #######################*/
	
	public void die () {
		dieAnim.startAnimation();
	}
	
	public bool isDying () {
		return dieAnim.isDying();
	}
	
	public void resetPlayer () {
		if (teleportable != null)
			teleportable.teleportReset();
		setPowerUp(null);
		jump.reset();
		idle.setIdle(true);
		ChipmunkBody body = GetComponent<ChipmunkBody>();
		body.ResetForces();
		body.velocity = Vector2.zero;
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
	
	public static bool beginCollisionWithScenery (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
		
		Player player = shape1.GetComponent<Player>();
		if (player.isDying())
			return false; // stop collision with scenery
		
		// if isn't a grounded surface then stop velocity
		
		// Returning false from a begin callback means to ignore the collision
	    // response for these two colliding shapes until they separate.
		return true;
	}
	
	public static void endCollisionWithScenery (ChipmunkArbiter arbiter) {
		ChipmunkShape shape1, shape2;
	    // The order of the arguments matches the order in the function name.
	    arbiter.GetShapes(out shape1, out shape2);
	}
}
