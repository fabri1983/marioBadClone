using UnityEngine;

public abstract class PowerUp : MonoBehaviour, IPausable {
	
	public GameObject artifact; // Game Object that will be used as bullet/gunfire
	
	protected int usageLeft;
	protected float firePow; 
	protected float destroyTime; // destroy the game object after certain seconds since instantiated
	protected GameObject objectToAnim; // short live gameobject to show when the power up is gained
	protected ChipmunkBody body;
	protected ChipmunkShape shape;

	void Awake () {
		body = GetComponent<ChipmunkBody>();
		shape = GetComponent<ChipmunkShape>();
	}

	void Start () {
		PauseGameManager.Instance.register(this, gameObject);

		ownStart(); // invokes subclass own starting method
	}
	
	void Update () {
		// only for animate how the power up artifact shows when pops up the brick
		if (objectToAnim != null)
			animOnGotcha();
		
		// invokes implementation's specific update
		ownUpdate();
	}
	
	public void pause () {}
	
	public void resume () {}
	
	public bool isSceneOnly () {
		// used for allocation in subscriber lists managed by PauseGameManager
		return false; // the power up should be in a pool
	}
	
	/**
	 * Defines specific initiallization
	 */
	protected abstract void ownStart ();
	
	/**
	 * Defines implementation's specific update behavior
	 */
	protected abstract void ownUpdate ();
	
	/**
	 * Defines specific animation over the transform objectForAnim when the power up is gotcha
	 */
	protected abstract void animOnGotcha ();
	
	/**
	 * Assign the power up to the character/element that will make uso of it (if need to)
	 */
	public abstract void assignToCharacter (MonoBehaviour element);
	
	/**
	 * Eeach implementation knows how to proceed. This method invoked from action() method
	 */
	public abstract void ownAction (GameObject go);
	
	/**
	 * Still have some rounds?
	 */
	public abstract bool ableToUse ();
	
	/**
	 * Decrease amount of units
	 */
	public abstract void use ();
	
	/**
	 * Returns true if the curretn Input is the expected to be operable with this power up
	 */
	public abstract bool isAllowedInput ();
	
	public void action (GameObject go) {
		// checks first if input key is correct and if the power up is still usable
		if (!isAllowedInput() || !ableToUse())
			return;
		
		// invoke the implementation's specific action routine
		ownAction(go);
		
		// use() method invocation must be inside ownAction() method, because each implementation can has special behavior
	}
	
	/**
	 * Tells this power up to perform a little animation to show what power up the player has got.
	 */
	public void doGotchaAnim (Vector3 startingPos) {

		// instantiate the artifact for a short moment
		if (artifact != null) {
			objectToAnim = GameObject.Instantiate(artifact) as GameObject;
			objectToAnim.transform.position = startingPos;
			//objectToAnim.transform.parent = null;
			Invoke("destroy", 1f);
		}
	}
	
	public GameObject getArtifact () {
		return artifact;
	}
	
	public float getDestroyTime () {
		return destroyTime;
	}
	
	public float getPower () {
		return firePow;
	}
	
	void OnDestroy () {	
		GameObjectTools.ChipmunkBodyDestroy(body);
	}
	
	/**
	 * Self implementation for destroy since using GamObject.Destroy() has a performance hit in android.
	 */
	private void destroy () {
		body.enabled = false; // makes the body to be removed from the space
		shape.enabled = false; // makes the shape to be removed from the space
#if UNITY_4_AND_LATER
		gameObject.SetActive(false);
#else
		gameObject.SetActiveRecursively(false);
#endif
	}
}
