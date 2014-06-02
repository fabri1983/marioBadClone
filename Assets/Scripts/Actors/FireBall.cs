using UnityEngine;

public class FireBall : MonoBehaviour {
	
	private Vector3 dir = Vector3.zero;
	private Bounce bounce = null;
	private float torqueGrades = 28f;
	
	void Awake () {
		bounce = GetComponent<Bounce>();
	}
	
	void Update () {
		
		// move the game object in a rectiline direction until first hit occurs
		if (!bounce.firstCollision)
			transform.Translate(dir.x * Time.deltaTime, dir.y * Time.deltaTime,	0f, Space.World);
		// after first bounce occurred continue translating only in X axis
		else
			transform.Translate(dir.x * Time.deltaTime, 0f,	0f, Space.World);
	}
	
	void OnCollisionEnter (Collision collision) {

		// if it hits Mario then kill it
		if (collision.transform.tag.Equals("Mario")) {
			LevelManager.Instance.loseGame(true);
			Destroy(gameObject);
		}
		// only change X axis direction if the fireball can bounce
		else if (bounce.canBounce){
			// if normal.x is near to 1 it means it is a hit against something perpendicular to floor (a wall)
			float absX = Mathf.Abs(collision.contacts[0].normal.x);
			if (absX > 0.8f) {
				dir.x *= -1f;
				applyTorque();
			}
			bounce.collision(collision);
		}
	}
	
	private void applyTorque () {
		if (dir.x < 0)
			rigidbody.AddTorque(0f, 0f, torqueGrades);
		if (dir.x > 0)
			rigidbody.AddTorque(0f, 0f, -torqueGrades);
	}
	
	public void setDir (Vector3 pDir) {
		dir = pDir;
		applyTorque();
	}
	
	public void setDoBouncing (bool val) {
		bounce.canBounce = val;
	}
	
	public void setDestroyTime(float time) {
		if (time > 0f)
			Destroy(gameObject, time);
	}
	
	public void setHitForEnemy (bool val) {
		if (val)
			gameObject.layer = LevelManager.POWERUP_LAYER;
		else
			gameObject.layer = LevelManager.ONLY_WITH_MARIO_LAYER;
	}
}
