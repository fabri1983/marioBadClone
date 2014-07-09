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
	// NOTE: currently only TRANSFORM is used, because I don't know exactly what values to use when working with pixels
	TRANSFORM, GUI_TEXT, GUI_TEXTURE
}

/// <summary>
/// This class modifies transform component for doing a transition effect.
/// When this script in a GUIText or GUITexture element, please consider your actual pixel offset 
/// and set the start offset transform property accordingly.
/// </summary>
public class TransitionGUIFx : MonoBehaviour {
	
	public Vector2 startOffsetTransform = Vector2.zero;
	public Transition _transition;
	public Direction direction;
	public float startDelaySecs=0;
	public int steps = 32;
	public float acceleration = 1f;
	public EasingType easingType;
	public bool useCoroutine = false;
	
	private Element elem; // which element the script will transform
	private Vector2 finalPos = Vector2.zero;
	private int currentStep;
	private float offsetX=0;
	private float offsetY=0;
	private Vector2 startPos = Vector2.zero;
	private bool update;
	
	void Awake ()
	{
		elem = Element.TRANSFORM;
		/*if (guiTexture != null)
			elem = Element.GUI_TEXTURE;
		else if (guiText != null)
			elem = Element.GUI_TEXT;*/
		
		update = false;
		prepareTransition();
	}
	
	void OnEnable () {
		currentStep = 0;
		if (useCoroutine)
			StartCoroutine("DoCoroutine");
		else
			Invoke("enableUpdate", startDelaySecs);
	}
	
	void OnDisable () {
		if (useCoroutine)
			StopCoroutine("DoCoroutine");
	}
	
	void Update () {
		if (useCoroutine)
			return;
		if (update)
			DoTransition();
	}
	
	void enableUpdate () {
		update = true;
	}
	
	private void prepareTransition ()
	{
		if (Mathf.Abs(steps) < 2)
			steps = (int)Mathf.Sign(steps) * 2;
		
		// set final position
		finalPos.Set(transform.position.x, transform.position.y);
		
		if (elem == Element.TRANSFORM)
			startPos.Set(transform.position.x + startOffsetTransform.x, transform.position.y + startOffsetTransform.y);
		else if (elem == Element.GUI_TEXT)
			startPos.Set(guiText.pixelOffset.x + startOffsetTransform.x, guiText.pixelOffset.y + startOffsetTransform.y);
		else if (elem == Element.GUI_TEXTURE)
			startPos.Set(guiTexture.pixelInset.x + startOffsetTransform.x, guiTexture.pixelInset.y + startOffsetTransform.y);
		
		// calculate automatic offsets
		switch (_transition)
		{
		case Transition.FromCurrentPosition:
			offsetX = -EasingFX.Ease(0,acceleration, easingType);
			offsetY = -EasingFX.Ease(0,acceleration, easingType);
			break;
		case Transition.ToCurrentPosition:
			offsetX = -EasingFX.Ease(1,acceleration, easingType);
			offsetY = -EasingFX.Ease(1,acceleration, easingType);
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
		}

		// main transition/easing loop
		//while (currentStep < steps)
		while (true)
		{
			transition(currentStep);
			++currentStep;
			if (finalPos.x == transform.position.x && finalPos.y == transform.position.y)
				break;
            yield return null;
		}
		
		this.enabled = false;
	}
	
	private void DoTransition ()
	{
		// main transition/easing loop
		//if (currentStep >= steps)
		if (finalPos.x == transform.position.x && finalPos.y == transform.position.y) {
			this.enabled = false;
			return;
		}
		transition(currentStep);
		++currentStep;
	}
	
	private void transition (int step)
	{
		// TODO: remap step from 0-steps into 0-1
		float linearStep = (float)step/(steps-1);
		float e = EasingFX.Ease(linearStep, acceleration, easingType);
		float newX=0f, newY=0f;
		
		switch (direction)
		{
		case Direction.Up:
			newX=startPos.x + offsetX; newY=startPos.y + offsetY + e;
			if (newY > finalPos.y) newY = finalPos.y;
			break;
		case Direction.Down:
			newX=startPos.x + offsetX; newY=startPos.y + offsetY - e;
			// don't exceed the final position
			if (newY < finalPos.y) newY = finalPos.y;
			break;
		case Direction.Left:
			newX=startPos.x + offsetX - e; newY=startPos.y + offsetY;
			// don't exceed the final position
			if (newX < finalPos.x) newX = finalPos.x;
			break;
		case Direction.Right:
			newX=startPos.x + offsetX + e; newY=startPos.y + offsetY;
			// don't exceed the final position
			if (newX > finalPos.x) newX = finalPos.x;
			break;
		}
		
		switch (elem)
		{
		case Element.TRANSFORM:
			Vector3 pos = transform.position;
			pos.x = newX;
			pos.y = newY;
			transform.position = pos;
			break;
		case Element.GUI_TEXT:
			Vector2 pOffset = guiText.pixelOffset;
			pOffset.x = newX;
			pOffset.y = newY;
			guiText.pixelOffset = pOffset;
			break;
		case Element.GUI_TEXTURE:
			Rect pInset = guiTexture.pixelInset;
			pInset.x = newX;
			pInset.y = newY;
			guiTexture.pixelInset = pInset;
			break;
		}
	}
	
} // class