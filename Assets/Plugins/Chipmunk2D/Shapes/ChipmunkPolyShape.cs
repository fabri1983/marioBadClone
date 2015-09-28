// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;	
using CP = ChipmunkBinding;
using System;
using System.Runtime.InteropServices;

/// Chipmunk convex polygon shape type.
public class ChipmunkPolyShape : ChipmunkShape {
	protected static Vector2[] defaultVerts = new Vector2[]{
		new Vector2( 0, 1),
		new Vector2( 1, 0),
		new Vector2(-1, 0)
	};
	
	[HideInInspector]
	public Vector2[] _verts = defaultVerts;
	[HideInInspector]
	public Vector2[] _hull = defaultVerts;
	
	#if UNITY_EDITOR
	private bool skipHullOldValue;
	#endif
	
	protected Vector2[] MakeVerts(){
		int count = _hull.Length;
		Vector2[] verts = new Vector2[count];
		Matrix4x4 bmatrix = BodyRelativeMatrix(body);
		
		for(int i=0; i<count; i++){
			verts[i] = bmatrix.MultiplyPoint3x4(_hull[i]);
		}
		
		return verts;
	}
	
	/// The vertexes of the polygon shape.
	/// The vertexes will be made into a convex hull for you automatically.
	public Vector2[] verts {
		get { return (Vector2[])_verts.Clone(); }
		set {
			_verts = (Vector2[])value.Clone();
			
			Vector2[] hull = (Vector2[])verts.Clone();
			int hullCount = CP.MakeConvexHull(verts.Length, hull, 0);
			_hull = new Vector2[hullCount];
			Array.Copy(hull, _hull, hullCount);
			
			// If the C side is already initialized, need to update the existing vertexes. 
			if(_handle != IntPtr.Zero){
				Vector2[] transformed = MakeVerts();
				CP.UpdateConvexPolyShapeWithVerts(_handle, transformed.Length, transformed);
				CP.cpSpaceReindexShape(space._handle, _handle);
				
				if(body != null) body.Activate();
			}
		}
	}
	
	/// Vertexes of the convex hull of the polygon.
	/// This is the actual shape of the polygon as it will collide with other shapes.
	public Vector2[] hull {
		get { return (Vector2[])_hull.Clone(); }
	}
	
	public float _radius = 0f;
	
	/// Beveling radius of a polygon shape relative to it's transform.
	/// This is the extra thickness added onto the outside of the polygons perimeter.
	public float radius {
		get { return _radius; }
		set {
			_radius = value;
			CP.cpPolyShapeSetRadius(_handle, _maxScale*_radius);
		}
	}
	
	public void generateVertsAndHull () {

		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter == null) {
			
		} else {
			Mesh mesh = meshFilter.sharedMesh;
			if (mesh == null) {
				this.verts = _verts; // if no mesh then use default verts
			} else {
				// copy the current Mesh vertices
				Vector3[] vertices3 = mesh.vertices;
				Vector2[] vertices2 = new Vector2[vertices3.Length];
				for (int i=0, c=vertices3.Length; i < c; ++i)
					vertices2[i] = (Vector2)vertices3[i];
				// this force the generation of the hull structure
				this.verts = vertices2;
				// we dont need it anymore since is cloned in the set property
				vertices2 = null;
			}
		}
	}
	
	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
		base.Awake();
		
		this.verts = _verts;
		
		Vector2[] transformed = MakeVerts();
		_handle = CP.NewConvexPolyShapeWithVerts(transformed.Length, transformed);
		CP.cpPolyShapeSetRadius(_handle, _maxScale*_radius);
		if(body != null) body._AddMassForShape(this);
		
		GCHandle gch = GCHandle.Alloc(this);
		CP._cpShapeSetUserData(_handle, GCHandle.ToIntPtr(gch));
	}
	
	public override ChipmunkBody _UpdatedTransform(){
		UpdateParentBody();
		
		// Force the properties to update themselves.
		this.verts = this.verts;
		this.radius = this.radius;
		
		return body;
	}
	
#if UNITY_EDITOR
	protected void OnDrawGizmosSelected(){
		Gizmos.color = this.gizmoColor;
		Gizmos.matrix = transform.localToWorldMatrix;
		
		int count = _hull.Length;
		for(int i=0, j=count-1; i<count; j=i, i++){
			Gizmos.DrawLine(_hull[i], _hull[j]);
		}
	}	
#endif
	
//	public int vertexCount {
//		get { return CP.cpPolyShapeGetNumVerts(_handle); }
//	}
//	
//	public Vector2 GetVertex(int i){
//		// TODO needs to do bounds checking to avoid an abort()
//		return CP.cpPolyShapeGetVert(_handle, i);
//	}
}
