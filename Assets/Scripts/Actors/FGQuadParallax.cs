using UnityEngine;

/// <summary>
/// This class is a sub class of Quad Parallax with the only purpose of be usefull when 
/// using GetComponent<QuadParallax>() we can discern at component level in case a game object 
/// uses both components BGQuadParallax and FGQuadParallax.
/// </summary>
[ExecuteInEditMode]
public class FGQuadParallax : QuadParallax {

}
