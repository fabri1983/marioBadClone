using UnityEngine;

public class OptionLoadLevel : MonoBehaviour, ITouchListener, IPausable, ITransitionListener {
	
	// index of the scene to be loaded
	public int sceneIndex;

	void Awake () {
		PauseGameManager.Instance.register(this);
		TransitionGUIFxManager.Instance.register(this, false);
	}
	
	void OnDestroy () {
		TouchEventManager.Instance.removeListener(this);
		PauseGameManager.Instance.remove(this);
		TransitionGUIFxManager.Instance.remove(this);
	}
	
	/**
	 * This only fired on PC
	 */
	void OnMouseUpAsButton () {
		optionSelected();
	}
	
	public bool isStatic () {
		return true;
	}
	
	public GameObject getGameObject () {
		return gameObject;
	}
	
	public Rect getScreenBoundsAA () {
		return guiText.GetScreenRect(Camera.main);
	}
	
	public void OnBeganTouch (Touch t) {
		optionSelected();
	}
	
	public void OnStationaryTouch (Touch t) {}
	
	public void OnEndedTouch (Touch t) {
		optionSelected();
	}
	
	private void optionSelected() {
		LevelManager.Instance.loadLevel(sceneIndex);
	}
	
	public void pause () {
		gameObject.SetActiveRecursively(false);
	}
	
	public void resume () {
		gameObject.SetActiveRecursively(true);
	}
	
	public bool isSceneOnly () {
		return true;
	}
	
	public TransitionGUIFx[] getTransitions () {
		return GetComponents<TransitionGUIFx>();
	}
	
	public void onEndTransition (TransitionGUIFx fx) {
		// register with touch event manager once the transition finishes since the manager
		// depends on final element's position
		TouchEventManager.Instance.register(this, TouchPhase.Began);
	}
}
