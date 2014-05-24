using UnityEngine;

public class AnimateTiledConfig : MonoBehaviour {
	
	// Animated Tiled Texture params
	public AnimateTiledTexture animComponent; // assigned per every player instance (currently only one player)
	public float animFPS = 1; // fps of the sprite animation
	public bool pingPongAnim = false; // true for going forward and backwards in the animation
	public int maxColsAnimInRow = 0; // max cols per row the anim covers, not the total cols the anim has
	public int rowStartAnim = 0; // the row position where the anim start
	public int rowLengthAnim = 0; // how many rows the anim has
	public int colStartAnim = 0; // the col position where the anim start
	public int colLengthAnim = 0; // how many cols the anim has
}
