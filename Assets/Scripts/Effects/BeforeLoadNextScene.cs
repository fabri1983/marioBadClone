using UnityEngine;
using System.Collections.Generic;

public class BeforeLoadNextScene : MonoBehaviour {

	private SceneNameWithIndex targetScene;
	private List<Effect> effects;
	
	void Awake () {
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
		for (int i=0,c=effects.Count; i < c; ++i) {
			if (effects[i] != null && effects[i].enabled) {
				allFinish = false;
				break;
			}
		}
		return allFinish;
	}
	
	public void setScene (SceneNameWithIndex value) {
		targetScene = value;
	}
	
	public void execute () {
		this.enabled = true;
		
		effects = BeforeLoadNextSceneManager.Instance.getEffects();
		if (effects == null)
			return;
		
		for (int i=0,c=effects.Count; i < c; ++i) {
			if (effects[i] != null && effects[i].beforeLoadNextScene) {
				effects[i].startEffect();
			}
		}
	}
}
