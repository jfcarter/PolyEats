using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class RestaurantChooser : MonoBehaviour {

    public string restaurant;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void LoadScene(string sceneName)
    {
        restaurant = EventSystem.current.currentSelectedGameObject.name;
        SceneManager.LoadScene(sceneName);
    }
}
