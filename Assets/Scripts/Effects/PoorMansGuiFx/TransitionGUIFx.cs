// PoorMansGUIFX by UnityCoder.com
// Modifications added by fabri1983@gmail.com

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	// NOTE: currently only TRANSFORM is used, because I don't know exactly what values to use when working with Unity GUI components
	TRANSFORM, GUI_TEXT, GUI_TEXTURE
}

/// <summary>
/// This class modifies transform component for doing a transition effect.
/// When this script is in a GUIText or GUITexture game object, please consider your actual 
/// pixel offset and set the start offset transform property accordingly.
/// </summary>
public class TransitionGUIFx : Effect {
	
	public Vector2 startOffsetTransform = Vector2.zero;
	public Transition _transition;
	public Direction direction;
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

	protected override void ownAwake () {
		update = false;
	}
	
	protected override void ownOnDestroy () {
	}
	
	void OnDisable () {
		if (useCoroutine)
			StopCoroutine("DoCoroutine");
	}

	protected override void ownEffectStarts () {
		elem = Element.TRANSFORM;
		// NOTE: currently only TRANSFORM is used, because I don't know exactly what values to use when working with Unity GUI components
		/*if (guiTexture != null)
			elem = Element.GUI_TEXTURE;
		else if (guiText != null)
			elem = Element.GUI_TEXT;*/
		
		prepareTransition();
		
		currentStep = 0;
		if (useCoroutine)
			StartCoroutine("DoCoroutine");
		else
			Invoke("enableUpdate", startDelaySecs);
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
		
		// the final position is the current one
		finalPos.Set(transform.localPosition.x, transform.localPosition.y);
		
		if (elem == Element.TRANSFORM)
			startPos.Set(transform.localPosition.x + startOffsetTransform.x, transform.localPosition.y + startOffsetTransform.y);
		/*else if (elem == Element.GUI_TEXT)
			startPos.Set(guiText.pixelOffset.x + startOffsetTransform.x, guiText.pixelOffset.y + startOffsetTransform.y);
		else if (elem == Element.GUI_TEXTURE)
			startPos.Set(guiTexture.pixelInset.x + startOffsetTransform.x, guiTexture.pixelInset.y + startOffsetTransform.y);*/
		
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
		switch (elem)
		{
		case Element.TRANSFORM:
			Vector3 pos = transform.localPosition;
			pos.x = startPos.x + offsetX;
			pos.y = startPos.y + offsetY;
			transform.localPosition = pos;
			break;
		/*case Element.GUI_TEXT:
			Vector2 pOffset = guiText.pixelOffset;
			pOffset.x = startPos.x + offsetX;
			pOffset.y = startPos.y + offsetY;
			guiText.pixelOffset = pOffset;
			break;
		case Element.GUI_TEXTURE:
			Rect pInset = guiTexture.pixelInset;
			pInset.x = startPos.x + offsetX;
			pInset.y = startPos.y + offsetY;
			guiTexture.pixelInset = pInset;
			break;*/
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
			if (finalPos.x == transform.localPosition.x && finalPos.y == transform.localPosition.y)
				break;
            yield return null;
		}

		effectEnded();
	}
	
	private void DoTransition ()
	{
		// main transition/easing loop
		//if (currentStep >= steps) {
		if (finalPos.x == transform.localPosition.x && finalPos.y == transform.localPosition.y) {
			effectEnded();
			return;
		}
		transition(currentStep);
		++currentStep;
	}

	private void transition (int step)
	{
		// TODO: remap step from 0-steps into 0-1
		float linearStep = (float)step/(steps-1);
		float e = Easing.Ease(linearStep, acceleration, easingType);
		float newX=0f, newY=0f;
		
		switch (direction)
		{
		case Direction.Up:
			newX=startPos.x + offsetX; newY=startPos.y + offsetY + e;
			// don't exceed the final position
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
			Vector3 pos = transform.localPosition;
			pos.x = newX;
			pos.y = newY;
			transform.localPosition = pos;
			break;
		/*case Element.GUI_TEXT:
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
			break;*/
		}
	}
}