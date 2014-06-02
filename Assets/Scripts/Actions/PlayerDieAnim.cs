using UnityEngine;

public class PlayerDieAnim : MonoBehaviour {

	private int playerLayer;
	private bool dying = false;
	private float endPosition = 0f;
	private Jump jump;
	
	void Awake () {
		jump = GetComponent<Jump>();
	}

	void LateUpdate () {
		if (!dying)
			return;
		
		// end of animation?
		if (transform.position.y < endPosition) {
			dying = false;
			// set back mario's original layer
			GameObjectTools.setLayerAndChildren(gameObject, playerLayer);
			// restart level
			LevelManager.Instance.loseGame(false);
		}
	}
	
	public void startAnimation () {
		
		dying = true;
		
		// change Mario's current layer to CAMERA_IN_FRONT layer
		playerLayer = gameObject.layer;
		GameObjectTools.setLayerAndChildren(gameObject, LevelManager.CAMERA_IN_FRONT);

		// execute die animation
		endPosition = transform.position.y + LevelManager.ENDING_DIE_ANIM_Y_POS;
		GetComponent<ChipmunkShape>().body.velocity = Vector2.zero;
		jump.forceJump(GetComponent<Player>().lightJumpVelocity);
	}
	
	public bool isDying () {
		return dying;
	}
}
