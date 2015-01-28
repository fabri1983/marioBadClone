#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
#define UNITY_4_AND_LATER
#endif
using UnityEngine;

/**
 * This power up defines the behavior of a fire ball. Can be used by Mario or by whatever character.
 * The action instantiate the fire ball prefab and shoots it given a direction and an initial fire position.
 */
public class PowerUpFireBall : PowerUp {
	
	public float rateOfFire = 0.3f;
	private Player player;
	private float timingFire; // rate of fire
	
	protected override void ownStart () {
		base.usageLeft = 15;
		base.destroyTime = 5f;
		base.firePow = 20f;
		timingFire = 0f;
	}
	
	protected override void ownUpdate () {
		
		// only will update rate of fire
		if (timingFire > 0)
			timingFire -= Time.deltaTime;
	}
	
	public override void assignToCharacter (MonoBehaviour element) {
		player = ((Player) element);
		player.setPowerUp(this);
		
		// init usage left
		base.usageLeft = 15;
		timingFire = 0f;
	}
	
	protected override void animOnGotcha () {
		
		// moves the transform upwards showing the power up just gained
		if (objectToAnim != null)
			objectToAnim.transform.Translate(0f, 4f * Time.deltaTime, 0f, Space.World);
		// NOTE: sice the game object will be destroyed then there is no need for stop animation
	}
	
	public override void ownAction (GameObject go) {
		
		// shoot a fireball with a given fire rate
		if (timingFire <= 0f) {
			timingFire = rateOfFire;
			
			GameObject newGO = GameObject.Instantiate(getArtifact(), player.getFirePivot().position, Quaternion.identity) as GameObject;
			// deactivate it so it doesn't interact with current game object creator
#if UNITY_4_AND_LATER
			newGO.SetActive(false);
#else
			newGO.active = false; 
#endif
			newGO.tag = "PowerUp";
			FireBall fireball = newGO.GetComponent<FireBall>();
			fireball.setDestroyTime(getDestroyTime());
			fireball.setBouncing(true);
			fireball.setDir(player.getFireDir());
			fireball.setSpeed(getPower());
			fireball.addTargetLayer(LayerMask.NameToLayer(Layers.POWER_UP));
			newGO.transform.parent = null;
#if UNITY_4_AND_LATER
			newGO.SetActive(true);
#else
			newGO.active = true;
#endif
			use();
		}
	}
	
	public override bool ableToUse () {
		return usageLeft > 0;
	}
	
	public override void use () {
		--usageLeft;
	}
	
	public override bool isAllowedInput () {
		return Gamepad.isB() || Input.GetButtonDown("Fire1");
	}
}

