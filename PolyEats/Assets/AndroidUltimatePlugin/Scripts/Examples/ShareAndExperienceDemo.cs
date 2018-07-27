using UnityEngine;
using System.Collections;

public class ShareAndExperienceDemo : MonoBehaviour {
	
	private AndroidUltimatePluginController androidUltimatePluginController;
	private bool isImmersive = false;
	
	// Use this for initialization
	void Start (){
		androidUltimatePluginController = AndroidUltimatePluginController.GetInstance();
	}
	
	public void ActivateLocalNotification(){
		Debug.Log("ActivateScheduleNotification");
		//schedule notification
		//1000 = 1 sec
		int delay = 3000;
		androidUltimatePluginController.ScheduleNotification("my notification title","my notification message","my notification ticker message",delay,true,true);
	}

	public void ImmersiveToggle(){
		if(!isImmersive){
			androidUltimatePluginController.ImmersiveOn(500);
			isImmersive = true;
		}else{
			androidUltimatePluginController.ImmersiveOff();
			isImmersive = false;
		}
	}

	public void ShareText(){		
		//share link
		androidUltimatePluginController.ShareUrl("my subject","my subject content","https://www.urltoshare.com");
	}

	public void ShareImage(){

		string screenShotName = "AndroidUltimateScreenShot.png";
		string path = Application.persistentDataPath + "/" + screenShotName;

		StartCoroutine(Utils.TakeScreenshot(path));
		androidUltimatePluginController.ShareImage("subject","subjectContent",path);
	}
}