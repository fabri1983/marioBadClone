using UnityEngine;

/**
 * This power up defines the behavior of wings for Mario.
 * Only defines max usage and max time of usage
 */
public class PowerUpPWings : PowerUp {
	
	public float flySpeed = 50f;
	public float fallSpeed = 50f;
	public float timeBetweenFly = 0.4f; // time to wait between fly actions
	public float lifeTimeSecs = 10f;
	
	private Fly fly;
	private float timerAlive, rateOfFly;
	private Rigidbody targetRigidbody;
	private Jump targetJump;
	
	protected override void ownStart () {
		base.usageLeft = 0;
		base.destroyTime = lifeTimeSecs;
		base.firePow = 0f;
		fly = GetComponent<Fly>();
		timerAlive = 0f;
		rateOfFly = 0f;
	}
	
	protected override void ownUpdate () {
		
		// updates living time. When reaches to 0 then the power up isn't usable anymore
		if (timerAlive <= 0f) {
			if (targetRigidbody) {
				targetRigidbody.useGravity = true;
				targetRigidbody = null;
			}
			if (targetJump) {
				targetJump.enabled = true;
				targetJump.reset();
				targetJump = null;
			}
			return;
		}
		
		timerAlive -= Time.deltaTime;
		rateOfFly -= Time.deltaTime;
	}
	
	public override void assignToCharacter (MonoBehaviour element) {
		Player p = (Player) element;
		p.setPowerUp(this);
		if (p.GetComponent<Fly>() == null)
			fly = p.gameObject.AddComponent<Fly>();
		
		// init usage left
		base.usageLeft = 50;
		timerAlive = base.destroyTime;
		rateOfFly = 0f;
	}
	
	protected override void animOnGotcha () {
		
		// moves the transform upwards showing the power up just gained
		if (objectToAnim != null)
			objectToAnim.transform.Translate(0f, 4f * Time.deltaTime, 0f, Space.World);
		// NOTE: sice the transform will be destroyed then there is no need for stop animation
	}
	
	public override void ownAction (GameObject go) {

		if (fly != null && rateOfFly <= 0f) {
			targetJump.reset();
			fly.fly();
			// restore rate of fly
			rateOfFly = timeBetweenFly;
		}
	}
	
	public override bool ableToUse () {
		return timerAlive > 0f;
	}
	
	public override void use () {
	}
	
	public override bool isAllowedInput () {
		return Input.GetButtonDown("Jump");
	}
}

