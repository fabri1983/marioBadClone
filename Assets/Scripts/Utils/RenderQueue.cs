using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
public class RenderQueue : MonoBehaviour {
	
	public EnumRenderQueue baseQueue = EnumRenderQueue.Background;
	public int offset = 0;
	
#if UNITY_EDITOR
	private Material mat;
#endif
	
	// Use this for initialization
	void Start () {
		Material m = renderer.sharedMaterial;
		if (m)
			m.renderQueue = (int)baseQueue + offset;
#if UNITY_EDITOR
		mat = m;
#endif
	}
	
#if UNITY_EDITOR
	void Update () {
		if (mat)
			mat.renderQueue = (int)baseQueue + offset;
		Debug.Log((int)baseQueue + offset);
	}
#endif
}
