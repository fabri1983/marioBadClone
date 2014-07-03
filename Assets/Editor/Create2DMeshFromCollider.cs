using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Create a 2D traingulated mesh from a mesh generated from 2D ColliderGen v1.4.
/// When generating the mesh collider using 2d ColliderGen take note of the property z-thikness.
/// Use 1 so you don't need to set it in the wizard window of this script. Both values must match.
/// 
/// Run from unity editor. This class should be placed in Editor folder.
/// Created by Fabricio Lettieri on 02.Jul.2014
/// </summary>
[RequireComponent(typeof(Triangulator))]
public class Create2DMeshFromCollider : ScriptableWizard
{
	public enum MIRROR_DIRS
	{
		NONE,
		LEFT_TO_RIGHT,
		RIGHT_TO_LEFT,
		TOP_TO_BOTTOM,
		BOTTOM_TO_TOP
	}
	
	public MIRROR_DIRS mirrorDir = MIRROR_DIRS.NONE; // mirroring
	public float sourceZThickness = 1; // z-thickness as in the 2d collider gen
	public Mesh _Mesh = null; //mesh generated from 2D ColliderGen
	public string MeshName = "mesh"; // asset name
	public bool createGameObject = true; // choose yes or no for game object creaiotn into the scene
	public string GameObjectName = "mesh"; // game object name
	//Name of asset folder to contain quad asset when created
	public string AssetFolder = "Assets/Colliders/Generated";
	
	[MenuItem("GameObject/Create Other/2D Mesh from 2D ColliderGen")]
	static void CreateWizard ()
	{
		ScriptableWizard.DisplayWizard ("Create 2D Mesh from a 2D ColliderGen mesh", typeof(Create2DMeshFromCollider));
	}
	
	//Function called when window is created
	void OnEnable ()
	{
		//Call selection change to load asset path from selected, if any
		OnSelectionChange ();
	}
	
	//Called 10 times per second
	void OnInspectorUpdate ()
	{
	}

	//Function called when window is updated
	void OnSelectionChange ()
	{
		//Check user selection in editor - check for folder selection
		if (Selection.objects != null && Selection.objects.Length == 1) {
			//Get path from selected asset
			string assetPath = AssetDatabase.GetAssetPath (Selection.objects [0]);
			if (assetPath != null && !"".Equals (assetPath))
				AssetFolder = Path.GetDirectoryName (assetPath);
		}
	}
	
	//Function to create quad mesh
	void OnWizardCreate ()
	{
		if (_Mesh == null) {
			Debug.LogError ("You have to select a mesh");
			return;
		}
		
		/// 2d ColliderGen v1.4 takes a "z-thickness" property for the mesh generation and locates
		/// one plane at z-thicknes/2 and the other at -z-thickness/2.
		/// Also the amount of vertices we set in the "Outline Vertex Count" property is quadruplied.
		/// Eg: having an outline of 24 verts will generate a mesh collider of 92 verts.
		/// So we need to extract only frontal vertices (z = z-thickness/2) and avoid duplicates
		
		Vector3[] colliderVerts = _Mesh.vertices;
		HashSet<Vector3> vertsSet = new HashSet<Vector3> (new Vector3Comparer ());
		for (int i=0,c=colliderVerts.Length; i<c; ++i)
			vertsSet.Add (colliderVerts [i]);
		
		// the set should contain colliderVerts/2 verts according to my analysis
		if (vertsSet.Count != colliderVerts.Length / 2) {
			Debug.LogError ("Set of vertices should contain colliderVerts/2 verts. The set has " + 
				vertsSet.Count + " and colliderVerts/2 = " + colliderVerts.Length / 2);
			return;
		}
		
		// although we only take frontal vertices, some of them may be duplicated
		HashSet<Vector2> verts2DSet = new HashSet<Vector2> (new Vector2Comparer ());
		float z = sourceZThickness / 2f;
		foreach (Vector3 v in vertsSet) {
			if (v.z == z)
				verts2DSet.Add ((Vector2)v);
		}
		Vector2[] verts = new Vector2[verts2DSet.Count];
		verts2DSet.CopyTo (verts);
		
		// triangulate
		Mesh mesh = Triangulator.CreateMesh3D (verts, 0f); // use 0 for no extruding
		mesh.name = MeshName;
		
		//Create or Replace asset in database
		CreateOrReplaceAsset(mesh);
		
		//Create game object to locate into the scene
		if (createGameObject) {
			GameObject _2d_mesh = new GameObject (GameObjectName);
			MeshFilter meshFilter = (MeshFilter)_2d_mesh.AddComponent (typeof(MeshFilter));
			_2d_mesh.AddComponent (typeof(MeshRenderer));
			meshFilter.sharedMesh = mesh;
		}
		
		mesh.RecalculateBounds ();
	}
	
	private void CreateOrReplaceAsset (Mesh mesh) {
		string path = AssetDatabase.GenerateUniqueAssetPath (AssetFolder + "/" + GameObjectName) + ".asset";
		Mesh outputMesh = AssetDatabase.LoadMainAssetAtPath (path) as Mesh;
		
		// if asset exists, then copy current mesh into outputMesh, so updating the existing asset
	    if (outputMesh != null) {
		    EditorUtility.CopySerialized (mesh, outputMesh);
		    AssetDatabase.SaveAssets ();
	    }
		// asset doesn't exist, create it
	    else {
		    AssetDatabase.CreateAsset (mesh, path);
			AssetDatabase.SaveAssets ();
	    }
	}
}

class Vector2Comparer : IEqualityComparer<Vector2>
{
	public bool Equals (Vector2 a, Vector2 b)
	{
		return a.Equals (b);
	}

	public int GetHashCode (Vector2 v)
	{
		return v.GetHashCode ();
	}
}

class Vector3Comparer : IEqualityComparer<Vector3>
{
	public bool Equals (Vector3 a, Vector3 b)
	{
		return a.Equals (b);
	}

	public int GetHashCode (Vector3 v)
	{
		return v.GetHashCode ();
	}
}