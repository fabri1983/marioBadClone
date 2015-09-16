using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Add this script to the component you want to wait perform until some effects finishes.
/// Those Effect components marked as executed before load next scene are automatically registered 
/// into the BeforeLoadNextSceneManager. Then any script having a reference to BeforeLoadNextScene must 
/// call execute() in order to trigger the effects.
/// </summary>
public class BeforeLoadNextScene : MonoBehaviour {

	private KScenes targetScene;
	private Effect[] effects;
	
	void Awake () {
		// starts disabled since the level load only is valid when all effects finished
		this.enabled = false;
	}
	
	void Update () {
		if (allEffectsFinish()) {
			this.enabled = false;
			effects = null;
			LevelManager.Instance.loadLevel(targetScene);
		}
	}
	
	private bool allEffectsFinish () {
		if (effects == null)
			return true;
		
		// traverse all effects and check all of them are finished (enabled == false)
		bool allFinish = true;
		for (int i=0,c=effects.Length; i < c; ++i) {
			if (effects[i] != null && effects[i].enabled) {
				allFinish = false;
				break;
			}
		}
		return allFinish;
	}
	
	public void setScene (KScenes value) {
		targetScene = value;
	}
	
	public void execute () {
		this.enabled = true;
		effects = BeforeLoadNextSceneManager.Instance.getEffects();
		BeforeLoadNextSceneManager.Instance.executeEffects();
	}
}
