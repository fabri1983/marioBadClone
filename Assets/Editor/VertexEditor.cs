using UnityEngine;
using System.Collections;
     
[AddComponentMenu("Mesh/Vertex Editor")]
[ExecuteInEditMode]
public class VertexEditor : MonoBehaviour {
	
	public bool _destroy;
	
	private Mesh mesh;
	private Vector3[] verts;
	private Vector3 vertPos;
	private GameObject[] handles;
	
	void OnEnable ()
	{
		/// This executed once when the menu option is selected
		/// It creates a game object in scene per mesh vertex

		mesh = GetComponent<MeshFilter>().mesh; // don't use sharedMesh if you only want to edit current game object mesh
		verts = mesh.vertices;
		handles = new GameObject[verts.Length];
		for (int i=0,c=verts.Length; i<c; ++i) {
			Vector3 vert = verts[i];
			vertPos = transform.TransformPoint (vert);
			GameObject handle = new GameObject();
			// handle.hideFlags = HideFlags.DontSave;
			handle.transform.position = vertPos;
			handle.transform.parent = transform;
			// creates the gizmo
			handle.AddComponent<VertHandleGizmo> ()._parent = this;
			handles[i] = handle;
		}
	}
     
	void OnDisable ()
	{
		for (int i=0,c=handles.Length; i<c; ++i) {
			DestroyImmediate (handles[i]);
		}
	}
     
	void Update ()
	{
		if (_destroy) {
			_destroy = false;
			DestroyImmediate (this);
			return;
		}
     
		for (int i = 0; i < verts.Length; i++) {
			verts [i] = handles [i].transform.localPosition;
		}
     
		mesh.vertices = verts;
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
	}
     
}
     
[ExecuteInEditMode]
public class VertHandleGizmo : MonoBehaviour {
	
	public float _size = CURRENT_SIZE;
	public VertexEditor _parent;
	public bool _destroy;
	
	private static float CURRENT_SIZE = 0.4f;
	private float _lastKnownSize = CURRENT_SIZE;
     
	void Update ()
	{
		// Change the size if the user requests it
		if (_lastKnownSize != _size) {
			_lastKnownSize = _size;
			CURRENT_SIZE = _size;
		}
     
		// Ensure the rest of the gizmos know the size has changed...
		if (CURRENT_SIZE != _lastKnownSize) {
			_lastKnownSize = CURRENT_SIZE;
			_size = _lastKnownSize;
		}
     
		if (_destroy)
			DestroyImmediate (_parent);
	}
     
	void OnDrawGizmos ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawCube (transform.position, Vector3.one * CURRENT_SIZE);
	}
}