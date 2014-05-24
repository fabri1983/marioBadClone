using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class CreateAtlas : ScriptableWizard 
{
	public string AtlasName = "AtlasTexture";
	
	public int Padding = 4;
	
	public Texture2D[] Textures;
	
	[MenuItem("GameObject/Create Other/Create Atlas")]
	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard("Create Atlas",typeof(CreateAtlas));
	}
	
	void OnWizardCreate()
	{
		GenerateAtlas();
	}
	
	//Fucnction to configure texture for atlasing
	public void ConfigureForAtlas(string TexturePath)
	{
		TextureImporter TexImport = AssetImporter.GetAtPath(TexturePath) as TextureImporter;
		TextureImporterSettings tiSettings = new TextureImporterSettings();
			
		TexImport.textureType = TextureImporterType.Advanced;
			
		TexImport.ReadTextureSettings(tiSettings);
			
		tiSettings.mipmapEnabled = false;
		tiSettings.readable = true;
		tiSettings.maxTextureSize = 1024;
		tiSettings.textureFormat = TextureImporterFormat.RGBA32;
		tiSettings.filterMode = FilterMode.Bilinear;
		tiSettings.wrapMode = TextureWrapMode.Clamp;
		tiSettings.npotScale = TextureImporterNPOTScale.None;

		TexImport.SetTextureSettings(tiSettings);
			
		//Re-import/update Texture
		AssetDatabase.ImportAsset(TexturePath, ImportAssetOptions.ForceUpdate);
		AssetDatabase.Refresh();
	}
	
	void GenerateAtlas()
	{
		GameObject AtlasObject = new GameObject("AtlasData");
		AtlasData AD = AtlasObject.AddComponent<AtlasData>();
		
		//Generate texture names
		AD.TextureNames = new string[Textures.Length];

		for(int i=0; i<Textures.Length; i++)
		{
			string Path = AssetDatabase.GetAssetPath(Textures[i]);
			ConfigureForAtlas(Path);
			AD.TextureNames[i]=Path;
		}
		
		//Generate Atlas
		Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
		AD.UVs = tex.PackTextures(Textures, Padding, 2048);
		
		//Generate Unique Asset Path
		string AssetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/Textures" + AtlasName + ".png");
		
		//Write texture to file
		byte[] bytes = tex.EncodeToPNG();
		System.IO.File.WriteAllBytes(AssetPath, bytes);
		bytes = null;
		
		//Delete generated texture
		UnityEngine.Object.DestroyImmediate(tex);
		
		//Import Asset
		AssetDatabase.ImportAsset(AssetPath);
		
		//Get Imported Texture
		tex = AssetDatabase.LoadAssetAtPath(AssetPath, typeof(Texture2D)) as Texture2D;
		
		//Configure texture as atlas
		ConfigureForAtlas(AssetDatabase.GetAssetPath(tex));
		
		AD.AtlasTexture = tex;
	}
}
