#if !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
#define UNITY_4_AND_LATER
#endif
#if !(UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
#define UNITY_4_3_AND_LATER
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR

//-------------------------------------------------------------------------
/// <summary>
/// A data storage class to hold the intermediate results of the collider-generation algorithm
/// and the resulting colliders of all AlphaMeshCollider instances.
/// 
/// Implemented as a singleton.
/// </summary>
[ExecuteInEditMode]
public class AlphaMeshColliderRegistry : MonoBehaviour {

	//-------------------------------------------------------------------------
	/// <summary>
	/// Nested class to hold informations of one group of equal
	/// AlphaMeshColliders that all share the same targetPath,
	/// intermediate results, etc.
	/// </summary>
	[System.Serializable]
	public class ColliderGroup {
		public string mFullColliderMeshPath;
		public PolygonOutlineFromImageFrontend mOutlineAlgorithm = null;
		public AlphaMeshCollider.RegionIndependentParameters mRegionIndependentParameters = null;
		public AlphaMeshCollider.ColliderRegionParameters[] mColliderRegionParameters = null;
		public ColliderRegionData[] mColliderRegions = null;
		public Mesh mColliderMesh = null;
		public float mAlphaOpaqueThreshold = 0.1f;
		public bool mConvex = false;
		public float mCustomRotation = 0.0f;
		public Vector2 mCustomScale = Vector2.one;
		public Vector3 mCustomOffset = Vector3.zero;
		public List<WeakReference> mAlphaMeshColliderObjects = null;
	}
	//-------------------------------------------------------------------------
	
	
	/// <summary> The singleton instance. </summary>
    private static AlphaMeshColliderRegistry mInstance = null;
	public List<ColliderGroup> mColliderGroups = new List<ColliderGroup>();
	
    
	//-------------------------------------------------------------------------
	public static AlphaMeshColliderRegistry Instance { 
        get {
            if (mInstance == null) {
			
				AlphaMeshColliderRegistry existingSingleton = (AlphaMeshColliderRegistry) GameObject.FindObjectOfType(typeof(AlphaMeshColliderRegistry));
				if (existingSingleton != null) {
					mInstance = existingSingleton;
				}
				else {
		            GameObject gameObject = new GameObject();
		            mInstance = gameObject.AddComponent<AlphaMeshColliderRegistry>();
		            gameObject.name = "AlphaMeshColliderRegistry";
				}
            }
            return mInstance; 
        }
    }
	
	//-------------------------------------------------------------------------
	public void ReInitColliderGroups() {

		mColliderGroups.Clear();
		
#if UNITY_4_AND_LATER
		object[] alphaMeshColliders = GameObject.FindObjectsOfType(typeof(AlphaMeshCollider));
#else
		object[] alphaMeshColliders = GameObject.FindSceneObjectsOfType(typeof(AlphaMeshCollider));
#endif
		Array.Sort(alphaMeshColliders, delegate(object first, object second) {
                    							return ((AlphaMeshCollider)first).mColliderMeshFilename.CompareTo(
													   ((AlphaMeshCollider)second).mColliderMeshFilename);
                  						});
		
		foreach (AlphaMeshCollider collider in alphaMeshColliders) {
			
			string colliderMeshPath = collider.FullColliderMeshPath();
			ColliderGroup colliderGroup = FindColliderGroup(colliderMeshPath);
			if (colliderGroup == null) {
				// first AlphaMeshCollider with path colliderMeshPath.
				colliderGroup = new ColliderGroup();
				colliderGroup.mFullColliderMeshPath = colliderMeshPath;
				mColliderGroups.Add(colliderGroup);
			
				AssignValuesFromInstanceToGroup(collider, colliderGroup);
				
				colliderGroup.mAlphaMeshColliderObjects = new List<WeakReference>();				
				colliderGroup.mAlphaMeshColliderObjects.Add(new WeakReference(collider));
			}
			else {
				// not the first one - add it to the list of weak references.
				colliderGroup.mAlphaMeshColliderObjects.Add(new WeakReference(collider));
			}
		}
	}
	
	//-------------------------------------------------------------------------
	void Update() {
        if (!Application.isEditor || Application.isPlaying)
            return;
		
		ReInitColliderGroups();
	}
	
	//-------------------------------------------------------------------------
	public void RecalculateColliderAndUpdateSimilar(AlphaMeshCollider target) {
		if (!target.CanRecalculateCollider)
			return;
		
		target.RecalculateCollider();
		
		UpdateSimilarCollidersAndGroupToTarget(target);
	}
	
	//-------------------------------------------------------------------------
	public void RecalculateColliderFromPreviousResultAndUpdateSimilar(AlphaMeshCollider target) {
		if (!target.CanRecalculateCollider)
			return;
		
		target.RecalculateColliderFromPreviousResult();
		
		UpdateSimilarCollidersAndGroupToTarget(target);
	}
	
