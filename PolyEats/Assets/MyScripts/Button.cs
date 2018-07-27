using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour {

    private string Name;
    public Text ButtonText;
    public ScrollView scrollView;
    public GameObject Storage;
  

    

    public void SetName(string name)
    {
        Name = name;
        ButtonText.text = name;
    }

    public void Button_Click()
    {
        scrollView.ButtonClicked(name);
        DontDestroyOnLoad(Storage.transform);
        Storage.GetComponent<Text>().text = name;
        SceneManager.LoadScene(0);

        

    }


}
