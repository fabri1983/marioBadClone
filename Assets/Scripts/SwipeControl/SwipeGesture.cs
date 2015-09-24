using UnityEngine;
using System.Collections.Generic;

public class SwipeGesture : MonoBehaviour, IGUIScreenLayout {
	
	public SwipeGestureSettings settings;
	public bool debug;

	private List<SwipeGestureListener> onRightSwipeListeners;
	private List<SwipeGestureListener> onLeftSwipeListeners;
	private List<SwipeGestureListener> onUpSwipeListeners;
	private List<SwipeGestureListener> onDownSwipeListeners;
	private float swipeStartTime;
	private bool couldBeSwipe;
	private Vector2 startPos;
	private int stationaryForFrames;
	private TouchPhase lastPhase;
	private Vector3 posInverted = Vector3.zero;
	
	void Awake () {
		// register this class with ScreenLayoutManager for screen resize event
		GUIScreenLayoutManager.Instance.register(this as IGUIScreenLayout);

		onRightSwipeListeners = new List<SwipeGestureListener>(2);
		onLeftSwipeListeners = new List<SwipeGestureListener>(2);
		onUpSwipeListeners = new List<SwipeGestureListener>(2);
		onDownSwipeListeners = new List<SwipeGestureListener>(2);
	}
	
	void OnDestroy () {
		onRightSwipeListeners = null;
		onLeftSwipeListeners = null;
		onUpSwipeListeners = null;
		onDownSwipeListeners = null;
		GUIScreenLayoutManager.Instance.remove(this as IGUIScreenLayout);
	}
	
	public void setup () {
		settings.setup();
		// add here other initializations
	}
	
	public void setup (SwipeGestureSettings gestureSettings) {
		this.settings = gestureSettings;
		setup();
	}
	
	public void updateForGUI () {
		// if screen is resized then need update the active area, if necessary
		settings.recalculateArea();
	}
	
	void Update() {
		processTouch();
	}
	
#if UNITY_EDITOR
	void OnGUI () {
		if (debug)
			GUI.Box(settings.activeArea, GUIContent.none);
	}
#endif
	
	private void processTouch () {
		foreach (Touch touch in Input.touches)
		{
			posInverted.x = touch.position.x;
			posInverted.y = Screen.height - touch.position.y;
			if (!settings.activeArea.Contains(posInverted))
				continue;
			
			switch (touch.phase)
			{
				case TouchPhase.Began: //The finger first touched the screen --> It could be(come) a swipe
					couldBeSwipe = true;
					startPos = touch.position;  //Position where the touch started
					swipeStartTime = Time.time; //The time it started
					stationaryForFrames = 0;
					break;
					
				case TouchPhase.Stationary: //Is the touch stationary? --> No swipe then!
					if (isContinouslyStationary(15))
						couldBeSwipe = false;
					break;
					
				case TouchPhase.Canceled:
				case TouchPhase.Ended:
					if (isASwipe(touch))
					{
						couldBeSwipe = false; //<-- Otherwise this part would be called over and over again.
						if (Mathf.Abs(touch.position.x - startPos.x) > settings.minSwipeDist) {
							if (Mathf.Sign(touch.position.x - startPos.x) == 1f) {
								for (int i=0, c=onRightSwipeListeners.Count; i < c; ++i)
									onRightSwipeListeners[i].notifyRight();
							} else {
								for (int i=0, c=onLeftSwipeListeners.Count; i < c; ++i)
									onLeftSwipeListeners[i].notifyLeft();
							}
						}
						
						if (Mathf.Abs(touch.position.y - startPos.y) > settings.minSwipeDist) {
							if (Mathf.Sign(touch.position.y - startPos.y) == 1f) {
								for (int i=0, c=onUpSwipeListeners.Count; i < c; ++i)
									onUpSwipeListeners[i].notifyUp();
							} else {
								for (int i=0, c=onDownSwipeListeners.Count; i < c; ++i)
									onDownSwipeListeners[i].notifyDown();
							}
						}
					}
					break;
			}
			lastPhase = touch.phase;
		}
	}
	
	private bool isContinouslyStationary(int frames)
	{
		if (lastPhase == TouchPhase.Stationary)
			stationaryForFrames++;
		else
			stationaryForFrames = 1;
		
		return stationaryForFrames > frames;
	}
	
	private bool isASwipe(Touch touch)
	{
		float swipeTime = Time.time - swipeStartTime;
		float swipeDist = Mathf.Sqrt((touch.position - startPos).sqrMagnitude);
		return couldBeSwipe && swipeTime < settings.maxSwipeTime && swipeDist > settings.minSwipeDist;
	}

	public void registerOnRightSwipe (SwipeGestureListener listener) {
		onRightSwipeListeners.Add(listener);
	}
	
	public void registerOnLeftSwipe (SwipeGestureListener listener) {
		onLeftSwipeListeners.Add(listener);
	}
	
	public void registerOnUpSwipe (SwipeGestureListener listener) {
		onUpSwipeListeners.Add(listener);
	}
	
	public void registerOnDownSwipe (SwipeGestureListener listener) {
		onDownSwipeListeners.Add(listener);
	}
}