	//-------------------------------------------------------------------------
	public void RewriteColliderToFileAndUpdateSimilar(AlphaMeshCollider target) {
		if (target.CanRewriteCollider) {
			target.RewriteColliderToFile();
		}
		
		UpdateSimilarCollidersAndGroupToTarget(target);
	}
	
	//-------------------------------------------------------------------------
	public void ReloadOrRecalculateColliderAndUpdateSimilar(AlphaMeshCollider target) {
		if (!target.CanRecalculateCollider)
			return;
		
		string colliderMeshPath = target.FullColliderMeshPath();
		
		ColliderGroup colliderGroup = FindColliderGroup(colliderMeshPath);
		if (colliderGroup == null || !IsColliderGroupValid(colliderGroup)) {
			target.RecalculateCollider();
			UpdateSimilarCollidersAndGroupToTarget(target);
		}
		else {
			UpdateSimilarCollidersToGroup(colliderGroup);
		}
	}
	
	//-------------------------------------------------------------------------
	public void ReloadOrRecalculateSingleCollider(AlphaMeshCollider target) {
		if (!target.CanReloadCollider)
			return;
		
		string colliderMeshPath = target.FullColliderMeshPath();
		ColliderGroup colliderGroup = FindColliderGroup(colliderMeshPath);
		if (colliderGroup == null || !IsColliderGroupValid(colliderGroup)) {
			if (!target.CanRecalculateCollider)
				return;
			
			target.RecalculateCollider();
			UpdateSimilarCollidersAndGroupToTarget(target);
		}
		else {
			AssignValuesFromColliderGroup(target, colliderGroup);
		}
	}
	
	//-------------------------------------------------------------------------
	protected void UpdateSimilarCollidersToGroup(ColliderGroup colliderGroup) {
		
		string colliderMeshPath = colliderGroup.mFullColliderMeshPath;
#if UNITY_4_AND_LATER
		object[] alphaMeshColliders = GameObject.FindObjectsOfType(typeof(AlphaMeshCollider));
#else
		object[] alphaMeshColliders = GameObject.FindSceneObjectsOfType(typeof(AlphaMeshCollider));
#endif
		
		colliderGroup.mAlphaMeshColliderObjects = new List<WeakReference>();
		foreach (AlphaMeshCollider collider in alphaMeshColliders)
		{
			if (collider.FullColliderMeshPath().Equals(colliderMeshPath)) {
				
				colliderGroup.mAlphaMeshColliderObjects.Add(new WeakReference(collider));
				
				// reassign previously calculated values.
				AssignValuesFromColliderGroup(collider, colliderGroup);
			}
		}
	}
	
	//-------------------------------------------------------------------------
	protected void UpdateSimilarCollidersAndGroupToTarget(AlphaMeshCollider target) {
		
		string colliderMeshPath = target.FullColliderMeshPath();
#if UNITY_4_AND_LATER
		object[] alphaMeshColliders = GameObject.FindObjectsOfType(typeof(AlphaMeshCollider));
#else
		object[] alphaMeshColliders = GameObject.FindSceneObjectsOfType(typeof(AlphaMeshCollider));
#endif

		ColliderGroup colliderGroup = FindColliderGroup(colliderMeshPath);
		if (colliderGroup == null) {
			// add new group.
			colliderGroup = new ColliderGroup();
			colliderGroup.mFullColliderMeshPath = colliderMeshPath;
			mColliderGroups.Add(colliderGroup);
		}
		
		AssignValuesFromInstanceToGroup(target, colliderGroup);
		
		colliderGroup.mAlphaMeshColliderObjects = new List<WeakReference>();
		foreach (AlphaMeshCollider collider in alphaMeshColliders)
		{
			if (collider.FullColliderMeshPath().Equals(colliderMeshPath)) {
				
				colliderGroup.mAlphaMeshColliderObjects.Add(new WeakReference(collider));
				
				// reassign previously calculated values.
				AssignValuesFromColliderGroup(collider, colliderGroup);
			}
		}
	}
	
	//-------------------------------------------------------------------------
	protected void AssignValuesFromInstanceToGroup(AlphaMeshCollider target, ColliderGroup colliderGroup) {

		colliderGroup.mRegionIndependentParameters = target.RegionIndependentParams;
		colliderGroup.mColliderRegionParameters = target.ColliderRegionParams;
		colliderGroup.mColliderRegions = target.ColliderRegions;
		colliderGroup.mOutlineAlgorithm = target.OutlineAlgorithm;
		colliderGroup.mColliderMesh = target.ColliderMesh;
	}
	
