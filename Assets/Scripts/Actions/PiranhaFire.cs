using UnityEngine;
	
public class PiranhaFire : MonoBehaviour {
	
	public GameObject fireBall;
	public float rateOfFire = 1f;
	
	private int quad; // the correct value is set on PiranhaLookAnim
	private PiranhaLookAnim piranhaLookAnim;
	private PiranhaAnim piranhaAnim;
	private PiranhaSafeZone piranhaSafeZone;
	private PiranhaFireDetector rangeFiredetector;
	private float timingFire;
	private float firePow = 6f;
	private float destroyTime = 3f; // destroy the fireball after certain seconds since instantiated
	
	// fire direction per quadrant, in ccw order
	private static Vector3[] fireDirs = new Vector3[]{ 
		new Vector3(-1f, -1f, 0f),	// 00 
		new Vector3(1f, -1f, 0f),	// 01
		new Vector3(1f, 1f, 0f),	// 11
		new Vector3(-1f, 1f, 0),	// 10
	};
	
	// Use this for initialization
	void Start () {
		
		piranhaAnim = transform.parent.GetComponent<PiranhaAnim>();
		piranhaLookAnim = transform.parent.GetComponent<PiranhaLookAnim>();
		piranhaSafeZone = transform.parent.FindChild("SafeZone").GetComponent<PiranhaSafeZone>();
		//piranhaDeadZone = transform.parent.GetComponent<PiranhaDeadZone>();
		rangeFiredetector = transform.parent.FindChild("FireTrigger").GetComponent<PiranhaFireDetector>();
		timingFire = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (timingFire > 0)
			timingFire -= Time.deltaTime;
		
		// avoid fire if Mario is aout of range or is inside safe zone
		if (rangeFiredetector.getTarget() == null || piranhaSafeZone.isInside())
			piranhaAnim.enabled = true;
		
		// get current piranha fire
		if (piranhaLookAnim.getCurrentPiranhaFire() == null)
			return;
		
		// check if this doesn't match the active piranha fire
		PiranhaFire current = piranhaLookAnim.getCurrentPiranhaFire().GetComponent<PiranhaFire>();
		if (current.quad != quad)
			return;
		
		// only fires when target is in radio of fire, piranha is idle and totally out of tube, and target isn't in dead zone
		if ((rangeFiredetector.getTarget() != null) && piranhaAnim.isOut() && !piranhaSafeZone.isInside()) {
			// disable piranha anim so it stays out of the tube for firing
			piranhaAnim.enabled = false;
			// controls the rate of fire
			if (timingFire <= 0f) {
				fire(current);
				timingFire = rateOfFire;
			}
		}
		
	}
	
	private void fire (PiranhaFire currentPiranhaFire) {
		
		Vector3 dirFire = Vector3.zero;
		// get FirePivot position
		Vector3 firePos = currentPiranhaFire.transform.FindChild("FirePivot").position;
		
		if (currentPiranhaFire.quad == 00) {
			dirFire = fireDirs[0];
		}
		else if (currentPiranhaFire.quad == 01) {
			dirFire = fireDirs[1];
		}
		else if (currentPiranhaFire.quad == 11) {
			dirFire = fireDirs[2];
		}
		else if (currentPiranhaFire.quad == 10) {
			dirFire = fireDirs[3];
		}
		
		GameObject newGO = GameObject.Instantiate(fireBall, firePos, Quaternion.identity) as GameObject;
		newGO.active = false; // so it doesn't interact with current game object creator
		FireBall fireball = newGO.GetComponent<FireBall>();
		fireball.setDestroyTime(destroyTime);
		fireball.setBouncing(false);
		fireball.setDir(dirFire);
		fireball.setSpeed(firePow);
		fireball.addTargetLayer(LevelManager.LAYER_PLAYER);
		newGO.transform.parent = null;
		newGO.active = true;
	}
	
	public void setQuad (int val) {
		quad = val;
	}
}
