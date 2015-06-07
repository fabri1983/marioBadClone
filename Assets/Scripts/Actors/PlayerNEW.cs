using UnityEngine;

public class PlayerNEW {
}
/*public class PlayerNEW : MonoBehaviour, IPausable, IPowerUpAble, IMortalFall {

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
	private ChipmunkShape shape;
	private ChipmunkBody body;
	private bool doNotResume;
	
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
		shape = GetComponent<ChipmunkShape>();
		body = GetComponent<ChipmunkBody>();
		
		rightFireDir.x = 1f;
		rightFireDir.y = -0.5f;
		leftFireDir.x = -1f;
		leftFireDir.y = -0.5f;
		fireDir = rightFireDir;
		
		collisionGroupSkip = GetComponent<ChipmunkShape>().collisionGroup;
		collisionLayers = unchecked((uint)(1 << gameObject.layer)); // not sure if ok: all layers except Player's layer
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
	
	public void resetPlayer () {
		this.enabled = true; // enable Update() and OnGUI()
		if (teleportable != null)
			teleportable.teleportReset();
		setPowerUp(null);
		jump.resetStatus();
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
	
	
	
	
	
	
	
	// Player is touching a surface that is pointed upwards.
	protected bool _grounded = false;
	public bool grounded {
		get { return (_grounded || remainingJumpLeniency > 0f); }
	}
	
	// How long the player has not been grounded.
	public float airTime { get; protected set; }
	
	// Send an "OnLand" message if landing from a jump longer than this.
	public float landThreshold = 0.25f;
	
	// Player is grounded on a surface they won't slide down.
	public bool wellGrounded { get; protected set; }
	
	protected Vector2 groundNormal;
	protected float groundPenetration;
	protected ChipmunkBody groundBody;
	protected ChipmunkShape groundShape;
	protected Vector2 groundImpulse;
	protected Vector2 recentGroundVelocity = Vector2.zero;
	
	public Vector2 groundPenetrationOffset {
		get { return groundNormal*(groundPenetration); }
	}
	
	public Vector2 groundVelocity {
		get { return (grounded ? body.velocity - recentGroundVelocity : Vector2.zero); }
	}
	
	protected void UpdateGrounding(){
		bool wasGrounded = _grounded;
		
		// Reset the grounding values to defaults
		_grounded = false;
		groundNormal = new Vector2(0f, -1f);
		groundPenetration = 0f;
		groundBody = null;
		groundShape = null;
		groundImpulse = Vector2.zero;
		
		// Find the best (most upright) normal of something you are standing on.
		body.EachArbiter(delegate(ChipmunkArbiter arbiter){
			ChipmunkShape player, ground; arbiter.GetShapes(out player, out ground);
			Vector2 n = -arbiter.GetNormal(0);
			
			// Magic thershold here to detect if you hit your head or not.
			if(n.y < -0.7f){
				// Bumped your head, disable further jumping.
				remainingAirJumps = 0;
				remainingBoost = 0f;
			} else if(n.y > groundNormal.y){
				_grounded = true;
				groundNormal = n;
				groundPenetration = -arbiter.GetDepth(0);
				groundBody = ground.body;
				groundShape = ground;
			}
			
			groundImpulse += arbiter.impulseWithFriction;
		});
		
		// If the player just landed from a significant jump, send a message.
		if(_grounded && !wasGrounded && airTime > landThreshold){
			SendMessage("OnLand");
		}
		
		// Increment airTime if the player is not grounded.
		airTime = (!_grounded ? airTime + Time.deltaTime : 0f);
		
		// To be well grounded, the slope you are standing on needs to less than the amount of friction
		float friction = _grounded ? shape.friction * groundShape.friction : 0f;
		wellGrounded = grounded && Mathf.Abs(groundNormal.x/groundNormal.y) < friction;
		if(wellGrounded){
			recentGroundVelocity = (groundBody != null ? groundBody.velocity : Vector2.zero);
			remainingAirJumps = maxAirJumps;
			remainingJumpLeniency = jumpLeniency;
		}
	}
	
	public float walkSpeed = 8f;
	public float runSpeed = 12;
	public float walkAccelTime = 0.05f;
	public float airAccelTime = 0.25f;
	public float fallSpeed = 5f;
	
	protected float walkAccel {
		get { return walkSpeed/walkAccelTime; }
	}
	
	protected float airAccel {
		get { return walkSpeed/airAccelTime; }
	}
	
	public int maxAirJumps = 1;
	public float jumpLeniency = 0.05f;
	public float jumpHeight = 1.0f;
	public float jumpBoostHeight = 2.0f;
	
	protected float remainingJumpLeniency = 0f;
	protected int remainingAirJumps = 0;
	protected bool lastJumpKeyState = false;
	protected float remainingBoost = 0f;
	
	private string jumpButton = "Jump";
	protected bool jumpInput {
		get {
			// Check both the virtual controls and the input buttons.
			return (
				(virtualControls && virtualControls.jump)
				|| Input.GetButton(jumpButton)
				);
		}
	}
	
	private string runButton = "Fire1";
	protected bool runInput {
		get { return Input.GetButton(runButton); }
	}
	
	private string directionAxis = "Horizontal";
	protected float directionInput {
		get {
			// Check both the virtual controls and the input buttons.
			return (virtualControls ? virtualControls.direction : 0f) + Input.GetAxis(directionAxis);
		}
	}
	
	protected void FixedUpdate(){
		float dt = Time.fixedDeltaTime;
		Vector2 v = body.velocity;
		Vector2 f = Vector2.zero;
		
		UpdateGrounding();
		
		// Target horizontal velocity used by air/ground control
		float target_vx = (runInput ? runSpeed : walkSpeed)*directionInput;
		
		// Update the surface velocity and friction
		Vector2 surface_v = new Vector2(target_vx, 0f);
		shape.surfaceVelocity = surface_v;
		if(_grounded){
			shape.friction = -walkAccel/Chipmunk.gravity.y;
		} else {
			shape.friction = 0f;
		}
		
		// Apply air control if not grounded
		if(!_grounded){
			float max = airAccel;
			float accel_x = (target_vx + recentGroundVelocity.x - v.x)/dt;
			f.x = Mathf.Clamp(accel_x, -max, max)*body.mass;
			//			v.x += Mathf.MoveTowards(v.x, target_vx + recentGroundVelocity.x, playerAirAccel*dt);
		}
		
		// If the jump key was just pressed this frame, jump!
		bool jumpState = jumpInput;
		bool jump = (jumpState && !lastJumpKeyState);
		//Input.GetButton(jumpButton)
		if(jump && (wellGrounded || remainingAirJumps > 0 || remainingJumpLeniency > 0f)){
			float jump_v = Mathf.Sqrt(-2f*jumpHeight*Chipmunk.gravity.y);
			remainingBoost = jumpBoostHeight/jump_v;
			
			// Apply the jump to the velocity.
			v.y = recentGroundVelocity.y + jump_v;
			
			
			// Check if it was an air jump.
			if(!wellGrounded && (remainingJumpLeniency <= 0f)) {
				remainingAirJumps--;
				
				// Force the jump direction for air jumps.
				// Otherwise difficult to air jump to a block directly over you.
				v.x = directionInput*walkSpeed;
				SendMessage("OnAirJump");
			}else{
				SendMessage("OnJump");
			}
		} else if(!jumpState){
			remainingBoost = 0f;
		}
		
		// Apply jump boosting.
		if(jumpState && remainingBoost > 0f){
			f -= body.mass*(Vector2)Chipmunk.gravity;
		}
		
		remainingJumpLeniency -= dt;
		remainingBoost -= dt;
		lastJumpKeyState = jumpState;
		
		// Clamp off the falling velocity.
		v.y = Mathf.Clamp(v.y, -fallSpeed, Mathf.Infinity);
		
		body.velocity = v;
		body.force = f;
	}
}*/
