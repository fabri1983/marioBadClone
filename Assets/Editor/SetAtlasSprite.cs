using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Usage:
/// 
/// </summary>
public class SetAtlasSprite : EditorWindow
{
	//Reference to atlas data game object
	public GameObject atlasDataObject = null;
	
	//Reference to atlas data
	public AtlasData atlasDataComponent = null;
	
	//Popup Index
	public int PopupIndex = 0;
	
	private int AtlasW, AtlasH;
	
	
	//------------------------------------------------
	[MenuItem ("Window/Atlas Texture Editor")]
	static void Init () 
	{
		GetWindow (typeof(SetAtlasSprite),false,"Texture Atlas", true);
	}

	//------------------------------------------------
	void OnEnable ()
	{
	}
	
	//------------------------------------------------
	void OnGUI () 
	{
		//Draw Atlas Object Selector
		GUILayout.Label ("Atlas Generation", EditorStyles.boldLabel);
		atlasDataObject = (GameObject) EditorGUILayout.ObjectField("Atlas Object", atlasDataObject, typeof (GameObject), true);
		
		if(atlasDataObject == null)
			return;
		
		atlasDataComponent = atlasDataObject.GetComponent<AtlasData>();
		
		// if no has a valid AtlasData component then exit drawing GUI, until the game object be the correct one
		if(!atlasDataComponent)
			return;
		
		AtlasW = atlasDataComponent.AtlasTexture.width;
		AtlasH = atlasDataComponent.AtlasTexture.height;
		
		PopupIndex = EditorGUILayout.Popup(PopupIndex, atlasDataComponent.TextureNames);
		
		if(GUILayout.Button("Select Sprite From Atlas"))
		{
			// Selection works when at least one gameObject is selected in the hierarchy
			if(Selection.gameObjects.Length > 0)
			{
				foreach(GameObject Obj in Selection.gameObjects)
				{
					// if sprite frame size is (0,0) means it is not a game object with sprite animation. Update the mesh uvs
					if (Vector2.zero.Equals(atlasDataComponent.frameSizePixels[PopupIndex])) {
						if(Obj.GetComponent<MeshFilter>())
							UpdateMeshUVs(Obj, atlasDataComponent.UVs[PopupIndex]);
					}
					// its a game object with a sprite anim. Update the animation objects accordingly
					else {
						// get AnimateTiledTexture component to update some properties for Atlas configuration
						AnimateTiledTexture anim = Obj.GetComponentInChildren<AnimateTiledTexture>();
						// get all AnimateTiledConfig components to update their properties for Atlas configuration
						AnimateTiledConfig[] configs = Obj.GetComponentsInChildren<AnimateTiledConfig>();
						
						// for the AnimateTiledTexture component set the total number of rows in the atlas according to the sprite frame's height
						//anim._rowsTotalInSprite = ;
						
						// for all the AnimateTiledConfig components set the properties accordingly
						
					}
				}
			}
		}
	}
	
	//------------------------------------------------
	void OnInspectorUpdate()
	{
		Repaint();
	}
	
	/// <summary>
	/// Function to update mesh UVs of selected mesh object
	/// </summary>
	/// <param name='MeshOject'>
	/// Mesh oject.
	/// </param>
	/// <param name='AtlasUVs'>
	/// Atlas U vs.
	/// </param>
	/// <param name='Reset'>
	/// Reset.
	/// </param>
	void UpdateMeshUVs(GameObject MeshOject, Rect AtlasUVs, bool Reset = false)
	{
		//Get Mesh Filter Component
		MeshFilter MFilter = MeshOject.GetComponent<MeshFilter>();
		Mesh MeshObject = MFilter.sharedMesh;
		
		//Vertices
		Vector3[] Vertices = MeshObject.vertices;
		Vector2[] UVs = new Vector2[Vertices.Length];
		
		//Bottom-left
		UVs[0].x=(Reset) ? 0.0f : AtlasUVs.x;
		UVs[0].y=(Reset) ? 0.0f : AtlasUVs.y;
		
		//Bottom-right
		UVs[1].x=(Reset) ? 1.0f : AtlasUVs.x+AtlasUVs.width;
		UVs[1].y=(Reset) ? 0.0f : AtlasUVs.y;
		
		//Top-left
		UVs[2].x=(Reset) ? 0.0f : AtlasUVs.x;
		UVs[2].y=(Reset) ? 1.0f : AtlasUVs.y+AtlasUVs.height;
		
		//Top-right
		UVs[3].x=(Reset) ? 1.0f : AtlasUVs.x+AtlasUVs.width;
		UVs[3].y=(Reset) ? 1.0f : AtlasUVs.y+AtlasUVs.height;
		
		MeshObject.uv = UVs;
		MeshObject.vertices = Vertices;
		
		AssetDatabase.Refresh();
    	AssetDatabase.SaveAssets();
	}

}