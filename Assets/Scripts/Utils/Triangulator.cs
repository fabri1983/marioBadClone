using UnityEngine;
using System.Collections.Generic;
using System.Collections;
 
public class Triangulator {
	
	private List<Vector2> m_points = new List<Vector2> ();
	 
	public Triangulator (Vector2[] points)
	{
		m_points = new List<Vector2> (points);
	}
	 
	public int[] Triangulate ()
	{
		List<int> indices = new List<int> ();
	 
		int n = m_points.Count;
		if (n < 3)
			return indices.ToArray ();
	 
		int[] V = new int[n];
		if (Area () > 0) {
			for (int v = 0; v < n; v++)
				V [v] = v;
		} else {
			for (int v = 0; v < n; v++)
				V [v] = (n - 1) - v;
		}
	 
		int nv = n;
		int count = 2 * nv;
		for (int m = 0, v = nv - 1; nv > 2;) {
			if ((count--) <= 0)
				return indices.ToArray ();
	 
			int u = v;
			if (nv <= u)
				u = 0;
			v = u + 1;
			if (nv <= v)
				v = 0;
			int w = v + 1;
			if (nv <= w)
				w = 0;
	 
			if (Snip (u, v, w, nv, V)) {
				int a, b, c, s, t;
				a = V [u];
				b = V [v];
				c = V [w];
				indices.Add (a);
				indices.Add (b);
				indices.Add (c);
				m++;
				for (s = v, t = v + 1; t < nv; s++, t++)
					V [s] = V [t];
				nv--;
				count = 2 * nv;
			}
		}
	 
		indices.Reverse ();
		return indices.ToArray ();
	}
	 
	private float Area ()
	{
		int n = m_points.Count;
		float A = 0.0f;
		for (int p = n - 1, q = 0; q < n; p = q++) {
			Vector2 pval = m_points [p];
			Vector2 qval = m_points [q];
			A += pval.x * qval.y - qval.x * pval.y;
		}
		return (A * 0.5f);
	}
	 
	private bool Snip (int u, int v, int w, int n, int[] V)
	{
		int p;
		Vector2 A = m_points [V [u]];
		Vector2 B = m_points [V [v]];
		Vector2 C = m_points [V [w]];
		if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
			return false;
		for (p = 0; p < n; p++) {
			if ((p == u) || (p == v) || (p == w))
				continue;
			Vector2 P = m_points [V [p]];
			if (InsideTriangle (A, B, C, P))
				return false;
		}
		return true;
	}
	 
	private bool InsideTriangle (Vector2 A, Vector2 B, Vector2 C, Vector2 P)
	{
		float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
		float cCROSSap, bCROSScp, aCROSSbp;
	 
		ax = C.x - B.x;
		ay = C.y - B.y;
		bx = A.x - C.x;
		by = A.y - C.y;
		cx = B.x - A.x;
		cy = B.y - A.y;
		apx = P.x - A.x;
		apy = P.y - A.y;
		bpx = P.x - B.x;
		bpy = P.y - B.y;
		cpx = P.x - C.x;
		cpy = P.y - C.y;
	 
		aCROSSbp = ax * bpy - ay * bpx;
		cCROSSap = cx * apy - cy * apx;
		bCROSScp = bx * cpy - by * cpx;
	 
		return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
	}
	 
	public static Mesh CreateMesh3D (Vector2[] poly, float extrusion)
	{
		// convert polygon to triangles
		Triangulator triangulator = new Triangulator (poly);
		int[] traingulatedTris = triangulator.Triangulate ();
		Mesh m = new Mesh ();
		Vector3[] vertices;
		if (extrusion == 0f) vertices = new Vector3[poly.Length];
		else vertices = new Vector3[poly.Length * 2];
		
		for (int i = 0; i < poly.Length; i++) {
			vertices [i].x = poly [i].x;
			vertices [i].y = poly [i].y;
			vertices [i].z = -extrusion; // front vertex
			if (extrusion != 0f) {
				vertices [i + poly.Length].x = poly [i].x;
				vertices [i + poly.Length].y = poly [i].y;
				vertices [i + poly.Length].z = extrusion; // back vertex
			}
		}
	 
		int[] triangles;
		if (extrusion == 0f) triangles = new int[traingulatedTris.Length];
		else triangles = new int[traingulatedTris.Length * 2 + poly.Length * 6];
		
		int count_tris = 0;
		for (int i = 0; i < traingulatedTris.Length; i += 3) {
			triangles [i] = traingulatedTris [i];
			triangles [i + 1] = traingulatedTris [i + 1];
			triangles [i + 2] = traingulatedTris [i + 2];
		} // front vertices
	 
		if (extrusion != 0f) {
			count_tris += traingulatedTris.Length;
			for (int i = 0; i < traingulatedTris.Length; i += 3) {
				triangles [count_tris + i] = traingulatedTris [i + 2] + poly.Length;
				triangles [count_tris + i + 1] = traingulatedTris [i + 1] + poly.Length;
				triangles [count_tris + i + 2] = traingulatedTris [i] + poly.Length;
			} // back vertices
		}
		
		//texture coordinate
		Vector2[] uvs = new Vector2[vertices.Length];
		for (int i=0,c=uvs.Length; i < c; ++i) {
			uvs[i].x = vertices[i].x;
			uvs[i].y = vertices[i].y;
		}

		m.vertices = vertices;
		m.triangles = triangles;
		m.uv = uvs;
		if (extrusion != 0f)
			m = Triangulator.SideExtrusion (m);
		m.RecalculateNormals ();
		m.RecalculateBounds ();
		m.Optimize ();
	 
		return m;
	}
	 
	private static Mesh SideExtrusion (Mesh mesh)
	{
		List<int> indices = new List<int> (mesh.triangles);
		int count = (mesh.vertices.Length / 2);
		for (int i = 0; i < count; i++) {
			int i1 = i;
			int i2 = (i1 + 1) % count;
			int i3 = i1 + count;
			int i4 = i2 + count;
	 
			indices.Add (i4);
			indices.Add (i3);
			indices.Add (i1);
	 
			indices.Add (i2);
			indices.Add (i4);
			indices.Add (i1);
		}
		mesh.triangles = indices.ToArray ();
		return mesh;
	}
}
