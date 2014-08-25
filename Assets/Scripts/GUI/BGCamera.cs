using UnityEngine;

public class BGCamera : MonoBehaviour, IScreenLayout {
	
	void Awake () {
		ScreenLayoutManager.Instance.register(this);
	}
	
	void OnDestroy () {
		ScreenLayoutManager.Instance.remove(this);
	}
	
	public void updateSizeAndPosition () {
		Rect theRect = guiTexture.pixelInset;
		theRect.x = 0f;
		theRect.y =	0f;
		theRect.width = Screen.width;
		theRect.height = Screen.height;
		guiTexture.pixelInset = theRect;
	}
}
