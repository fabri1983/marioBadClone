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

	#if UNITY_EDITOR
	public Color _gizmoColor = Color.cyan;
	#endif

	public Vector2[] _verts;
	public Vector2[] _hull;
	public float _radius = 0f;

	protected Vector2[] MakeVerts(){
		// get the hull data
		Vector2[] hullTemp = hull;
		int count = hullTemp.Length;
		// allocate space for vertexes
		Vector2[] vertsTemp = new Vector2[count];
		// transform every vertex according body position
		Matrix4x4 bmatrix = BodyRelativeMatrix(body);
		for(int i=0; i<count; i++){
			vertsTemp[i] = bmatrix.MultiplyPoint3x4(hullTemp[i]);
		}
		return vertsTemp;
	}
	
	/// The vertexes of the polygon shape.
	/// The vertexes will be made into a convex hull for you automatically.
	public Vector2[] verts {
		get {
			if (_verts == null) _verts = defaultVerts;
			return _verts; 
		}
		set {
			_verts = value;

			int hullCount = CP.MakeConvexHull(_verts.Length, _verts, 0);
			_hull = new Vector2[hullCount];
			Array.Copy(_verts, _hull, hullCount);
			
			// If the C side is already initialized, need to update the existing vertexes. 
			if (_handle != IntPtr.Zero && space != null){
				Vector2[] transformed = MakeVerts();
				CP.UpdateConvexPolyShapeWithVerts(_handle, transformed.Length, transformed);
				CP.cpSpaceReindexShape(space._handle, _handle);
				if (body != null)
					body.Activate();
			}
		}
	}

	/// Vertexes of the convex hull of the polygon.
	/// This is the actual shape of the polygon as it will collide with other shapes.
	public Vector2[] hull {
		get {
			// in case no vertices has been set yet
			if (_hull == null) _hull = defaultVerts;
			return _hull;
		}
	}
	
	/// Beveling radius of a polygon shape relative to it's transform.
	/// This is the extra thickness added onto the outside of the polygons perimeter.
	public float radius {
		get { return _radius; }
		set {
			_radius = value;
			CP.cpPolyShapeSetRadius(_handle, _maxScale*_radius);
		}
	}

	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
		base.Awake();

		// in case no vertices have been set yet
		if (_verts == null)
			_verts = defaultVerts;
		
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
		Gizmos.color = _gizmoColor;
		Gizmos.matrix = transform.localToWorldMatrix;
		
		if (_handle == IntPtr.Zero)
			Awake();

		Vector2[] hullTemp = hull;
		int count = hullTemp.Length;
		for(int i=0, j=count-1; i<count; j=i, i++){
			Gizmos.DrawLine(hullTemp[i], hullTemp[j]);
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
