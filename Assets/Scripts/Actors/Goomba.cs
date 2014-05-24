using UnityEngine;

public class Goomba : MonoBehaviour {

	private GoombaDieAnim goombaDie;
	private Patrol patrol;
	private Idle idle;
	
	void Start () {
		
		goombaDie = GetComponent<GoombaDieAnim>();
		
		patrol = GetComponent<Patrol>();
		idle = GetComponent<Idle>();
	}
	
	void OnCollisionEnter (Collision collision) {

		if (!goombaDie.isDying()) {
			// if a powerUp (ie fireball) hits the goomba then he dies
			if (collision.transform.tag.Equals("PowerUp")) {
				Destroy(collision.gameObject);
				goombaDie.die();
				return;
			}
			else if (collision.transform.tag.Equals("Mario")) {
				// if collides from top then kill the goomba
				if ((collision.transform.position.y - collision.collider.bounds.size.y/2f) > transform.position.y) {
					goombaDie.die();
					// makes Mario jumps a little upwards
					/*Jump jump = collision.transform.GetComponent<Jump>();
					if (jump)
						jump.jump(7f, 35f);*/
				}
				// kills Mario
				else {
					patrol.stopPatrol();
					idle.setIdle(true);
					LevelManager.Instance.loseGame(true);
				}
			}
		}
	}
}
