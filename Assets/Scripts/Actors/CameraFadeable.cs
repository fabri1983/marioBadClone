using UnityEngine;

/**
 * This class acts as a container for the fadeable component a camera has as a child component.
 * This solves the next runtime warning message:
 * 
 *    GetComponent requires that the requested component 'IFadeable' is inherited or implemented by 'MainCamera'.
 * 
 * that use to happens when triying to get the IFadeable component from Camera.main.
 */
public class CameraFadeable : MonoBehaviour {
	
	private IFadeable fader = null;
	
	public IFadeable getFader () {
		// can't set the reference on Start() because LevelManager also do some stuffs in Start() that depends on this fader component
		if (fader == null) {
			FadeColor component = GetComponentInChildren<FadeColor>();
			fader = (IFadeable)component.GetComponent(typeof(IFadeable));
		}
		return fader;
	}
}
