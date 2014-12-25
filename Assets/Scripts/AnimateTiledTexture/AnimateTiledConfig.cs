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
	
	public EnumAnimateTiledName actionName = EnumAnimateTiledName.None; // this used to reference the correct animate tiled configuration component
	public float animFPS = 1; // sprite animation's fps
	public bool pingPongAnim = false; // true for going forward and backwards in the animation
	public int rowStartAnim = 0; // the absolute row position where the anim starts
	public int rowLengthAnim = 1; // how many rows the anim has?
	public int colStartAnim = 0; // the column from the starting row where the anim starts
	public int colLengthAnim = 1; // how total columns the anim has
	
	private bool working = false; // handled from the caller, not from the animate tiled object
	
	void Awake () {
		animComp = GetComponent<AnimateTiledTexture>();
	}
	
	public void setupAndPlay () {
		animComp.setFPS(animFPS);
		animComp.setRowLimits(rowStartAnim, rowLengthAnim);
		animComp.setColLimits(colStartAnim, colLengthAnim);
		animComp.setPingPongAnim(pingPongAnim);
		working = true;
		animComp.Play();
	}
	
	public void stop () {
		working = false;
	}
	
	public bool isWorking () {
		return working;
	}
	
	/// <summary>
	/// Searchs for the AnimateTiledConfig instance of a given game object that is associated with the given name.
	/// </summary>
	/// <returns>
	/// The AnimateTiledConfig instance
	/// </returns>
	/// <param name='go'>
	/// The source game object
	/// </param>
	/// <param name='name'>
	/// Enum value which is associated to the animation
	/// </param>
	/// <param name='inChildren'>
	/// If true then traverse down into herarchy
	/// </param>
	public static AnimateTiledConfig getByName (GameObject go, EnumAnimateTiledName name, bool inChildren) {
		if (go == null)
			return null;
		
		AnimateTiledConfig[] anims = null;
		
		if (inChildren)
			anims = go.GetComponentsInChildren<AnimateTiledConfig>();
		else
			anims = go.GetComponents<AnimateTiledConfig>();
		
		for (int i=0,c=anims.Length; i<c; ++i) {
			if (name.Equals(anims[i].actionName))
				return anims[i];
		}
		
		return null;
	}
}
