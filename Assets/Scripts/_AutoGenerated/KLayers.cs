// This class is auto-generated by ConstantClassesGenerator. Do not modify
public static class KLayers
{
	public const int DEFAULT = 0;
	public const int TRANSPARENT_FX = 1;
	public const int IGNORE_RAYCAST = 2;
	public const int WATER = 4;
	public const int ENEMY = 8;
	public const int ONLY_WITH_PLAYER = 9;
	public const int PLAYER = 10;
	public const int POWER_UP = 11;
	public const int TELEPORTING = 12;
	public const int FOR_ENEMY = 13;


	public static int onlyIncluding( params int[] layers )
	{
		int mask = 0;
		for( var i = 0; i < layers.Length; i++ )
			mask |= ( 1 << layers[i] );

		return mask;
	}


	public static int everythingBut( params int[] layers )
	{
		return ~onlyIncluding( layers );
	}
}