using UnityEngine;

/**
 * This power up only add a score point to Mario. It ins't assigned to him.
 */
public class PowerUpCoin : PowerUp {
	
	//private PlayerController player;
	
	protected override void ownStart () {
		
		base.usageLeft = 0;
		base.destroyTime = 1f;
		base.firePow = 0f;
	}
	
	protected override void ownUpdate () {
	}
	
	protected override void animOnGotcha () {
		
		// moves the transform upwards showing the power up just gained
		if (objectToAnim != null)
			objectToAnim.transform.Translate(0f, 4f * Time.deltaTime, 0f, Space.World);
		// NOTE: sice the transform will be destroyed then there is no need for stop animation
	}
	
	public override void assignToCharacter (MonoBehaviour element) {
		//player = ((PlayerController) element);
		// adds +1 to Mario's wallet
	}
	
	public override void ownAction (GameObject go) {
		// does nothing because is already considered when power up is "assigned" to Mario
	}
	
	public override bool ableToUse () {
		// a coin isn't used
		return false;
	}
	
	public override void use () {
		// a coin has no usage as a weapon
	}
	
	public override bool isAllowedInput () {
		// a coin isn't reactive to Input
		return false;
	}
}

