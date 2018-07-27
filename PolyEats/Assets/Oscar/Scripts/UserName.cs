using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserName : MonoBehaviour {

    static UserName instance = null;
    public User username;

    // on creates
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // if duplicate -> remove
        }
        else
        {
            instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }

    }

    public void SetUsername(User s)
    {
        this.username = s;
    }
}
