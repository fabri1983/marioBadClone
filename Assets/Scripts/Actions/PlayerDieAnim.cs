using UnityEngine;

public class PlayerDieAnim : MonoBehaviour {

	private int playerLayer;
	private uint layersCP;
	private bool dying = false;
	private Jump jump;
	
	void Awake () {
		jump = GetComponent<Jump>();
	}
	
	void Start () {
		// get original player layers so when falling in empty space and die we have the correct layers to restore the player
		playerLayer = gameObject.layer;
		layersCP = gameObject.GetComponent<ChipmunkShape>().layers;
	}
	
	public void startAnimation () {
		dying = true;
		
		// change player's current layer to CAMERA_IN_FRONT layer
		playerLayer = gameObject.layer;
		layersCP = gameObject.GetComponent<ChipmunkShape>().layers;
		GameObjectTools.setLayer(gameObject, LevelManager.LAYER_CAMERA_IN_FRONT);
		GameObjectTools.setLayerForShapes(gameObject, 0);

		// execute a little jump as dying animation
		GetComponent<ChipmunkShape>().body.velocity = Vector2.zero;
		jump.forceJump(GetComponent<Player>().lightJumpVelocity);
	}
	
	public void restorePlayerProps () {
		dying = false;
		// set back player's original layer
		GameObjectTools.setLayer(gameObject, playerLayer);
		GameObjectTools.setLayerForShapes(gameObject, layersCP);
	}
	
	public bool isDying () {
		return dying;
	}
}
