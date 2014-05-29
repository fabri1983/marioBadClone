using UnityEngine;

public class BouncerPlatform : MonoBehaviour {
	
	public float bounceSpeed = 55f;

	void OnCollisionEnter (Collision collision) {
	
		if (collision.transform.tag.Equals("Mario")) {

			Jump jump = collision.transform.GetComponent<Jump>();
			if (jump != null)
				jump.jump(bounceSpeed);
		}
	}
}
