using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// class to test it, just attach this script onto a Game Object having a mesh behaving like a 2d mesh (meaning its 
/// vertices ar arranged all in the same z coordinate) and you'll see the algorithm in action.
/// </summary>
public class BayazitPolygonDecomposerTester: MonoBehaviour {
	
	public Color gizmosColor = Color.green;
	public bool show = true;

	private List<Vector2[]> convexPolys = null;

	private void convexDecomp () {
		Mesh col = null;
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter != null)
			col = meshFilter.mesh;
		if (col == null) {
			Debug.LogError("There is no 'Mesh' attached");
			convexPolys = new List<Vector2[]>(1);
			return;
		}

		List<Vector2> worldColPoints = new List<Vector2>(col.vertices.Length);
		Transform thisTransform = transform;
		foreach(Vector2 point in col.vertices) {
			Vector2 currentWorldPoint = thisTransform.TransformPoint(point);
			worldColPoints.Add(currentWorldPoint);
		}
		
		convexPolys = BayazitPolygonDecomposer.ConvexPartition(worldColPoints);
	}

	void OnDrawGizmos() {
		if (!Application.isPlaying || !show)
			return;

		Gizmos.color = gizmosColor;

		if (convexPolys == null)
			convexDecomp();

		foreach(Vector2[] convexPoly in convexPolys) {
			for (int i = 0, c = convexPoly.Length; i < c; ++i) {
				Vector2 currentVertex = convexPoly[i];
				int j = i + 1 >= c ? 0 : i + 1;
				Vector2 nextVertex = convexPoly[j];

				Gizmos.DrawLine(currentVertex, nextVertex);
			}
		}
	}

}