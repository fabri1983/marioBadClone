using UnityEngine;
using System.Collections;

public class SwipeGesture : MonoBehaviour, IGUIScreenLayout {

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
	
	public SwipeGestureSettings settings;
	public bool debug;
	
	public delegate void DelegateContainer ();
	public event DelegateContainer EventOnRightSwipe;
	public event DelegateContainer EventOnLeftSwipe;
	public event DelegateContainer EventOnUpSwipe;
	public event DelegateContainer EventOnDownSwipe;
	
	private float swipeStartTime;
	private bool couldBeSwipe;
	private Vector2 startPos;
	private int stationaryForFrames;
	private TouchPhase lastPhase;
	private Vector3 posInverted = Vector3.zero;
	
	void Awake () {
		// register this class with ScreenLayoutManager for screen resize event
		GUIScreenLayoutManager.Instance.register(this as IGUIScreenLayout);
		
		// initialize with empty delegates to avoid null pointer exception
		EventOnRightSwipe += () => { };
		EventOnLeftSwipe += () => { };
		EventOnUpSwipe += () => { };
		EventOnDownSwipe += () => { };
	}
	
	void OnDestroy () {
		EventOnRightSwipe = null;
		EventOnLeftSwipe = null;
		EventOnUpSwipe = null;
		EventOnDownSwipe = null;
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
					if (isContinouslyStationary(12))
						couldBeSwipe = false;
					break;
					
				case TouchPhase.Canceled:
				case TouchPhase.Ended:
					if (isASwipe(touch))
					{
						couldBeSwipe = false; //<-- Otherwise this part would be called over and over again.
						if (Mathf.Abs(touch.position.x - startPos.x) > settings.minSwipeDist) {
							if (Mathf.Sign(touch.position.x - startPos.x) == 1f) {
								EventOnRightSwipe();
							} else {
								EventOnLeftSwipe();
							}
						}
						
						if (Mathf.Abs(touch.position.y - startPos.y) > settings.minSwipeDist) {
							if (Mathf.Sign(touch.position.y - startPos.y) == 1f) {
								EventOnUpSwipe();
							} else {
								EventOnDownSwipe();
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

}
