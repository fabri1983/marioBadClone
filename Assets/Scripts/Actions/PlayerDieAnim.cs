using UnityEngine;

public class PlayerDieAnim : MonoBehaviour {

	private int playerLayer;
	private uint layersCP;
	private bool dying = false;
	private Jump jump;
	
	void Awake () {
		jump = GetComponent<Jump>();
	}

	void LateUpdate () {
		if (!dying)
			return;
		
		// end of animation?
		if (transform.position.y < LevelManager.ENDING_DIE_ANIM_Y_POS) {
			dying = false;
			// set back player's original layer
			GameObjectTools.setLayer(gameObject, playerLayer);
			GameObjectTools.setLayerForShapes(gameObject, layersCP);
			// restart level
			LevelManager.Instance.loseGame(false);
		}
	}
	
	public void startAnimation () {
		dying = true;
		
		// change player's current layer to CAMERA_IN_FRONT layer
		playerLayer = gameObject.layer;
		layersCP = gameObject.GetComponent<ChipmunkShape>().layers;
		GameObjectTools.setLayer(gameObject, LevelManager.LAYER_CAMERA_IN_FRONT);
		GameObjectTools.setLayerForShapes(gameObject, 0);

		// execute die animation
		GetComponent<ChipmunkShape>().body.velocity = Vector2.zero;
		jump.forceJump(GetComponent<Player>().lightJumpVelocity);
	}
	
	public bool isDying () {
		return dying;
	}
}
