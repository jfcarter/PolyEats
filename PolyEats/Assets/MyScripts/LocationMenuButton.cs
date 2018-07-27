using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationMenuButton : MonoBehaviour {

    public GameObject button;

    public void Button_Click()
    {
        SceneManager.LoadScene(3);
    }
	
	
}
