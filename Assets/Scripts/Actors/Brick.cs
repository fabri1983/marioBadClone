using UnityEngine;

public class Brick : MonoBehaviour {
	
	public int maxHits = 1;
	public bool isInvisible = false;
	public Transform unusableReplacement;
	
	private int hitCount;
	private float brickBottomPoint, brickHalfSizeY;
	private Vector3 vec3aux = new Vector3();

	// Use this for initialization
	void Start () {

		hitCount = maxHits;
		brickHalfSizeY = gameObject.renderer.bounds.size.y/2f + 1f;
		// used to check if Mario hit brick from below
		brickBottomPoint = transform.position.y - (renderer.bounds.size.y / 2);
	}
	
	void OnCollisionEnter (Collision collision) {
		
		executeHit(collision.collider);
	}
	
	public void executeHit (Collider collider) {
		
		if (!collider.tag.Equals("Mario") || hitCount == 0 || !hitFromBelow(collider.transform)) 
			return;
		
		--hitCount;
		
		// makes the player use this power up (if any)
		if (GetComponent<PowerUp>() != null) {
			
			PowerUp powUp = (PowerUp)GetComponent<PowerUp>();
			powUp.assignToCharacter(collider.gameObject.GetComponent<Player>());
			// perform a little animation to show what power up the player has got
			vec3aux.x = transform.position.x;
			vec3aux.y = transform.position.y + brickHalfSizeY;
			vec3aux.z = transform.position.z;
			powUp.doGotchaAnim(vec3aux);
		}
		
		// replace the current brick game object for a new one that simulate none behavior
		if (hitCount == 0 && unusableReplacement != null) {
			
			// instantiate the replacement game object which has a MonoBehavior script attached to it who is in charge of its behavior
			GameObject.Instantiate(unusableReplacement, transform.position, Quaternion.identity);
			destroyBrickGracefully();
		}
	}
	
	/**
	 * Determines whether the transform hit this game object's transform from below.
	 * Uses an epsilon margin of error.
	 */
	public bool hitFromBelow (Transform t) {
		
		float tTopPoint = (t.renderer.bounds.size.y / 2f) + t.transform.position.y;
		
		if ((tTopPoint - 0.2f) < brickBottomPoint)
			return true;

		return false;
	}
	
	private void destroyBrickGracefully () {
		gameObject.renderer.enabled = false;
		// if no power up attached then destroy it
		if (GetComponent<PowerUp>() == null)
			Destroy(this.gameObject, 0.1f);
		// don't destroy the object to avoid killing the PowerUp instance
		else {
			gameObject.collider.enabled = false;
			// disable the child ("top" game object)
#if UNITY_4_AND_LATER
			transform.FindChild("BrickTop").gameObject.SetActive(false);
#else
			transform.FindChild("BrickTop").gameObject.active = false;
#endif
		}
	}
}
