using UnityEngine;
using System.Collections;

public class CameraBlackBars : MonoBehaviour {

	void Awake () {
		if (LevelManager.keepAspectRatio)
			DontDestroyOnLoad(gameObject);
	}
}
