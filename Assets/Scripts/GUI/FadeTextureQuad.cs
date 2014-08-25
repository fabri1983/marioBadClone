using UnityEngine;

/**
 * Fade the current viewport to and from a given a color using a GUI texture created internally.
 * Since it uses Unity's GUI.DrawTexture then there is no need to assign correct render queue 
 * neither locate the game object in front of the screen.
 */
public class FadeTextureQuad : MonoBehaviour, IFadeable, IScreenLayout {
	
	public Color fadeColor = Color.black;
	public float fadeTimeFactor = 1f;
	public bool fadeOutOnStart = true;
	
	private bool doFading;
	private EnumFadeDirection fadeDir;
	private float alphaFadeValue;
	private Texture2D tex; // empty texture element, used for GUI.DrawTexture()
	private Rect rectTex; // is the rectangle where the texture will be drawn
	private bool finishedTransition; // true if the fade could finish
	
	void Awake () {
		// create the texture manually
		tex = new Texture2D(1, 1, TextureFormat.Alpha8, false);
		tex.SetPixel(1,1, fadeColor);
		// create the rectangle where the texture will fill in
		rectTex = new Rect(0, 0, Screen.width, Screen.height);
		
		finishedTransition = false;
		fadeDir = EnumFadeDirection.FADE_NONE;
		// disable by default
		this.enabled = false;
		
		ScreenLayoutManager.Instance.register(this);
	}
	
	void Start () {
		if (fadeOutOnStart)
			startFading(EnumFadeDirection.FADE_OUT);
		else
			stopFading();
	}
	
	void OnDestroy () {
		ScreenLayoutManager.Instance.remove(this);
	}
	
	void LateUpdate () {
		// do fading?
		if (fadeDir != EnumFadeDirection.FADE_NONE)
			fade();
	}
	
	public void updateSizeAndPosition () {
		rectTex.x = 0f;
		rectTex.y =	0f;
		rectTex.width = Screen.width;
		rectTex.height = Screen.height;
	}
	
	private void fade () {
		// GUI.color affects both background and text colors, so back it up
		Color origColor = GUI.color;
		// set the fade color
		fadeColor.a = alphaFadeValue;
		GUI.color = fadeColor;
		GUI.DrawTexture(rectTex, tex, ScaleMode.StretchToFill, true);
		// restore GUI color
		GUI.color = origColor;
		
		if (fadeDir.Equals(EnumFadeDirection.FADE_IN)) {
			alphaFadeValue = Mathf.Clamp01(alphaFadeValue + (Time.deltaTime * fadeTimeFactor));
			if (alphaFadeValue == 1f) {
				finishedTransition = true;
			}
		}
		else if (fadeDir.Equals(EnumFadeDirection.FADE_OUT)) {
			alphaFadeValue = Mathf.Clamp01(alphaFadeValue - (Time.deltaTime * fadeTimeFactor));
			if (alphaFadeValue == 0f) {
				finishedTransition = true;
				if (fadeOutOnStart)
					stopFading(); // leaves state properly
			}
		}
	}
	
	public void startFading (EnumFadeDirection direction) {
		this.enabled = true;
		// keeps the fade direction for the fade transition
		fadeDir = direction;
		// reset status
		finishedTransition = false;
		// sets starting alpha values depending on the fade direction
		if (direction.Equals(EnumFadeDirection.FADE_IN))
			alphaFadeValue = 0f;
		else if (direction.Equals(EnumFadeDirection.FADE_OUT))
			alphaFadeValue = 1f;
	}
	
	public void stopFading () {
		this.enabled = false;
		fadeDir = EnumFadeDirection.FADE_NONE;
		finishedTransition = true;
	}
	
	public bool isTransitionFinished () {
		return finishedTransition;
	}
	
	public EnumFadeDirection getFadingDirection () {
		return fadeDir;
	}
}
