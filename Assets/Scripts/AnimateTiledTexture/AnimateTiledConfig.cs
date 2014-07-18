using UnityEngine;

/// <summary>
/// This class exposes properties for configuring an AnimateTiledTexture instance.
/// The setting of properties is handled by the script that needs to run a sprite animation.
/// Note that AnimateTiledTexture has similar properties, and it's a standalone script. 
/// This configuration class acts as a helper.
/// </summary>
public class AnimateTiledConfig : MonoBehaviour {
	
	[HideInInspector] // hide because I want this be asigned thru script
	public AnimateTiledTexture animComp; // component which has all the animation tiled logic

	// Animated Tiled Texture params
	public float animFPS = 1; // sprite animation's fps
	public bool pingPongAnim = false; // true for going forward and backwards in the animation
	public int maxColsAnimInRow = 0; // the greatest number of columns from the rows the anim covers, not the total columns the anim has
	public int rowStartAnim = 0; // the absolute row position where the anim starts
	public int rowLengthAnim = 0; // how many rows the anim has?
	public int colStartAnim = 0; // the column from the starting row where the anim starts
	public int colLengthAnim = 0; // how total columns the anim has
[HideInInspector] public bool working = false; // avoid setting these properties. Handled from the caller, not from the animate tiled object
	
	void Awake() {
		animComp = GetComponent<AnimateTiledTexture>();
	}
}
