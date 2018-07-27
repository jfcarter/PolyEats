using UnityEngine;
using System.Collections;

public class IntroController : MonoBehaviour {

	private AndroidUltimatePluginController androidUltimatePluginController;

	// Use this for initialization
	void Start (){
		//init android plugin 
		androidUltimatePluginController = AndroidUltimatePluginController.GetInstance();

		//when set to 1 shows debug toast messages ,if set to 0 will not show any debug toast message on android
		androidUltimatePluginController.SetDebug(0);
	}
}
