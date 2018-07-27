using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaymentCamera : MonoBehaviour {

    int currentCamIndex = 0;

    WebCamTexture tex;

    public RawImage currentPicture;

    public void SwapCam_Clicked()
    {
        //if device has multiple cameras only use one
        if (WebCamTexture.devices.Length > 0)
        {
            currentCamIndex += 1;
            currentCamIndex %= WebCamTexture.devices.Length;
        }
    }

    public void StartStopCam()
    {
        if (tex != null)
        {
            //takes photo
            currentPicture.texture = tex;
            //stops camera
            tex.Stop();
            tex = null;
            //change button text
            Text text = GameObject.FindGameObjectWithTag("Photo_Button").GetComponentInChildren<Text>();
            text.text = "Take Payment Photo";
        }
        else
        {
            //gets which device to use
            WebCamDevice device = WebCamTexture.devices[currentCamIndex];
            tex = new WebCamTexture(device.name);
            //starts camera
            tex.Play();
            //gets you in frame
            currentPicture.texture = tex;
            //change button text
            Text text = GameObject.FindGameObjectWithTag("Photo_Button").GetComponentInChildren<Text>();
            text.text = "Snap Picture";
        }
    }
}