	//-------------------------------------------------------------------------
	public bool AssignValuesFromColliderGroup(AlphaMeshCollider target, string fullMeshPath) {
		
		ColliderGroup colliderGroup = this.FindColliderGroup(fullMeshPath);
		if (colliderGroup == null) {
			return false;
		}
		else {
			AssignValuesFromColliderGroup(target, colliderGroup);
			return true;
		}
	}
	
	//-------------------------------------------------------------------------
	public void AssignValuesFromColliderGroup(AlphaMeshCollider target, ColliderGroup colliderGroup) {
		
		target.RegionIndependentParams = colliderGroup.mRegionIndependentParameters;
		target.ColliderRegionParams = colliderGroup.mColliderRegionParameters;
		target.ColliderRegions = colliderGroup.mColliderRegions;
		target.OutlineAlgorithm = colliderGroup.mOutlineAlgorithm;

		target.CorrectColliderTypeToParameters();
		target.ColliderMesh = colliderGroup.mColliderMesh; // sets the sharedMesh to null first, so no need to set it here.
#if UNITY_4_3_AND_LATER
		target.ReassignPolygonCollider2DDataIfNeeded();
#endif
	}
	
	//-------------------------------------------------------------------------
	/// <summary>
	/// Returns a collider group within mColliderGroups if one with
	/// mFullColliderMeshPath equal to <c>fullMeshPath</c> is found,
	/// null otherwise.
	/// </summary>
	protected ColliderGroup FindColliderGroup(string fullMeshPath) {
		foreach (ColliderGroup colliderGroup in mColliderGroups) {
			if (colliderGroup.mFullColliderMeshPath.Equals(fullMeshPath))
				return colliderGroup;
		}
		return null;
	}
	
	//-------------------------------------------------------------------------
	protected bool IsColliderGroupValid(ColliderGroup colliderGroup) {
		if (colliderGroup.mOutlineAlgorithm == null ||
			colliderGroup.mColliderRegionParameters == null ||
			colliderGroup.mColliderRegions == null ||
			colliderGroup.mColliderRegionParameters.Length != colliderGroup.mColliderRegions.Length ||
			colliderGroup.mColliderMesh == null) {
			
			return false;
		}
		else {
			return true;
		}
	}
	
	//-------------------------------------------------------------------------
	protected void EnsureColliderGroupIsValid(ColliderGroup colliderGroup) {
		if (IsColliderGroupValid(colliderGroup))
			return;
		
		string colliderMeshPath = colliderGroup.mFullColliderMeshPath;
#if UNITY_4_AND_LATER
		object[] alphaMeshColliders = GameObject.FindObjectsOfType(typeof(AlphaMeshCollider));
#else
		object[] alphaMeshColliders = GameObject.FindSceneObjectsOfType(typeof(AlphaMeshCollider));
#endif
		
		colliderGroup.mAlphaMeshColliderObjects = new List<WeakReference>();
		AlphaMeshCollider target = null;
		foreach (AlphaMeshCollider collider in alphaMeshColliders)
		{
			if (collider.FullColliderMeshPath().Equals(colliderMeshPath)) {
				
				target = collider;
				colliderGroup.mAlphaMeshColliderObjects.Add(new WeakReference(collider));
			}
		}
		
		if (target != null) {
			AssignValuesFromInstanceToGroup(target, colliderGroup);
		}
	}
	
	//-------------------------------------------------------------------------
	protected void CheckForOutdatedColliderMeshPaths() {
		
		List<ColliderGroup> groupsToRemove = new List<ColliderGroup>();
		
		foreach (ColliderGroup colliderGroup in mColliderGroups) {
			
			if (colliderGroup.mAlphaMeshColliderObjects != null) {
			
				colliderGroup.mAlphaMeshColliderObjects.RemoveAll(
					delegate(WeakReference colliderInstanceRef) {
						bool isOutdated = !((AlphaMeshCollider) colliderInstanceRef.Target).FullColliderMeshPath().Equals(colliderGroup.mFullColliderMeshPath);
						return isOutdated;
				});
				if (colliderGroup.mAlphaMeshColliderObjects.Count == 0) {
					groupsToRemove.Add(colliderGroup);
				}
			}
		}
		
		for (int removeIndex = groupsToRemove.Count - 1; removeIndex >= 0; --removeIndex) {
			mColliderGroups.Remove(groupsToRemove[removeIndex]);
		}
	}
	
	//-------------------------------------------------------------------------
	protected void RemoveEmptyColliderGroups() {
		
		mColliderGroups.RemoveAll(
			delegate(ColliderGroup colliderGroup) {
				bool hasNoInstances = (colliderGroup.mAlphaMeshColliderObjects != null &&
									   colliderGroup.mAlphaMeshColliderObjects.Count == 0);
				if (hasNoInstances)
					Debug.Log("removing group: " + colliderGroup.mFullColliderMeshPath);
				return hasNoInstances;
			});
	}
	
}

#endif // #if UNITY_EDITOR

