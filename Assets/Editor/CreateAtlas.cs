using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

public class CreateAtlas : ScriptableWizard 
{
	public string AtlasName = "AtlasTexture";
	public string path = "Textures/Atlases/";
	public int Padding = 0;
	public int maxAtlasSize = 2048;
	public int maxTextureSize = 512;
	public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
	public Texture2D[] Textures;
	
	private bool mipmap = false;
	private TextureImporterSettings[] settingBackup;
	private Regex pattern = new Regex("(\\w+)_([0-9]+)x([0-9]+)\\.([A-Za-z0-9]+)");
	
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
	private void ConfigureForAtlas(string TexturePath, int size)
	{
		TextureImporter TexImport = AssetImporter.GetAtPath(TexturePath) as TextureImporter;
		TextureImporterSettings tiSettings = new TextureImporterSettings();
			
		TexImport.textureType = TextureImporterType.Advanced;
			
		TexImport.ReadTextureSettings(tiSettings);
			
		tiSettings.mipmapEnabled = mipmap;
		tiSettings.readable = true; // set it true to avoid error "UnityEngine.Texture2D:PackTextures. Texture atlas needs textures to have Readable flag set!"
		tiSettings.maxTextureSize = size;
		tiSettings.textureFormat = TextureImporterFormat.AutomaticCompressed;
		tiSettings.filterMode = FilterMode.Bilinear;
		tiSettings.aniso = 0;
		tiSettings.wrapMode = wrapMode;
		tiSettings.npotScale = TextureImporterNPOTScale.ToNearest;

		TexImport.SetTextureSettings(tiSettings);
			
		//Re-import/update Texture
		AssetDatabase.ImportAsset(TexturePath, ImportAssetOptions.ForceUpdate);
		AssetDatabase.Refresh();
	}
	
	private void setTextureSettings (string TexturePath, TextureImporterSettings tiSettings) {
		TextureImporter TexImport = AssetImporter.GetAtPath(TexturePath) as TextureImporter;
			
		TexImport.textureType = TextureImporterType.Advanced;

		TexImport.SetTextureSettings(tiSettings);
			
		//Re-import/update Texture
		AssetDatabase.ImportAsset(TexturePath, ImportAssetOptions.ForceUpdate);
		AssetDatabase.Refresh();
	}
	
	private TextureImporterSettings getTextureSettings (string texPath) {
		TextureImporter TexImport = AssetImporter.GetAtPath(texPath) as TextureImporter;
		TextureImporterSettings tiSettings = new TextureImporterSettings();
			
		TexImport.textureType = TextureImporterType.Advanced;
			
		TexImport.ReadTextureSettings(tiSettings);
		
		return tiSettings;
	}
	
	void GenerateAtlas()
	{
		GameObject AtlasObject = new GameObject("AtlasData");
		AtlasData AD = AtlasObject.AddComponent<AtlasData>();
		
		// backup texture settings per texture
		settingBackup = new TextureImporterSettings[Textures.Length];
		for(int i=0; i<settingBackup.Length; i++)
			settingBackup[i] = getTextureSettings(AssetDatabase.GetAssetPath(Textures[i]));
		
		//Generate texture names and sprite frame sizes (if any)
		AD.TextureNames = new string[Textures.Length];
		AD.frameSizePixels = new Vector2[Textures.Length];
		
		for(int i=0; i<Textures.Length; i++)
		{
			string texPath = AssetDatabase.GetAssetPath(Textures[i]);
			ConfigureForAtlas(texPath, maxTextureSize);
			AD.TextureNames[i] = texPath;
			// set frame size in pixels. (0,0) means is not a sprite
			AD.frameSizePixels[i] = getFrameSizeFromTextureName(texPath);
		}
		
		//Generate Atlas texture
		Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, mipmap); // png encode accepts ARGB32 or RGB24
		AD.UVs = tex.PackTextures(Textures, Padding, maxAtlasSize);
		
		//Generate Unique Asset Path
		string AssetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/" + path + "/" + AtlasName + ".png");
		
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
		ConfigureForAtlas(AssetDatabase.GetAssetPath(tex), maxAtlasSize);
		
		AD.AtlasTexture = tex;
		
		// restore read only property per texture
		for(int i=0; i<settingBackup.Length; i++) {
			string texPath = AssetDatabase.GetAssetPath(Textures[i]);
			setTextureSettings(texPath, settingBackup[i]);
		}
	}
	
	/// <summary>
	/// Extracts from the texture name the size of a sprite frame.
	/// Eg:
	///     mario_sprite_2x10.png
	///   means it has 2 rows and 10 columns
	///   in atlas texture size is 512*128 (dependen on maxTextureSize)
	///   return Vector2(51.2, 64.0);
	/// </summary>
	/// <returns>
	/// Size of the sprite frame
	/// </returns>
	/// <param name='texPath'>
	/// Texture path with file name
	/// </param>
	private Vector2 getFrameSizeFromTextureName (string texPath) {
		Texture2D tex2d = AssetDatabase.LoadAssetAtPath(texPath, typeof(Texture2D)) as Texture2D;

		Match match = pattern.Match(texPath);
		
		if (!match.Success)
			return Vector2.zero;
		
		string rows = match.Groups[2].Value;
		string cols = match.Groups[3].Value;
		string pathAndName = match.Groups[1].Value;
		string ext = match.Groups[4].Value;
		
		int _rows, _cols;
		int.TryParse(rows, out _rows);
		int.TryParse(cols, out _cols);
		float frameW = (float)tex2d.width / (float)_cols;
		float frameH = (float)tex2d.height / (float)_rows;
		
		Debug.Log(pathAndName + " - " + rows + " - " + cols + " - " + ext 
			+ ". In Atlas: " + tex2d.width + "x" + tex2d.height + " px"
			+ ". Frame size in Atlas: " + frameW + "x" + frameH);
		
		return new Vector2(frameW, frameH);
	}
}
