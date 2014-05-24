using UnityEngine;

public class RotatingCube : MonoBehaviour {

	public float rotAngle = 30f;
	
	private float rotationsign;
	
	// Use this for initialization
	void Start () {
	
		rotationsign = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
		// damp the rotation around the z-axis along time
		float currentRotationAngle = transform.eulerAngles.z;
		currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, rotationsign * rotAngle, Time.deltaTime * 2f);
		// convert the angle into a rotation
		transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, currentRotationAngle);
		
		if (transform.rotation.eulerAngles.z >= (rotAngle - 0.1f) && transform.rotation.eulerAngles.z < (360f - rotAngle) && rotationsign > 0) {
			rotationsign = -1;
		}
		else if (transform.rotation.eulerAngles.z <= (360f - rotAngle + 0.1f) && transform.rotation.eulerAngles.z > rotAngle && rotationsign < 0) {
			rotationsign = 1;
		}
	}
}
