#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
#define UNITY_4_AND_LATER
#endif
using UnityEngine;

public class FireBall : MonoBehaviour, IPausable {
	
	private Vector2 dir = Vector2.zero;
	private float rotAnimGrades = 28f;
	private Patrol patrol;
	private Jump jump;
	private ChipmunkShape shape;
	private ChipmunkBody body;
	private bool doNotResume;
	
	void Awake () {
		jump = GetComponent<Jump>();
		patrol = GetComponent<Patrol>();
		shape = GetComponent<ChipmunkShape>();
		body = GetComponent<ChipmunkBody>();
	}

	void Start () {
		PauseGameManager.Instance.register(this as IPausable, gameObject);
	}

	void OnDestroy () {
		PauseGameManager.Instance.remove(this as IPausable);
	}
	
	void Update () {
		applyTransform();
	}

	private void applyTransform () {
		body.transform.rotation = Quaternion.AngleAxis(rotAnimGrades * Mathf.Rad2Deg, Vector3.forward);;
        body.ApplyImpulse(dir, Vector2.zero);
	}
	
	/**
	 * Self implementation for destroy since using GamObject.Destroy() has a performance hit in android.
	 */
	private void destroy () {
		shape.enabled = false; // makes the shape to be removed from the space
		GameObjectTools.ChipmunkBodyDestroy(body);
#if UNITY_4_AND_LATER
		gameObject.SetActive(false);
#else
		gameObject.SetActiveRecursively(false);
#endif
		PauseGameManager.Instance.remove(this as IPausable);
	}
	
	public bool DoNotResume {
		get {return doNotResume;}
		set {doNotResume = value;}
	}
	
	public void beforePause () {}
	
	public void afterResume () {}
	
	public bool isSceneOnly () {
		// used for allocation in subscriber lists managed by PauseGameManager
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
