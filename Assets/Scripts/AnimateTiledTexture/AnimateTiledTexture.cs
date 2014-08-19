using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimateTiledTexture : MonoBehaviour
{
	public bool _useCoroutine = false;					// Use coroutines for PC targets. For mobile targets WaitForSeconds doesn't work.
[HideInInspector] public float _framesPerSecond = 1f;	// Frames per second that you want to texture to play at
[HideInInspector] public bool _pingPongAnim = false;	// True for going forward and backwards in the animation
	public int _rowsTotalInSprite = 1;					// How much rows the sprite has. This value is defined once
	public int _maxColsInRows = 1;		// The greatest number of columns from the rows the anim covers, not the total columns the anim has
[HideInInspector] public bool _playOnce = false;				// Enable this if you want the animation to only play one time
[HideInInspector] public bool _disableUponCompletion = false;	// Enable this if you want the texture to disable the renderer when it is finished playing
[HideInInspector] public bool _enableEvents = false;			// Enable this if you want to register an event that fires when the animation is finished playing
[HideInInspector] public bool _playOnEnable = true;				// The animation will play when the object is enabled
	public bool _newMaterialInstance = false;					// Set this to true when having more than one game object using same sprite
[HideInInspector] public Vector2 _scale = new Vector2(1f, 1f);	// scale the texture. This must be a non-zero number. negative scale flips the image.
[HideInInspector] public Vector2 _offset = Vector2.zero;		// You can use this if you do not want the texture centered. (These are very small numbers .001)
[HideInInspector] public Vector2 _buffer = Vector2.zero;		// You can use this to buffer frames to hide unwanted grid lines or artifacts

	// these two vars will change depending on the sequence to be displayed.
[HideInInspector] public int[] _rowLimits = new int[]{0,1};		// start row and number of rows for current animation (0-based)
[HideInInspector] public int[] _colLimits = new int[]{0,1};		// start column and number of sprite frames for current animation (0-based)

    private int _index = 0;							// Keeps track of the current frame
	private int _direction = 1;						// 1: forward direction. -1: backwards
	private int _maxIndex;							// Max index for current animation
    private Vector2 _textureTiling = Vector2.zero;	// Keeps track of the texture scale 
    private Material _materialInstance = null;		// Material instance of the material we create
    private bool _hasMaterialInstance = false;		// A flag so we know if we have a material instance we need to clean up (better than a null check i think)
    private bool _isPlaying = false;				// A flag to determine if the animation is currently playing
	private float updateTime;						// Use for none coroutine function. Keeps track of time passed during game loops
	private float period;							// The inverse of frames per second. Calculated every time the fps is changed
	private float offsetYStart;						// what is the offset in Y the current animation starts from
	private Vector2 offsetTemp;
	//private Vector4 setupVec1, setupVec2;
	private List<VoidEvent> _voidEventCallbackList;	// A list of functions we need to call if events are enabled
	public delegate void VoidEvent();				// The Event delegate
	
	
	private void Awake()
    {
        // Allocate memory for the events, if needed
        if (_enableEvents)
            _voidEventCallbackList = new List<VoidEvent>();
 
        //Create the material instance. Else, just use this function to recalc the texture size
        ChangeMaterial(renderer.sharedMaterial, _newMaterialInstance);
		
		updateTime = Time.time;
		
		period = 1f / _framesPerSecond;
		
		// it can change if we modify the columns's limits when selecting another animation sequence in the sprite
		_maxIndex = _colLimits[0] + _colLimits[1] - 1;
		
		// what is the offset in Y the current animation starts from
		offsetYStart = 1f - 1f / (_rowsTotalInSprite - _rowLimits[0]);
    }
 
	private void OnEnable()
    {
		CalcTextureTiling();

        if (_playOnEnable)
            Play();
    }
	
    private void OnDestroy() {
        // If we wanted new material instances, we need to destroy the material
        if (_hasMaterialInstance) {
            Object.Destroy(renderer.sharedMaterial);
            _hasMaterialInstance = false;
        }
    }
	
	public void setRowLimits(int start, int numRows) {
		_rowLimits[0] = start;
		_rowLimits[1] = numRows;
		offsetYStart = 1f - 1f / (_rowsTotalInSprite - start);
	}
	
	public void setColLimits(int start, int length) {
		_colLimits[0] = start;
		_colLimits[1] = length;
		_maxIndex = start + length - 1;
		_index = start;
	}
 
	public void setFPS (float fps) {
		_framesPerSecond = fps;
		period = 1f / fps;
	}
	
	public void setPingPongAnim (bool val) {
		_direction = 1; // reset direction
		_pingPongAnim = val;
	}
	
    // Use this function to register your callback function with this script
    public void RegisterCallback(VoidEvent cbFunction) {
        // If events are enabled, add the callback function to the event list
        if (_enableEvents)
            _voidEventCallbackList.Add(cbFunction);
        else
            Debug.LogWarning("AnimateTiledTexture: You are attempting to register a callback but the events of this object are not enabled!");
    }
 
    // Use this function to unregister a callback function with this script
    public void UnRegisterCallback(VoidEvent cbFunction)
    {
        // If events are enabled, unregister the callback function from the event list
        if (_enableEvents)
            _voidEventCallbackList.Remove(cbFunction);
        else
            Debug.LogWarning("AnimateTiledTexture: You are attempting to un-register a callback but the events of this object are not enabled!");
    }
 
	// Handles all event triggers to callback functions
    private void HandleCallbacks(List<VoidEvent> cbList)
    {
        // For now simply loop through them all and call yet.
        for (int i = 0; i < cbList.Count; ++i)
            cbList[i]();
    }

    public void ChangeMaterial(Material newMaterial, bool newInstance = false)
    {
        if (newInstance) {
            // First check our material instance, if we already have a material instance
            // and we want to create a new one, we need to clean up the old one
            if (_hasMaterialInstance)
                Object.Destroy(renderer.sharedMaterial);
 
            // create the new material
            _materialInstance = new Material(newMaterial);
 
            // Assign it to the renderer
            renderer.sharedMaterial = _materialInstance;
 
            // Set the flag
            _hasMaterialInstance = true;
        }
        else // if we dont have create a new instance, just assign the texture
            renderer.sharedMaterial = newMaterial;        
 
        // We need to recalc the texture tiling (since different material = possible different texture)
        CalcTextureTiling();
    }
 
    private void CalcTextureTiling()
    {
        //set the tile size of the texture (in UV units), based on the rows and columns
		_textureTiling.x = 1f / _maxColsInRows;
		_textureTiling.y = 1f / _rowsTotalInSprite;
 
        // Add in the scale
        _textureTiling.x = _textureTiling.x / _scale.x;
        _textureTiling.y = _textureTiling.y / _scale.y;
 
        // Buffer some of the image out (removes gridlines and stufF)
        _textureTiling -= _buffer;
		
		// Assign the new texture tiling
        // old approach:
		renderer.sharedMaterial.SetTextureScale("_MainTex", _textureTiling);
		// new approach:
		/*renderer.sharedMaterial.SetFloat("_TilingX", _textureTiling.x);
		renderer.sharedMaterial.SetFloat("_TilingY", _textureTiling.y);*/
    }
 
	public void Play()
    {
        // If the animation is playing with a coroutine, stop it
        if (_isPlaying && _useCoroutine)
            StopCoroutine("updateCoroutine");
		
        // Make sure the renderer is enabled
        //renderer.enabled = true;
 
        // Because of the way textures calculate the y value, we need to start at the max y value
		//_index = _rowsTotal * _maxColsCurrentAnim;
        _index = _colLimits[0];
 
        // Start the update tiling coroutine
		if (_useCoroutine)
        	StartCoroutine(updateCoroutine());
		else
			updateTiling();
    }
	
	void Update () {
		if (_useCoroutine || !_isPlaying)
			return;
		float t = Time.time;
		if (t - updateTime > period) {
			updateTiling();
			updateTime = t;
		}
	}
	
	private IEnumerator updateCoroutine() {
		while (true) {
			
			updateTiling();
			
			if (!_isPlaying)
				// Break out of the loop, we are finished
				yield break;
			
			// Wait a time before we move to the next frame. Note, this gives unexpected results on mobile devices
            yield return new WaitForSeconds(period);
        }        
	}
	
    // The main update function of this script
    private void updateTiling()
    {
        _isPlaying = true;
		
		if (_index > _maxIndex) {
			if (_playOnce)
	        {
	            // We are done with the coroutine. Fire the event, if needed
	            if(_enableEvents)
	                HandleCallbacks(_voidEventCallbackList);
	
	            if (_disableUponCompletion)
	                gameObject.renderer.enabled = false;
	
	            _isPlaying = false;
	        }
			_index = _colLimits[0]; // reset index
			if (_pingPongAnim) {
				_direction = -1;
				_index = Mathf.Max(_colLimits[0], _maxIndex - 1); // reset the index fo backward animation
			}
        }
		else if (_pingPongAnim && _index < _colLimits[0]) {
			_direction = 1;
			_index = Mathf.Min(_colLimits[0] + 1, _maxIndex); // reset the index for forward animation
		}
		
		// Apply the offset in order to move to the next frame
        ApplyOffset();
		
        // Increment/Decrement the index of current frame in sprite
        _index += _direction;
    }
 
    private void ApplyOffset() {
		float xTemp = (float)_index / (float)_maxColsInRows;
		float xTempFloor = _index / _maxColsInRows; // operation beetween ints, then result is saved as float
		float x = xTemp - xTempFloor;
		float y = 1f - (xTempFloor / (float)_rowsTotalInSprite) - offsetYStart;
 
		// Reset the y offset, if needed
        if (y == 1f)
            y = 0f;
 
        // If we have scaled the texture, we need to reposition the texture to the center of the object
        x += ((1f / _maxColsInRows) - _textureTiling.x) / 2f;
        y += ((1f / _rowsTotalInSprite) - _textureTiling.y) / 2f;
 
        // Add an additional offset if the user does not want the texture centered
        offsetTemp.x = x + _offset.x;
        offsetTemp.y = y + _offset.y;
 
        // Update the material
        //old approach:
		renderer.sharedMaterial.SetTextureOffset("_MainTex", offsetTemp);
		//new approach:
		/*renderer.sharedMaterial.SetFloat("_OffsetX", offsetTemp.x);
		renderer.sharedMaterial.SetFloat("_OffsetY", offsetTemp.y);*/
		
		// new approach with calculations of offset in the shader
		// setupVec1: _index, _maxColsInRows, _rowsTotalInSprite, offsetYStart
		/*setupVec1.x = _index;
		setupVec1.y = _maxColsInRows;
		setupVec1.z = _rowsTotalInSprite;
		setupVec1.w = offsetYStart;
		// setupVec2: _textureTiling.x, _textureTiling.y, _offset.x, _offset.y
		setupVec2.x = _textureTiling.x;
		setupVec2.y = _textureTiling.y;
		setupVec2.z = _offset.x;
		setupVec2.w = _offset.y;
		// update shader params
		renderer.sharedMaterial.SetVector("_SetupVec1", setupVec1);
		renderer.sharedMaterial.SetVector("_SetupVec2", setupVec2);*/
    }
}
