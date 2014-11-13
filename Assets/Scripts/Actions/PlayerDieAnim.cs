using UnityEngine;

public class PlayerDieAnim : MonoBehaviour {

	private uint layersCP;
	private int renderQueue;
	private bool dying = false;
	private Jump jump;
	private AnimateTiledTexture animComp; // used to access its renderer
	
	void Awake () {
		jump = GetComponent<Jump>();
	}
	
	void Start () {
		// get original CP shape layer so when falling in empty space and die we can later restore to original state
		layersCP = gameObject.GetComponent<ChipmunkShape>().layers;
		
		// get original render queue
		animComp = GetComponentInChildren<AnimateTiledTexture>();
		renderQueue = animComp.renderer.sharedMaterial.renderQueue;
	}
	
	public void startAnimation () {
		dying = true;
		
		// change CP shape's current layer so it doesn't collide with any other shape
		layersCP = gameObject.GetComponent<ChipmunkShape>().layers;
		GameObjectTools.setLayerForShapes(gameObject, 0);
		
		// change player's current render layer to one such as it can be drawn in front of all objects except the pause overlay
		renderQueue = animComp.renderer.sharedMaterial.renderQueue;
		animComp.renderer.sharedMaterial.renderQueue = (int)EnumRenderQueue.Overlay_4000;
		
		// execute a little jump as dying animation
		GetComponent<ChipmunkShape>().body.velocity = Vector2.zero;
		jump.forceJump(GetComponent<Player>().lightJumpVelocity);
	}
	
	public void restorePlayerProps () {
		dying = false;
		// set back player's original layer
		GameObjectTools.setLayerForShapes(gameObject, layersCP);
		// restore original render layer
		animComp.renderer.sharedMaterial.renderQueue = renderQueue;
	}
	
	public bool isDying () {
		return dying;
	}
}
