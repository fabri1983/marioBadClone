using UnityEngine;

public class MarioDieAnim : MonoBehaviour {

	private int currentLayer;
	private int currentLayerChild;
	private bool dying = false;
	private float endPosition = 0f;
	private Jump jump;
	private static Component[] children;

	void Awake () {
		children = gameObject.GetComponentsInChildren<Component>();
		jump = GetComponent<Jump>();
	}

	void LateUpdate () {
		if (!dying)
			return;
		
		// end of animation?
		if (transform.position.y < endPosition) {
			dying = false;
			// set back mario's original layer
			gameObject.layer = currentLayer;
			for (int i=0; i < children.Length; ++i)
				children[i].gameObject.layer = currentLayerChild;
			// restart level
			LevelManager.Instance.loseGame(false);
		}
	}
	
	public void startAnimation () {
		
		dying = true;
		
		// change Mario's current layer
		currentLayer = gameObject.layer;
		gameObject.layer = LevelManager.CAMERA_IN_FRONT;
		for (int i=0; i < children.Length; ++i)
		{
			// only keeps last child's layer since they are all the same (should be)
			currentLayerChild = children[i].gameObject.layer;
			children[i].gameObject.layer = LevelManager.CAMERA_IN_FRONT;
		}

		// execute die animation
		endPosition = transform.position.y + LevelManager.ENDING_DIE_ANIM_Y_POS;
		jump.jump(25f, 55f);
	}
	
	public bool isDying () {
		return dying;
	}
}
