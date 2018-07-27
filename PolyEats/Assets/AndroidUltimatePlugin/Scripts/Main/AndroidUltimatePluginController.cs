using UnityEngine;
using System.Collections;
using System;

public class AndroidUltimatePluginController : MonoBehaviour {
	
	private static AndroidUltimatePluginController instance;
	private static GameObject container;
	private static AUPHolder aupHolder;
	
	#if UNITY_ANDROID
	private static AndroidJavaObject jo;
	#endif	
	
	public bool isDebug =true;
	
	public static AndroidUltimatePluginController GetInstance(){
		if(instance==null){
			container = new GameObject();
			container.name="AndroidUltimatePluginController";
			instance = container.AddComponent( typeof(AndroidUltimatePluginController) ) as AndroidUltimatePluginController;
			DontDestroyOnLoad(instance.gameObject);
			aupHolder = AUPHolder.GetInstance();
			instance.gameObject.transform.SetParent(aupHolder.gameObject.transform);
		}
		
		return instance;
	}
	
	private void Awake(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo = new AndroidJavaObject("com.gigadrillgames.androidplugin.main.AndroidUltimatePlugin");
		}
		#endif
	}

	/// <summary>
	/// Sets the debug.
	/// 0 - false, 1 - true
	/// </summary>
	/// <param name="debug">Debug.</param>
	public void SetDebug(int debug){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("SetDebug",debug);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	//----------------------------------------------[Share Intent]-----------------------------------------------------
	//share intent
	/// <summary>
	/// Shares the image.
	/// </summary>
	/// <param name="subject">Subject.</param>
	/// <param name="subjectContent">Subject content.</param>
	/// <param name="imagepath">Imagepath.</param>
	public void ShareImage(string subject,string subjectContent, string imagepath){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("shareImage",subject,subjectContent,imagepath);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	/// <summary>
	/// Shares the URL.
	/// </summary>
	/// <param name="subject">Subject.</param>
	/// <param name="subjectContent">Subject content.</param>
	/// <param name="url">URL.</param>
	public void ShareUrl(string subject,string subjectContent, string url){		
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("shareUrl",subject,subjectContent,url);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	//----------------------------------------------[Share Intent]-----------------------------------------------------


	//----------------------------------------------[Local Notification]-----------------------------------------------------

	//note: local notification will not show if the application is currently running

	/// <summary>
	/// Schedules the notification.
	/// </summary>
	/// <param name="notificationTitle">Notification title.</param>
	/// <param name="notificationMessage">Notification message.</param>
	/// <param name="notificationTickerMessage">Notification ticker message.</param>
	/// <param name="delay">Delay.</param>
	/// <param name="enableVibrate">If set to <c>true</c> enable vibrate.</param>
	/// <param name="enableSound">If set to <c>true</c> enable sound.</param>
	public void ScheduleNotification(string notificationTitle,string notificationMessage,string notificationTickerMessage, int delay,bool enableVibrate,bool enableSound ){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("scheduleNotification",notificationTitle,notificationMessage,notificationTickerMessage,delay,enableVibrate,enableSound);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}
	
	public void ShowScheduleSimpleNotification(string notificationTitle,string notificationMessage,string notificationTickerMessage,bool enableVibrate,bool enableSound){		
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("showSimpleNotification",notificationTitle,notificationMessage,notificationTickerMessage,enableVibrate,enableSound);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}
	
	/// <summary>
	/// Determines whether this instance is open using notification.
	/// </summary>
	/// <returns><c>true</c> if this instance is open using notification; otherwise, <c>false</c>.</returns>
	public int IsOpenUsingNotification(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			return jo.CallStatic<int>("isOpenUsingNotification");
		}else{
			Message("warning: must run in actual android device");
		}
		#endif

		return 0;
	}

	//----------------------------------------------[Local Notification]-----------------------------------------------------


	//----------------------------------------------[Immersive]-------------------------------------------------------------
	//immersive
	//only support kitkat and above version
	/// <summary>
	/// set immersive mode on
	/// , note:only support kitkat and above android version 4.4 api 19
	/// </summary>
	/// <param name="delay">Delay.</param>
	public void ImmersiveOn(int delay){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("immersiveOn",delay);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	/// <summary>
	/// Immersives the off.
	/// </summary>
	public void ImmersiveOff(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("immersiveOff");
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	//----------------------------------------------[Immersive]-------------------------------------------------------------


	//----------------------------------------------[Android Native UI]-------------------------------------------------------------
	/// <summary>
	/// Shows the toast message.
	/// </summary>
	/// <param name="message">Message.</param>
	public void ShowToastMessage(String message){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("showToastMessage",message);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}


	//native popup
	//rate us
	/// <summary>
	/// Shows the  native rate popup.
	/// </summary>
	/// <param name="title">Title.</param>
	/// <param name="message">Message.</param>
	/// <param name="url">URL.</param>
	public void ShowRatePopup(String title,String message,String url){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("showRatePopup",title,message,url);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	/// <summary>
	/// Shows the native alert popup.
	/// </summary>
	/// <param name="title">Title.</param>
	/// <param name="message">Message.</param>
	public void ShowAlertPopup(String title,String message){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("showNativePopup",title,message);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}
	//native popup

	//native loading
	/// <summary>
	/// Shows the native loading.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="isCancelable">If set to <c>true</c> is cancelable.</param>
	public void ShowNativeLoading(String message,bool isCancelable){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("showNativeLoading",message,isCancelable);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	/// <summary>
	/// Hides the native loading.
	/// </summary>
	public void HideNativeLoading(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("hideNativeLoading");
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}
	//----------------------------------------------[Android Native UI]-------------------------------------------------------------



	public void ShowMessage(string message){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.Call("ShowMessage", message);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}
	
	private void Message(string message){
		if(isDebug){
			Debug.LogWarning(message);
		}
	}
}
