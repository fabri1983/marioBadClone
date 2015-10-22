using UnityEngine;

[System.Serializable]
public class SwipeGestureSettings
{
	public float minSwipeDist = 20f; // minimum pixel displacement to consider a swipe
	public float maxSwipeTime = 10f; // time the touch stayed at the screen from begining till now.
	public Rect activeArea; // where you can touch to start the swipe detection
	
	private bool areaSetOriginallyByUser = false;
	private float leftProportion, topProportion;
	private bool setupReady = false;
	
	private static Rect EMPTY_AREA = new Rect(0f, 0f, 0f, 0f);
	
	public void setup () {
		if (EMPTY_AREA.Equals(activeArea)) {
			fillAreaToCurrentScreen();
			areaSetOriginallyByUser = false;
		} else {
			areaSetOriginallyByUser = true;
			// calculate left and top proportions
			leftProportion = (activeArea.x + 1f) / Screen.width;
			topProportion = (activeArea.y + 1f) / Screen.height;
		}
		setupReady = true;
	}
	
	private void fillAreaToCurrentScreen () {
		activeArea = new Rect(0f ,0f, Screen.width, Screen.height);
	}
	
	public void recalculateArea () {
		if (!setupReady)
			return;
		
		if (!areaSetOriginallyByUser)
			fillAreaToCurrentScreen();
		else {
			// given the proportion values calculate left and top values
			activeArea.x = Screen.width * leftProportion;
			activeArea.y = Screen.height * topProportion;
		}
	}
}
