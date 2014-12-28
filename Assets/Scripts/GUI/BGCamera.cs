using UnityEngine;

public class BGCamera : MonoBehaviour, IGUIScreenLayout {
	
	void Awake () {
		GUIScreenLayoutManager.Instance.register(this);
	}
	
	void OnDestroy () {
		GUIScreenLayoutManager.Instance.remove(this);
	}
	
	public void updateForGUI () {
		Rect theRect = guiTexture.pixelInset;
		theRect.x = 0f;
		theRect.y =	0f;
		theRect.width = Screen.width;
		theRect.height = Screen.height;
		guiTexture.pixelInset = theRect;
	}
}
