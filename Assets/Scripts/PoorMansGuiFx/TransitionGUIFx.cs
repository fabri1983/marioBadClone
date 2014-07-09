// PoorMansGUIFX by UnityCoder.com
// Modifications added by fabri1983

using UnityEngine;
using System.Collections;

public enum Transition
{
	FromCurrentPosition,
	ToCurrentPosition
}

public enum Direction
{
	Up,
	Right,
	Down,
	Left
}

enum Element
{
	TRANSFORM, GUI_TEXT, GUI_TEXTURE
}

public class TransitionGUIFx : MonoBehaviour {

	public Transition _transition;
	public Direction direction;
	public float fps = 20f;
	public float startDelaySecs=0;
	public int steps = 32;
	public float acceleration = 1.0f;
	public EasingType easingType;
	public bool useCoroutine = false;
	
	private Element elem; // which element the script will transform
	private int currentStep;
	private float offsetX=0;
	private float offsetY=0;
	private Vector2 startPos = Vector2.zero;
	private Vector2 tempPos = Vector2.zero;
	private float updateTime;
	
	void Awake ()
	{
		elem = Element.TRANSFORM;
		if (guiTexture != null)
			elem = Element.GUI_TEXTURE;
		else if (guiText != null)
			elem = Element.GUI_TEXT;
		
		prepareTransition();
	}
	
	void OnEnable () {
		currentStep = 0;
		if (useCoroutine)
			StartCoroutine("DoCoroutine");
		else {
			updateTime = 0;
			Invoke("DoTransition", startDelaySecs);
		}
	}
	
	void OnDisable () {
		if (useCoroutine)
			StopCoroutine("DoCoroutine");
	}
	
	void Update () {
		if (useCoroutine)
			return;
		float t = Time.time;
		if (t - updateTime > 1f/fps) {
			DoTransition();
			updateTime = t;
		}
	}
	
	// actual transition happens here
	IEnumerator DoCoroutine () 
	{
		// startup delay
		if (startDelaySecs > 0)
		{
			float startTime = Time.time;
			while(Time.time < startTime + startDelaySecs)
			{
				yield return null;
			}
		} // startup delay

		// main transition/easing loop
		while (currentStep < steps)
		{
			transition(currentStep);
			// Wait a time before we move to the next frame. Note, this gives unexpected results on mobile devices
            yield return new WaitForSeconds(1f/fps);
			
			++currentStep;

		} // transition for loop
		
		this.enabled = false;
	}
	
	private void prepareTransition ()
	{
		if (elem == Element.TRANSFORM)
			startPos = transform.position;
		else if (elem == Element.GUI_TEXT)
			startPos.Set(guiText.pixelOffset.x, guiText.pixelOffset.y);
		else if (elem == Element.GUI_TEXTURE)
			startPos.Set(guiTexture.pixelInset.x, guiTexture.pixelInset.y);
		
		// calculate automatic offsets
		switch (_transition)
		{
		case Transition.FromCurrentPosition:
			offsetX = -Easing.Ease(0,acceleration, easingType);
			offsetY = -Easing.Ease(0,acceleration, easingType);
			break;
		case Transition.ToCurrentPosition:
			offsetX = -Easing.Ease(1,acceleration, easingType);
			offsetY = -Easing.Ease(1,acceleration, easingType);
			break;
		}

		// fix offsets
		switch (direction)
		{
			case Direction.Up:
				offsetX = 0;
				break;
			case Direction.Down:
				offsetX = 0;
				offsetY = -offsetY;
				break;
			case Direction.Left:
				offsetX = -offsetX;
				offsetY = 0;
				break;
			case Direction.Right:
				offsetY = 0;
				break;
		}

		// set initial object position
		if (elem == Element.TRANSFORM) {
			Vector3 pos = transform.position;
			pos.x = startPos.x + offsetX;
			pos.y = startPos.y + offsetY;
			transform.position = pos;
		}
		else if (elem == Element.GUI_TEXT) {
			Vector2 pOffset = guiText.pixelOffset;
			pOffset.x = startPos.x + offsetX;
			pOffset.y = startPos.y + offsetY;
			guiText.pixelOffset = pOffset;
		}
		else if (elem == Element.GUI_TEXTURE) {
			Rect pInset = guiTexture.pixelInset;
			pInset.x = startPos.x + offsetX;
			pInset.y = startPos.y + offsetY;
			guiTexture.pixelInset = pInset;
		}
	}
	
	private void DoTransition ()
	{
		// main transition/easing loop
		if (currentStep >= steps)
			this.enabled = false;
		transition(currentStep);
		++currentStep;
	}
	
	private void transition (int step)
	{
		// TODO: remap step from 0-steps into 0-1
		float e = Easing.Ease((float)step/((float)steps-1f), acceleration, easingType);
		
		switch (direction)
		{
		case Direction.Up:
			tempPos.Set(startPos.x + offsetX, startPos.y + offsetY + e);
			break;
		case Direction.Down:
			tempPos.Set(startPos.x + offsetX, startPos.y + offsetY - e);
			break;
		case Direction.Left:
			tempPos.Set(startPos.x + offsetX - e, startPos.y + offsetY);
			break;
		case Direction.Right:
			tempPos.Set(startPos.x + offsetX + e, startPos.y + offsetY);
			break;
		}
		
		if (elem == Element.TRANSFORM) {
			Vector3 pos = transform.position;
			pos.x = tempPos.x;
			pos.y = tempPos.y;
			transform.position = pos;
		}
		else if (elem == Element.GUI_TEXT) {
			Vector2 pOffset = guiText.pixelOffset;
			pOffset.x = tempPos.x;
			pOffset.y = tempPos.y;
			guiText.pixelOffset = pOffset;
		}
		else if (elem == Element.GUI_TEXTURE) {
			Rect pInset = guiTexture.pixelInset;
			pInset.x = tempPos.x;
			pInset.y = tempPos.y;
			guiTexture.pixelInset = pInset;
		}
	}
	
} // class