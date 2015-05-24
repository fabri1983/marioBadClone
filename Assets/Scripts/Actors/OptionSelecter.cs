using UnityEngine;

public class OptionSelecter : MonoBehaviour, IEffectListener {

	public bool beginSelected = false;
	public OptionSelecter aboveSelecter, belowSelecter, leftSelecter, rightSelecter;
	
	private OptionLoadLevel optLoadLevel;
	
	void Awake () {
		optLoadLevel = transform.parent.gameObject.GetComponentInChildren<OptionLoadLevel>();
		unselect();
		EffectPrioritizerHelper.registerForEndEffect(this);
	}
	
	void Update () {
		// read the gamepad input and check what is the next menu option the user is browesing
		OptionSelecter selecterNext = null;
		if (Gamepad.isUp())
			selecterNext = aboveSelecter;
		else if (Gamepad.isDown())
			selecterNext = belowSelecter;
		else if (Gamepad.isRight())
			selecterNext = rightSelecter;
		else if (Gamepad.isLeft())
			selecterNext = leftSelecter;
		
		// unselect this option and continue with next
		if (selecterNext != null) {
			unselect();
			selecterNext.select();
		}
	}
	
	public Effect[] getEffects () {
		// return the transitions in an order set from Inspector.
		// Note: to return in a custom order get the transitions array and sort it as desired.
		return EffectPrioritizerHelper.getEffects(transform.parent.gameObject, true);
	}
	
	public void onLastEffectEnd () {
		if (beginSelected)
			select();
	}
	
	private void unselect () {
		this.enabled = false;
		renderer.enabled = false; // hide the GUI selecter
		optLoadLevel.setSelected(false);
	}
	
	private void select () {
		this.enabled = true;
		renderer.enabled = true; // show the GUI selecter
		optLoadLevel.setSelected(true);
	}
}
