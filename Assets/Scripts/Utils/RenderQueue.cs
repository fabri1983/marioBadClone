using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
public class RenderQueue : MonoBehaviour {
	
	public EnumRenderQueue baseQueue = EnumRenderQueue.Unaffected_0;
	public int offset = 0;
	
#if UNITY_EDITOR
	private Material mat;
#endif
	
	// Use this for initialization
	void Start () {
		Material m = GetComponent<Renderer>().sharedMaterial;
		if (m)
			m.renderQueue = (int)baseQueue + offset;
#if UNITY_EDITOR
		mat = m;
#endif
	}
	
#if UNITY_EDITOR
	void Update () {
		// if not playing from inside the editor: update in case any Inspector property changes
		if (!Application.isPlaying && mat != null)
			mat.renderQueue = (int)baseQueue + offset;
	}
#endif
}
