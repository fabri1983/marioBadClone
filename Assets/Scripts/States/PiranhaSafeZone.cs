using UnityEngine;

public class PiranhaSafeZone : MonoBehaviour {
	
	private PiranhaAnim piranhaAnim;
	private bool inside;
	
	// Use this for initialization
	void Start () {
		piranhaAnim = transform.parent.GetComponent<PiranhaAnim>();
		inside = false;
	}
	
	void OnTriggerEnter (Collider collider) {

		if (collider.tag.Equals("Mario")) {
			piranhaAnim.setKeepHide(true);
			inside = true;
		}
	}
	
	void OnTriggerExit (Collider collider) {

		if (collider.tag.Equals("Mario")) {
			piranhaAnim.setKeepHide(false);
			inside = false;
		}
	}
	
	public bool isInside () {
		return inside;
	}
}
