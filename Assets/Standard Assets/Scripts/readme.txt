Locate scripts in this folder when you need they to be compiled in the assembly-first-pass stage in order to be accesed from other script locations.

Explanation:
Unity have to do this so other languages can use an assembly from another language. 
However to use an assembly it has to be compiled first. 
That's why we have a first pass and a second pass assembly. 
The first-pass assemblies can't use each other. 
They are compiled at the same time so they can't reference each other.

The second-pass assemblies are compiled next so they can use all first-pass assemblies, but not other second-pass assemblies. 
That's why you have to put scripts that should be accessible from another language in a first-pass folder (Standard Assets, plugins).

Editor assemblies, which are only build and used in the editor, are compiled last so they can use all previous assemblies.

Final note: You can put your classes into namespaces without any problems. 
However Unity "extracts" classes that are derived from MonoBehaviour into the global namespace. 
That's why namespaces for MonoBehaviours become irrelevant. 
You can't have two MonoBehaviours (or Editor, EditorWindows, ...) with the same name, even when they are in different namespaces.