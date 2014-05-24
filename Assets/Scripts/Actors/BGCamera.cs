using UnityEngine;

public class BGCamera : MonoBehaviour {
	
	void OnGUI () {
		// if screen is resized then need to change background texture size
		if (EventType.Repaint == Event.current.type) {
			if (Screen.width != guiTexture.pixelInset.width || Screen.height != guiTexture.pixelInset.height) {
				guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
			}
		}
	}
}
