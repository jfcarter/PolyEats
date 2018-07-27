using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Utils{
	public static void Message(string tag, string message){
		Debug.LogWarning(tag + message);
	}

	//take screen shot then share via intent
	public static IEnumerator TakeScreenshot(string screenShotPath){
		yield return new WaitForEndOfFrame();
		
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		
		// Read screen contents into the texture
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);

		tex.Apply();
		
		//saving to phone storage
		byte[] screenshot = tex.EncodeToPNG();
		System.IO.File.WriteAllBytes(screenShotPath,screenshot);
	}
}
