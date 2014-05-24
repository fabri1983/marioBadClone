using UnityEngine;

public class GoombaDieAnim : MonoBehaviour {
	
	private Idle idle;
	private Chase chase;
	private bool dying;
	private float halfHeigth;
	private static Vector3 CRUNCH_VECTOR = new Vector3(0f, 0.8f, 0f);
	private static float TIMING_DIE = 0.25f;
	
	void Start () {
		dying = false;
		halfHeigth = transform.GetChild(0).renderer.bounds.size.y / 2f;
		// get the reference of every script involved in the goomba life cycle
		idle = transform.GetComponent<Idle>();
		chase = transform.GetComponent<Chase>();
	}
	
	public void die () {
		// avoid re dying
		if (dying)
			return;
		
		dying = true;
		if (chase != null)
			chase.stopChasing();
		idle.setIdle(true);
		transform.Translate(0f, -halfHeigth, 0f);
		transform.localScale -= CRUNCH_VECTOR;
		GameObject.Destroy(this.gameObject, TIMING_DIE);
	}
	
	public bool isDying () {
		return dying;
	}
}
