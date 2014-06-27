using UnityEngine;

public class FireBall : MonoBehaviour, IPausable {
	
	private Vector2 dir = Vector2.zero;
	private float rotAnimGrades = 28f;
	private Patrol patrol;
	private Jump jump;
	private ChipmunkShape shape;
	private ChipmunkBody body;
	
	void Awake () {
		jump = GetComponent<Jump>();
		patrol = GetComponent<Patrol>();
		shape = GetComponent<ChipmunkShape>();
		body = GetComponent<ChipmunkBody>();
		PauseGameManager.Instance.register(this);
	}
	
	void Update () {
		applyRotation();
	}
	
	void OnDestroy () {
		PauseGameManager.Instance.remove(this);
	}

	private void applyRotation () {
        // usar body y dir aca
	}
	
	/**
	 * Self implementation for destroy since using GamObject.Destroy() has a performance hit in android.
	 */
	private void destroy () {
		shape.enabled = false; // makes the shape to be removed from the space
		GameObjectTools.ChipmunkBodyDestroy(body);
		gameObject.SetActiveRecursively(false);
		PauseGameManager.Instance.remove(this);
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
	
	public void setDir (Vector2 pDir) {
		dir = pDir;
		patrol.setNewDir(pDir.x);
	}
	
	public void setSpeed (float speed) {
		patrol.setMoveSpeed(speed);
	}
	
	public void setBouncing (bool val) {
		if (val) {
			jump.setForeverJump(true);
			jump.setForeverJumpSpeed(20f);
		}			
		else
			jump.setForeverJump(false);
	}
	
	public void setDestroyTime(float time) {
		if (time >= 0f)
			Invoke("destroy", time);
	}
	
	public void addTargetLayer (int val) {
		// ACA HACER ACTUALIZAR LOS LAYERS DEL SHAPE y tmb del gameobject
		// convertir a uint
		//GameObjectTools.setLayer(gameObject, playerLayer);
		//GameObjectTools.setLayerForShapes(gameObject, layersCP);
	}
}
