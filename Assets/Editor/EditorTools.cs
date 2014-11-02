using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

static public class EditorTools
{
	private delegate void GetWidthAndHeight(TextureImporter importer, ref float width, ref float height);
    private static GetWidthAndHeight getWidthAndHeightDelegate;
    
	/// <summary>
	/// Gets the original texture's size as Unit imports it.
	/// </summary>
	/// <returns>
	/// The original texture size.
	/// </returns>
	/// <param name='texture'>
	/// Texture2D object
	/// </param>
    public static Vector2 GetOriginalTextureSize(Texture2D texture)
    {
        if (texture == null) {
            Debug.LogError("Texture2D parameter is null");
			return Vector2.zero;
		}
     
        string path = AssetDatabase.GetAssetPath(texture);
        if (string.IsNullOrEmpty(path)) {
			Debug.LogError("Texture2D is not an asset texture.");
			return Vector2.zero;
		}
     
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) {
			Debug.LogError("Failed to get Texture importer for " + path);
			return Vector2.zero;
		}
     
        return GetOriginalTextureSize(importer);
    }
    
	/// <summary>
	/// Gets the original texture's size as Unit imports it.
	/// </summary>
	/// <returns>
	/// The original texture size.
	/// </returns>
	/// <param name='importer'>
	/// Importer object
	/// </param>
    public static Vector2 GetOriginalTextureSize(TextureImporter importer)
    {
        if (getWidthAndHeightDelegate == null) {
            MethodInfo method = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            getWidthAndHeightDelegate = Delegate.CreateDelegate(typeof(GetWidthAndHeight), null, method) as GetWidthAndHeight;
        }
     
        Vector2 vec = new Vector2();
        getWidthAndHeightDelegate(importer, ref vec.x, ref vec.y);
     
        return vec;
    }
}
