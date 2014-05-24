using UnityEngine;

public class MovingCube : MonoBehaviour {
	
	public Transform startPos;
	public Transform endPos;
	public float movAmount = 5f; 
	
	private Vector3 adder;
	private float dirSign;
	
	// Use this for initialization
	void Start () {
	
		adder = new Vector3(movAmount, 0, 0);
		dirSign = -1;
	}
	
	// Update is called once per frame
	void Update () {
		
		// move the platform
		transform.position = Vector3.Lerp(transform.position, transform.position + (dirSign * adder), Time.deltaTime * 3f);
	}
	
	void OnTriggerEnter (Collider collider)  {

		if (collider.name.Equals("StartPos"))
			dirSign = 1;
		else if (collider.name.Equals("EndPos"))
			dirSign = -1;
	}
}
