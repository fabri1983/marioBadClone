using UnityEngine;

/// <summary>
/// This class is a sub class of Parallax with the only purpose of be usefull when 
/// using GetComponent<Parallax>() we can discern at component level in case a game object 
/// uses both components BGParallax and FGParallax.
/// </summary>
[ExecuteInEditMode]
public class FGParallax : Parallax {

}
