using UnityEngine;

public class BGCamera : MonoBehaviour, IGUIScreenLayout {
	
	void Awake () {
		GUIScreenLayoutManager.Instance.register(this as IGUIScreenLayout);
	}
	
	void OnDestroy () {
		GUIScreenLayoutManager.Instance.remove(this as IGUIScreenLayout);
	}
	
	public void updateForGUI () {
		GUITexture guiTexture = GetComponent<GUITexture>();
		Rect theRect = guiTexture.pixelInset;
		theRect.x = 0f;
		theRect.y =	0f;
		theRect.width = Screen.width;
		theRect.height = Screen.height;
		guiTexture.pixelInset = theRect;
	}
}
