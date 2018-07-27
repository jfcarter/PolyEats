using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScreens : MonoBehaviour
{

    public void LoadScene(string sceneName)
    {
        if (sceneName == "FacialRecog")
        {
            if (EditorUtility.DisplayDialog("Delivery Pressed",
                "Are you sure?", "Yes", "No"))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        else if((sceneName == "HungryRestaurantChooser") || (sceneName == "HustlerRestaurantChooser"))
        {
            GameObject restaurant = GameObject.FindGameObjectWithTag("Choice");
            DestroyObject(restaurant);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
