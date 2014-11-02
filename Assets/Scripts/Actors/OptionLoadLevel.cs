using UnityEngine;

public class OptionLoadLevel : MonoBehaviour, ITouchListener, IPausable, ITransitionListener {
	
	// index of the scene to be loaded
	public int sceneIndex;
	
	private 
	void Awake () {
		PauseGameManager.Instance.register(this);
		TransitionGUIFxManager.Instance.register(this, false);
	}
	
	void OnDestroy () {
		PauseGameManager.Instance.remove(this);
		TransitionGUIFxManager.Instance.remove(this);
		TouchEventManager.Instance.removeListener(this);
	}
	
	public bool isStatic () {
		// for event touch listener
		return true;
	}
	
	public GameObject getGameObject () {
		return gameObject;
	}
	
	public Rect getScreenBoundsAA () {
		// here I suppose this game object has attached a GUICustomElement
		return GUIScreenLayoutManager.positionInScreen(GetComponent<GUICustomElement>());
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
	
#if UNITY_EDITOR
	void OnGUI () {
		// since this game object has a GUICustomElement script attached to it, for strange a reason no mouse event 
		// is caught, so we need to manually check for the event and fire it here
		Event e = Event.current;
		if (e != null && e.isMouse && e.button == 0 && e.type == EventType.MouseUp) {
			if (GameObjectTools.testHitFromMousePos(transform, e.mousePosition))
				optionSelected();
		}
	}
#endif
}
