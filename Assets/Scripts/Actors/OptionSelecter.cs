using UnityEngine;

/// <summary>
/// Option selecter logic for a game object with a GUI element.
/// This script only enabled from and outside event, in this case is the onLastEffectEnd() event.
/// </summary>
public class OptionSelecter : MonoBehaviour, IEffectListener {

	public OptionLoadLevel optionLoadLevel;
	public bool beginSelected = false;
	public OptionSelecter aboveSelecter, belowSelecter, leftSelecter, rightSelecter;
	
	private Effect guiSelector = null;
	
	void Awake () {
		guiSelector = GetComponent<Effect>();
		// when the OptionText effect finishes then call this effect
		EffectPrioritizerHelper.registerAsEndEffect(this as IEffectListener);
	}
	
	void Start () {
		// this initialization depends whether we have another effect we wait for ends.
		unselect();
	}
	
	void Update () {
		// read the gamepad input and check what is the next menu option the user is browesing
		OptionSelecter selecterNext = null;
		if (Gamepad.Instance.isUp())
			selecterNext = aboveSelecter;
		else if (Gamepad.Instance.isDown())
			selecterNext = belowSelecter;
		else if (Gamepad.Instance.isRight())
			selecterNext = rightSelecter;
		else if (Gamepad.Instance.isLeft())
			selecterNext = leftSelecter;
		
		// unselect this option and continue with next
		if (selecterNext != null) {
			unselect();
			selecterNext.select();
		}
	}
	
	public Effect[] getEffects () {
		return optionLoadLevel.GetComponents<Effect>();
	}
	
	public void onLastEffectEnd () {
		if (beginSelected)
			select();
	}
	
	private void unselect () {
		this.enabled = false;
		guiSelector.endEffect();
		optionLoadLevel.setSelected(false);
	}
	
	private void select () {
		this.enabled = true;
		guiSelector.startEffect();
		optionLoadLevel.setSelected(true);
	}
}
