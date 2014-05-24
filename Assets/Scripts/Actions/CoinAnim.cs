using UnityEngine;

public class CoinAnim : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
		// rotate around X
		transform.Rotate(-90f, 0f, 0f, Space.World);
	}
	
	// Update is called once per frame
	void Update () {
		
		// moves the coin upwards
		transform.Translate(0f, 4f * Time.deltaTime, 0f, Space.World);
	}
}
