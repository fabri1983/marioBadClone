using UnityEngine;

public class ImmersiveModeEnabler : MonoBehaviour {

	AndroidJavaObject unityActivity;
	AndroidJavaObject javaObj;
	AndroidJavaClass javaClass;
	bool paused;
	static bool created = false;

	void Awake () {
		if (!Application.isEditor)
			HideNavigationBar();
		if (!created) {
			DontDestroyOnLoad(gameObject);
			created = true;
		}
		else {
			Destroy(gameObject); // duplicate will be destroyed if 'first' scene is reloaded
		}
	}
	
	void HideNavigationBar () {
		#if UNITY_ANDROID
		lock(this)
		{
			using(javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				unityActivity = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			
			if(unityActivity == null)
			{
				return;
			}
			
			using(javaClass = new AndroidJavaClass("org.fabri1983.androidimmersivemode.AndroidImmersiveMode"))
			{
				if(javaClass == null)
				{
					return;
				}
				else
				{
					javaObj = javaClass.CallStatic<AndroidJavaObject>("instance");
					if(javaObj == null)
						return;
					unityActivity.Call("runOnUiThread",new AndroidJavaRunnable(() => 
					                                                           {
						javaObj.Call("EnableImmersiveMode", unityActivity);
					}));
				}
			}
		}
		#endif
	}
	
	void OnApplicationPause (bool pausedState) {
		paused = pausedState;
	}
	
	void OnApplicationFocus (bool hasFocus) {
		if(hasFocus)
		{
			if(javaObj != null && paused != true)
			{
				unityActivity.Call("runOnUiThread",new AndroidJavaRunnable(() => 
						                                                           {
							javaObj.CallStatic("ImmersiveModeFromCache", unityActivity);
						}));
			}
		}
		
	}
	
	// Above android 5.0: App Pinning
	public void PinThisApp () {
		if(javaObj != null)
		{
			javaObj.CallStatic("EnableAppPin",unityActivity);
		}
	}
	
	// Unpin the app
	public void UnPinThisApp () {
		if(javaObj != null)
		{
			javaObj.CallStatic("DisableAppPin",unityActivity);
		}
	}

}
