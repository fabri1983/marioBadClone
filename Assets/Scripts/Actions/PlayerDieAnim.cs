#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
#define UNITY_4_AND_LATER
#endif
using UnityEngine;

[ExecuteInEditMode]
public class PlayerDieAnim : MonoBehaviour {
	
	// Shader Script set from Inspector which is used in replacement of normal shader.
	// The difference is only the activation of z-testing and its function so the game object is always rendered in front
	// NOTE: try using conditional compilation for shader
	public Shader shaderForDie;
	
	private uint layersCP;
	private bool dying = false;
	private Jump jump;
	private AnimateTiledTexture animComp; // used to access its renderer
	private Shader origShader;
#if UNITY_EDITOR
	private bool wasDying = false; // used to keep correct asignment of shader when isDying and exiting from Editor Play Mode
#endif

	void Awake () {
		jump = GetComponent<Jump>();
	}
	
	void Start () {
		// get original CP shape layer so when falling in empty space and die we can later restore to original state
		layersCP = gameObject.GetComponent<ChipmunkShape>().layers;
		
		// get original render queue
		animComp = GetComponentInChildren<AnimateTiledTexture>();
		origShader = animComp.renderer.sharedMaterial.shader;
	}
	
	void OnDestroy () {
		// this because oon Editor mode any change in sharedMaterial seems to be saved
		setBackOrigShader();
		// remove reference to anim component
		animComp = null;
	}

#if UNITY_EDITOR
	void Update () {
		// when in Editor Mode and not Playing keep correct original shader
		if (wasDying && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
			setBackOrigShader();
			wasDying = false;
		}
	}
#endif

	public void startAnimation () {
#if UNITY_EDITOR
		wasDying = true;
#endif
		dying = true;

		// change CP shape's current layer so it doesn't collide with any other shape
		layersCP = gameObject.GetComponent<ChipmunkShape>().layers;
		GameObjectTools.setLayerForShapes(gameObject, 0);
		
		// set special shader which will render this game object in front of all layers
		animComp = GetComponentInChildren<AnimateTiledTexture>();
		animComp.renderer.sharedMaterial.shader = shaderForDie;
		
		// execute a little jump as dying animation
		GetComponent<ChipmunkShape>().body.velocity = Vector2.zero;
		jump.forceJump(GetComponent<Player>().lightJumpVelocity);
	}
	
	public void restorePlayerProps () {
		dying = false;
		// set back original shader
		setBackOrigShader();
		// set back player's original layer
		GameObjectTools.setLayerForShapes(gameObject, layersCP);
	}
	
	public bool isDying () {
		return dying;
	}
	
	private void setBackOrigShader () {
		if (animComp != null)
			animComp.renderer.sharedMaterial.shader = origShader;
	}
}
