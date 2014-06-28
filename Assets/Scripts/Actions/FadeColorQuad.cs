using UnityEngine;

/**
 * Fade the current viewport to and from a given a color material using a shader.
 */
public class FadeColorQuad : MonoBehaviour, IFadeable {
	
	public Color fadeColor = Color.black;
	public float fadeTimeFactor = 1f;
	public bool fadeOutOnStart = true;
	public Renderer quadRenderer;
	
	private bool doFading;
	private EnumFadeDirection fadeDir;
	private float alphaFadeValue;
	private bool finishedTransition; // true if the fade could finish
	private Material colorQuad;
	
	// Use this for initialization
	void Awake () {
		colorQuad = quadRenderer.sharedMaterial;
		finishedTransition = false;
		fadeDir = EnumFadeDirection.FADE_NONE;
	}
	
	void Start () {
		if (fadeOutOnStart)
			startFading(EnumFadeDirection.FADE_OUT);
		else
			stopFading();
	}
	
	void LateUpdate () {
		// do fading?
		if (fadeDir != EnumFadeDirection.FADE_NONE)
			fade();
	}
	
	private void fade () {
		// set the alpha color
		fadeColor.a = alphaFadeValue;
		colorQuad.SetColor("_Color", fadeColor);
		
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
