using UnityEngine;

public class KoopaTroopa : MonoBehaviour {
	
	public bool jumpInLoop = false;
	
	private KoopaTroopaDieAnim koopaDie;
	private float heightHalf;
	private Walk move;
	private bool goingRight;
	
	void Start () {
		
		koopaDie = GetComponent<KoopaTroopaDieAnim>();

		// half the height of the goomba's renderer
		heightHalf = transform.GetChild(0).renderer.bounds.size.y / 2f;
		
		move = transform.GetComponent<Walk>();
		
		// set forever jumping
		if (jumpInLoop) {
			Jump jump = GetComponent<Jump>();
			if (jump) {
				jump.setForeverJump(true);
				jump.setForeverJumpSpeed(17f);
			}
		}
		
		// assuming koopa starts going right
		goingRight = true;
	}
	
	void Update () {
		
		// unique behavior: when change direction then scale by -1 in X to simulate rotation
		if (goingRight && !move.isLookingRight()) {
			Vector3 theScale = transform.localScale;
			theScale.x = transform.localScale.x * -1f;
			transform.localScale = theScale;
			goingRight = false;
		}
		else if (!goingRight && move.isLookingRight()) {
			Vector3 theScale = transform.localScale;
			theScale.x = transform.localScale.x * -1f;
			transform.localScale = theScale;
			goingRight = true;
		}
	}
	
	void OnCollisionEnter (Collision collision) {
		
		// if a powerUp (ie fireball) hits the koopa then...
		if (collision.transform.tag.Equals("PowerUp")) {
			Destroy(collision.gameObject);
			// hide it
			if (koopaDie.isFull())
				koopaDie.changeToHide();
			// kill it anyway
			else
				koopaDie.die();
			return;
		}
		
		if (collision.transform.tag.Equals("Floor"))
			return;

		bool collisionFromAbove = collision.transform.position.y > (transform.position.y + heightHalf);
		bool isMario = collision.transform.tag.Equals("Mario");

		// if koopa isn't hide neither bouncing like crazy and collisioned from above with Mario then change to hide state
		if (koopaDie.isFull() && isMario && collisionFromAbove) {
			koopaDie.changeToHide();
			// makes Mario jump a little upwards
			Jump jump = collision.transform.GetComponent<Jump>();
			if (jump)
				jump.jump(10f);
		}
		// koopa die
		else if ((koopaDie.isHide() || koopaDie.isBouncing()) && isMario && collisionFromAbove) {
			koopaDie.die();
			// makes Mario jump a little upwards
			Jump jump = collision.transform.GetComponent<Jump>();
			if (jump)
				jump.jump(10f);
		}
		// if koopa is hide and somebody touches it on its sides then change to bouncing like crazy state in opposite direction of collision
		else if (koopaDie.isHide() && !collisionFromAbove) {
			koopaDie.changeToBouncing(Mathf.Sign(transform.position.x - collision.transform.position.x));
		}
		// kills Mario
		else if (isMario && !collisionFromAbove) {
			move.stopWalking();
			LevelManager.Instance.loseGame(true);
		}
	}
}
