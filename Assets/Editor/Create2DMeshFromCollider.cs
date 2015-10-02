#if (UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
#define UNITY_3_AND_4
#endif

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
	public float sourceZThickness = 1; // z-thickness as in the 2d collider gen
	public Mesh meshSource = null; //mesh generated from 2D ColliderGen
	public string meshName = "mesh_2d"; // asset name
	public bool createGameObject = true; // choose yes or no for game object creaiotn into the scene
	public string gameObjectName = "mesh_2d"; // game object name
	public Material materialToUse = null;
	//Name of asset folder to contain quad asset when created
	public string assetFolder = "Assets/Colliders/Generated";
	
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
				assetFolder = Path.GetDirectoryName (assetPath);
		}
	}
	
	//Function to create quad mesh
	void OnWizardCreate ()
	{
		if (meshSource == null) {
			Debug.LogError ("You have to select a mesh");
			return;
		}
		
		/// 2d ColliderGen v1.4 takes a "z-thickness" property for the mesh generation and locates
		/// one plane at z-thicknes/2 and the other at -z-thickness/2.
		/// Also the amount of vertices we set in the "Outline Vertex Count" property is quadruplied.
		/// Eg: having an outline of 24 verts will generate a mesh collider of 92 verts.
		/// So we need to extract only frontal vertices (z = z-thickness/2) and avoid duplicates
		
		Vector3[] colliderVerts = meshSource.vertices;
		HashSet<Vector3> vertsSet = new HashSet<Vector3> (new Vector3Comparer ());
		for (int i=0,c=colliderVerts.Length; i<c; ++i)
			vertsSet.Add (colliderVerts [i]);
		
		// the set should contain colliderVerts/2 verts according to my analysis
		if (vertsSet.Count != colliderVerts.Length / 2) {
			Debug.LogWarning("Set of vertices should contain colliderVerts/2 verts. The set has " + 
				vertsSet.Count + " and colliderVerts/2 = " + colliderVerts.Length / 2);
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
		mesh.name = meshName;
		
		// it seems that 2D ColliderGen creates meshes normalized inside unitary XY plane center in 0.0, so the UVs are offset by 0.5
		Vector2[] uvs = mesh.uv;
		for (int i=0,c=uvs.Length; i < c; ++i) {
			uvs[i].x += 0.5f;
			uvs[i].y += 0.5f;
		}
		mesh.uv = uvs;
		
		//Create or Replace asset in database
		CreateOrReplaceAsset(mesh);
		
		//Create game object to locate into the scene
		if (createGameObject) {
			GameObject _2d_mesh = new GameObject (gameObjectName);
			MeshFilter meshFilter = (MeshFilter)_2d_mesh.AddComponent (typeof(MeshFilter));
			_2d_mesh.AddComponent (typeof(MeshRenderer));
			#if UNITY_3_AND_4
			_2d_mesh.GetComponent<MeshRenderer>().castShadows = false;
			#else
			_2d_mesh.GetComponent<MeshRenderer>().shadowCastingMode = Rendering.ShadowCastingMode.Off;
			#endif
			_2d_mesh.GetComponent<MeshRenderer>().receiveShadows = false;
			_2d_mesh.GetComponent<MeshRenderer>().sharedMaterial = materialToUse;
			meshFilter.sharedMesh = mesh;
		}
	}
	
	private void CreateOrReplaceAsset (Mesh mesh) {
		string path = AssetDatabase.GenerateUniqueAssetPath (assetFolder + "/" + meshName) + ".asset";
		Mesh outputMesh = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
		
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