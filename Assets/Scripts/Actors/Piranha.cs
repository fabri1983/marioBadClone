using UnityEngine;

public class Piranha : MonoBehaviour {
	
	void OnCollisionEnter (Collision collision) {

		// if a powerUp (ie fireball) hits the piranha then it dies
		if (collision.transform.tag.Equals("PowerUp")) {
			Destroy(collision.gameObject);
			Destroy(gameObject);
			return;
		}
		// if hits mario, the kill it
		if (collision.transform.tag.Equals("Mario")) {
			LevelManager.Instance.loseGame(true);
		}
	}
}
