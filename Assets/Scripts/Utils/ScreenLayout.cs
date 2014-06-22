using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ScreenLayout : MonoBehaviour {
	
	public Vector2 offset = Vector2.zero;
	public EnumScreenLayout layout = EnumScreenLayout.BOTTOM_LEFT;
	
	void Awake () {
		ScreenLayoutManager.Instance.register(this);
		updateSizeAndPosition();
	}
	
	void OnDestroy () {
		ScreenLayoutManager.Instance.remove(this);
	}
	
	public void updateSizeAndPosition() {
		ScreenLayoutManager.adjustPos(guiTexture, offset, layout);
	}

#if UNITY_EDITOR
	// this only execute for Editor mode so any change in offset or enum layout is applied in real time
	void Update () {
		if (Application.isPlaying)
			return;
		updateSizeAndPosition();
	}
#endif
}
