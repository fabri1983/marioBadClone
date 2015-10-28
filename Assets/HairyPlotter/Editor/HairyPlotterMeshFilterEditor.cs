/**
 * Copyright (C) 2012 Fredrik Holmstrom
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do 
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 **/

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MeshFilter))]
public class HairyPlotterMeshFilterEditor : Editor
{
    string saveName = "";
    bool saveAsset = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MeshFilter filter = (MeshFilter)target;
        HairyPlotter plotter = filter.GetComponent<HairyPlotter>();

        if (plotter)
        {
            if (saveAsset)
            {
                EditorGUILayout.BeginHorizontal();

                saveName = EditorGUILayout.TextField("Save Mesh As", saveName);

                if (GUILayout.Button("Save", EditorStyles.miniButton))
                {
                    if (saveName != "")
                    {
                        // Make sure mesh is latest version
                        plotter.UpdateMesh();

                        // Create asset mesh
                        Mesh asset = HairyPlotter.CloneMesh(plotter.EditMesh);

                        // Create asset and destroy temp mesh
                        AssetDatabase.CreateAsset(asset, "Assets/" + saveName + ".asset");

                        // Store asset on filter
                        filter.sharedMesh = asset;

                        // Set dirty
                        EditorUtility.SetDirty(filter);

                        // Destroy plotter object
                        MonoBehaviour.DestroyImmediate(plotter);

                        // Clean up assets
                        CleanUp(plotter);
                    }
                    else
                    {
                        Debug.LogWarning("Enter a name for the mesh asset");
                    }
                }

                if (GUILayout.Button("Cancel", EditorStyles.miniButton))
                {
                    saveAsset = false;
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (GUILayout.Button("Save Mesh And Stop Editing", EditorStyles.miniButton))
                {
                    saveName = "";
                    saveAsset = true;
                }

                if (GUILayout.Button("Revert Mesh", EditorStyles.miniButton))
                {
                    if (plotter.OriginalMesh != null)
                    {
                        // Load main asset
                        filter.sharedMesh = (Mesh)AssetDatabase.LoadMainAssetAtPath(plotter.OriginalMesh);
                    }
                    else
                    {
                        // Reset to empty mesh
                        filter.sharedMesh = new Mesh();


                        Debug.LogWarning("No original mesh to revert to");
                    }

                    // Clear edit asset
                    plotter.EditMesh = null;
                    plotter.OriginalMesh = null;

                    // Mark dirty
                    EditorUtility.SetDirty(filter);
                }

                if (GUILayout.Button("Abort Editing", EditorStyles.miniButton))
                {
                    if (plotter.OriginalMesh != null)
                    {
                        filter.sharedMesh = (Mesh)AssetDatabase.LoadAssetAtPath(plotter.OriginalMesh, typeof(Mesh));
                    }
                    else
                    {
                        // No mesh to revert to
                        filter.sharedMesh = null;
                    }

                    // Set dirty
                    EditorUtility.SetDirty(filter);

                    // Destroy plotter object
                    MonoBehaviour.DestroyImmediate(plotter);

                    // Clean upp assets
                    CleanUp(plotter);
                }
				
				// select vertex gizmo size
				HairyPlotterEditor.GizmoSize = EditorGUILayout.FloatField("Gizmo Vertex Zize", HairyPlotterEditor.GizmoSize);
				// keep UV mapping while moving verts?
				HairyPlotterEditor.KeepUVOnVertsMove = EditorGUILayout.Toggle("Keep vertices UVs", HairyPlotterEditor.KeepUVOnVertsMove);
            }
        }
        else
        {
            if (AssetDatabase.LoadAssetAtPath("Assets/HairyPlotterMesh.asset", typeof(Mesh)))
            {
                EditorGUILayout.LabelField("Another mesh is already being edited with HairyPlotter", EditorStyles.miniLabel);
            }
            else
            {
                if (GUILayout.Button("Edit Mesh With HairyPlotter", EditorStyles.miniButton))
                {
                    EditorUtility.SetDirty(filter);
                    EditorUtility.SetDirty(filter.gameObject.AddComponent<HairyPlotter>());
                }
            }
        }
    }

    void CleanUp(HairyPlotter plotter)
    {
        // Destroy temp mesh
        AssetDatabase.DeleteAsset("Assets/HairyPlotterMesh.asset");
        AssetDatabase.SaveAssets();

        // Un-register all delegates
        SceneView.onSceneGUIDelegate = null;
    }
}