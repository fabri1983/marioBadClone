using UnityEngine;

/**
 * Fade the current game object's material to and from a given a color.
 * The expected material is one having the shader FadeColor_CG.
 * It doesn't matter the mesh the game object has for render.
 * The FadeColor_CG shader has it's own render queue assigned in such way it will overlay 
 * all materials, except Unity's GUI and any material with correct render queue.
 */
public class FadeByColor : MonoBehaviour, IFadeable, IGUIScreenLayout {
	
	public Color fadeColor = Color.black;
	public float fadeTimeFactor = 1f;
	public bool fadeOutOnStart = true;
	public Vector2 sizeFactor = Vector2.one;
	
	private bool doFading;
	private EnumFadeDirection fadeDir;
	private float alphaFadeValue;
	private bool finishedTransition; // true if the fade could finish
	private Material fadeMat;
	
	void Awake () {
		fadeMat = renderer.sharedMaterial;
		finishedTransition = false;
		fadeDir = EnumFadeDirection.FADE_NONE;
		
		// register this class with ScreenLayoutManager for screen resize event
		GUIScreenLayoutManager.Instance.register(this as IGUIScreenLayout);
	}
	
	void Start () {
		if (fadeOutOnStart)
			startFading(EnumFadeDirection.FADE_OUT);
		else
			stopFading();
		
		locateInScreen(); // make this game object to be located correctly in viewport
	}
	
	void OnDestroy () {
		GUIScreenLayoutManager.Instance.remove(this as IGUIScreenLayout);
	}
	
	void Update () {
		// do fading?
		if (fadeDir != EnumFadeDirection.FADE_NONE)
			fade();
	}
	
	private void fade () {
		// set the alpha color
		fadeColor.a = alphaFadeValue;
		fadeMat.SetColor("_Color", fadeColor);
		
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
	
	private void locateInScreen () {
		Vector2 sizeInPixels;
		sizeInPixels.x = sizeFactor.x * Screen.width;
		sizeInPixels.y = sizeFactor.y * Screen.height;
		GUIScreenLayoutManager.locateForGUI(transform, sizeInPixels);
	}
	
	public void updateForGUI () {
		locateInScreen();
	}
}
