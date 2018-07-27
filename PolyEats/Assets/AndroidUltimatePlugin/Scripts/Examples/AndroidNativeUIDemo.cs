using UnityEngine;
using System.Collections;

public class AndroidNativeUIDemo : MonoBehaviour {
	
	private AndroidUltimatePluginController androidUltimatePluginController;
	private DemoController demoController;
	private bool isLoading = false;
	
	// Use this for initialization
	void Start () {
		demoController = GameObject.FindObjectOfType<DemoController>();		
		androidUltimatePluginController = AndroidUltimatePluginController.GetInstance();
		androidUltimatePluginController.SetDebug(0);
	}
	
	public void ShowRateUsPopup(){
		androidUltimatePluginController.ShowRatePopup("your rate us title","your rate us message","http://www.google.com");
	}
	
	public void ShowAlertPopup(){
		androidUltimatePluginController.ShowAlertPopup("your native popup title","your native popup message");
	}
	
	public void ShowNativeLoading(){
		if(!isLoading){
			isLoading = true;

			androidUltimatePluginController.ShowNativeLoading("loading please wait...",false);
			Invoke("HideNativeLoading",1f);
		}
	}
	
	public void HideNativeLoading(){
		androidUltimatePluginController.HideNativeLoading();
		isLoading = false;
	}

	public void ShowToastMessage(){
		androidUltimatePluginController.ShowToastMessage("insert your message here");
	}
	
	public void NextDemo(){
		demoController.nextPage();
	}
	
	public void PrevDemo(){
		demoController.prevPage();
	}

	public void Quit(){
		Debug.Log("Quit");
		Application.Quit();
	}
}


