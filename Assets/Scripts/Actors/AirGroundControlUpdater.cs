using UnityEngine;
using System.Collections;

public class AirGroundControlUpdater : MonoBehaviour {
	
	private static Vector2 GROUND_NORMAL = new Vector2(0f, -1f);
	private static Vector2 VECTOR_ZERO = Vector2.zero;
	
	public float fallSpeed = 100f;
	public float walkAccelTime = 0.05f;
	public float airAccelTime = 1f;
	
	private ChipmunkBody body;
	private ChipmunkShape shape;
	private float groundPenetration;
	private Vector2 recentGroundVelocity = VECTOR_ZERO;
	private float inputSign;
	
	private float walkSpeed = 0f;
	public void setWalkSpeed(float value) {
		walkSpeed = Mathf.Abs(value);
	}
	
	// is component touching a surface that is pointed upwards?
	private bool _grounded = false;
	public bool grounded {
		get { return _grounded; }
	}
	
	// is component grounded on a surface they won't slide down?
	public bool wellGrounded { get; protected set; }
	
	public Vector2 groundVelocity {
		get { return (grounded ? body.velocity - recentGroundVelocity : Vector2.zero); }
	}
	
	private float walkAccel {
		get { return walkSpeed/walkAccelTime; }
	}
	
	private float airAccel {
		get { return walkSpeed/airAccelTime; }
	}
	
	void Awake(){
		body = GetComponent<ChipmunkBody>();
		shape = GetComponent<ChipmunkShape>();
	}
	
	void Update () {
		inputSign = Gamepad.Instance.isLeft()? -1f : Gamepad.Instance.isRight()? 1f : 0;
	}
	
	void LateUpdate () {
		// add the groundPenetrationOffset to the position to force the graphics to not overlap.
		if (groundPenetration != 0f) {
			Vector3 thePos = transform.position;
			thePos.y += groundPenetration;
			transform.position = thePos;
			groundPenetration = 0f; // reset to avoid flickering up&down
		}
	}
	
	void FixedUpdate () {
		updateGrounding();
		updateMovement();
	}
	
	private void updateGrounding () {
		// reset the grounding values to defaults
		_grounded = false;
		Vector2 groundNormal = GROUND_NORMAL;
		groundPenetration = 0f;
		ChipmunkBody groundBody = null;
		ChipmunkShape groundShape = null;
		
		// find the best (most upright) normal of something you are standing on
		body.EachArbiter(delegate(ChipmunkArbiter arbiter){
			ChipmunkShape component, ground;
			arbiter.GetShapes(out component, out ground);
			Vector2 n = -arbiter.GetNormal(0);
			// magic threshold here to detect if you hit your head or not
			if (GameObjectTools.isCeiling(arbiter)) {
				// bumped your head, disable further jumping or whatever you need
			}
			else if (n.y > groundNormal.y) {
				_grounded = true;
				groundNormal = n;
				groundPenetration = -arbiter.GetDepth(0);
				groundBody = ground.body;
				groundShape = ground;
			}
		});
		
		// To be well grounded, the slope you are standing on needs to be less than the amount of friction
		float friction = _grounded ? shape.friction * groundShape.friction : 0f;
		wellGrounded = grounded && Mathf.Abs(groundNormal.x / groundNormal.y) < friction;
		if (wellGrounded)
			recentGroundVelocity = (groundBody != null ? groundBody.velocity : VECTOR_ZERO);
	}
	
	private void updateMovement () {
		float dt = Time.fixedDeltaTime;
		Vector2 v = body.velocity;
		Vector2 f = Vector2.zero;
		
		// Update the surface velocity
		Vector2 surface_v = shape.surfaceVelocity;
		surface_v.x = walkSpeed * inputSign;
		surface_v.y = 0f;
		shape.surfaceVelocity = surface_v;
		
		// Update the friction
		/*if(_grounded){
			shape.friction = -walkAccel/Chipmunk.gravity.y;
		} else {
			shape.friction = 0f;
		}*/
		
		// Apply air control if not grounded
		if(!_grounded){
			float accel_x = ((walkSpeed * inputSign) + recentGroundVelocity.x - v.x)/dt;
			f.x = Mathf.Clamp(accel_x, -airAccel, airAccel) * body.mass;
		}
		
		body.velocity = v;
		body.force = f;
	}
}
