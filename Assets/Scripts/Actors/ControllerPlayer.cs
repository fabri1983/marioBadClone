using UnityEngine;

public class ControllerPlayer : MonoBehaviour, IPowerUpAble {
	
	public float movementPow = 10f;
	public float lightJumpPow = 12f;
	public float gainJumpFactor = 1f;
	public float fallPow = 45f;
	public float diePosY = -20f;
	
	// Actions scripts
	private Jump jump;
	private MarioMove move;
	private MarioDieAnim dieAnim;
	private Crouch crouch;
	private Idle idle;
	
	// teleportable script
	private Teleportable teleportable;
	
	/// powerUp object
	private PowerUp powerUp;
	/// the position where the bullets will be thrown
	private Transform firePivot;
	private Vector3 rightFireDir, leftFireDir, fireDir;
	
	private static ControllerPlayer instance = null;
	
	public static ControllerPlayer Instance {
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
		move = GetComponent<MarioMove>();
		firePivot = transform.FindChild("FirePivot");
		teleportable = GetComponent<Teleportable>();
		dieAnim = GetComponent<MarioDieAnim>();
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
				jump.jump(lightJumpPow, fallPow);
				// apply gain jump power. Only once per jump (handled in Jump component)
				if (Gamepad.isHardPressed(Gamepad.BUTTONS.A))
					jump.applyFactor(gainJumpFactor); 
				isIdle = false;
			}
			
			// power up action
			if (powerUp != null && powerUp.ableToUse())
				powerUp.action(gameObject);
			
			// move
			if (Gamepad.isLeft() || Input.GetKey(KeyCode.LeftArrow)) {
				move.move(-movementPow);
				fireDir = leftFireDir;
				isIdle = false;
			}
			else if (Gamepad.isRight() || Input.GetKey(KeyCode.RightArrow)) {
				move.move(movementPow);
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
		if (transform.position.y <= diePosY) {
			LevelManager.Instance.loseGame(false);
			return;
		}
	}
	
	/*########################### CLASS/INSTANCE METHODS #######################*/
	
	public void die () {
		dieAnim.startAnimation();
	}
	
	public void resetPlayer () {
		if (teleportable != null)
			teleportable.teleportReset();
		setPowerUp(null);
		jump.reset();
		idle.setIdle(true);
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
}
