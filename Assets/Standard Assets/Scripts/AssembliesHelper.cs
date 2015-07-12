
public static class AssembliesHelper {

	private static System.Reflection.Assembly[] AS = null;
	
	public static System.Type GetType (string TypeName)
	{
		// Try Type.GetType() first. This will work with types defined by the Mono runtime, etc.
		System.Type type = System.Type.GetType( TypeName );
		
		if ( type != null )
			return type;
		
		// Get the name of the assembly (Assumption is that we are using fully-qualified type names)
		string assemblyName = TypeName.IndexOf( '.' ) >= 0 ? TypeName.Substring( 0, TypeName.IndexOf( '.' ) ) : TypeName;
		
		// Option 1: get all assemblies and search for TypeName
		// the next array brings all the assemblies, so should be cached
		if ( AS == null )
			AS = System.AppDomain.CurrentDomain.GetAssemblies();
		foreach (System.Reflection.Assembly A in AS)
		{
			type = A.GetType(assemblyName);
			if (type != null)
				return type;
		}
		
		return null;
	}
}
