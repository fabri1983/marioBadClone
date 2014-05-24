using UnityEngine;

public class PiranhaLookAnim : MonoBehaviour {
	
	public enum EnumQuadrant {
		QUAD_00 = 0, QUAD_01 = 1, QUAD_11 = 11, QUAD_10 = 10
	};
	
	struct Piranhas {
		public GameObject piranha00, piranha01, piranha11, piranha10;
	};
	
	private Transform target;
	private GameObject currentPiranhaFire;
	private Piranhas piranhas;
	private PiranhaFireDetector detector;
	
	// Use this for initialization
	void Start () {

		// get the 4 piranhas objects
		piranhas = GetPiranhasSprites(transform);
		// only enabled the piranha 00
		currentPiranhaFire = ActivatePiranhaImage(EnumQuadrant.QUAD_00, piranhas);
		// get the fire detector script
		detector = transform.FindChild("FireTrigger").GetComponent<PiranhaFireDetector>();
	}
	
	// Update is called once per frame
	void Update () {
		
		target = detector.getTarget();
		
		// only look to target if it exists
		if (target != null) {
			EnumQuadrant quad = GetTargetQuadrant(target.position, transform.position);
			currentPiranhaFire = ActivatePiranhaImage(quad, piranhas);
		}
		else
			currentPiranhaFire = null;
	}
	
	public GameObject getCurrentPiranhaFire () {
		return currentPiranhaFire;
	}
	
	private static Piranhas GetPiranhasSprites (Transform transform) {
		
		Piranhas piranhas = new Piranhas();
		
		// Note: according to the rotation of the tube in Z axis the sprites assignation may be different
		
		if (transform.parent.rotation.eulerAngles.z == 0f) {
			piranhas.piranha00 = transform.FindChild("PiranhaFire00").gameObject;
			transform.FindChild("PiranhaFire00").GetComponent<PiranhaFire>().setQuad(0);
			piranhas.piranha01 = transform.FindChild("PiranhaFire01").gameObject;
			transform.FindChild("PiranhaFire01").GetComponent<PiranhaFire>().setQuad(1);
			piranhas.piranha11 = transform.FindChild("PiranhaFire11").gameObject;
			transform.FindChild("PiranhaFire11").GetComponent<PiranhaFire>().setQuad(11);
			piranhas.piranha10 = transform.FindChild("PiranhaFire10").gameObject;
			transform.FindChild("PiranhaFire10").GetComponent<PiranhaFire>().setQuad(10);
		}
		else if (transform.parent.rotation.eulerAngles.z > 179f && transform.parent.rotation.eulerAngles.z < 181f) {
			piranhas.piranha00 = transform.FindChild("PiranhaFire11").gameObject;
			transform.FindChild("PiranhaFire11").GetComponent<PiranhaFire>().setQuad(0);
			piranhas.piranha01 = transform.FindChild("PiranhaFire10").gameObject;
			transform.FindChild("PiranhaFire10").GetComponent<PiranhaFire>().setQuad(1);
			piranhas.piranha11 = transform.FindChild("PiranhaFire00").gameObject;
			transform.FindChild("PiranhaFire00").GetComponent<PiranhaFire>().setQuad(11);
			piranhas.piranha10 = transform.FindChild("PiranhaFire01").gameObject;
			transform.FindChild("PiranhaFire01").GetComponent<PiranhaFire>().setQuad(10);
		}
		return piranhas;
	}
	
	private static GameObject ActivatePiranhaImage (EnumQuadrant quad, Piranhas piranhas) {
		piranhas.piranha00.renderer.enabled = false;
		piranhas.piranha01.renderer.enabled = false;
		piranhas.piranha11.renderer.enabled = false;
		piranhas.piranha10.renderer.enabled = false;
		
		if (quad.Equals(EnumQuadrant.QUAD_00)) {
			piranhas.piranha00.renderer.enabled = true;
			return piranhas.piranha00;
		}
		else if (quad.Equals(EnumQuadrant.QUAD_01)) {
			piranhas.piranha01.renderer.enabled = true;
			return piranhas.piranha01;
		}
		else if (quad.Equals(EnumQuadrant.QUAD_11)) {
			piranhas.piranha11.renderer.enabled = true;
			return piranhas.piranha11;
		}
		else if (quad.Equals(EnumQuadrant.QUAD_10)) {
			piranhas.piranha10.renderer.enabled = true;
			return piranhas.piranha10;
		}
		
		return null;
	}
	
	/**
	 * Checks which quadrant the target is located relative to the current position of the piranha
	 */
	private static EnumQuadrant GetTargetQuadrant (Vector3 targetPos, Vector3 piranhaPos) {
		
		if (targetPos.x <= piranhaPos.x) {
			if (targetPos.y <= piranhaPos.y)
				return EnumQuadrant.QUAD_00;
			return EnumQuadrant.QUAD_10;
		}
		else if (targetPos.y <= piranhaPos.y)
			return EnumQuadrant.QUAD_01;
		return EnumQuadrant.QUAD_11;
	}
}
