using UnityEngine;
using System.Collections;

public class CameraBlackBars : MonoBehaviour {

	void Awake () {
		DontDestroyOnLoad(gameObject);
	}
}
