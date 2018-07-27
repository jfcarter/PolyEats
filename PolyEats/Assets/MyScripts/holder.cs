using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class holder : MonoBehaviour {

    public Dictionary<string, Vector2> dict;
    public GameObject m;
    // Use this for initialization
    void Start () {
        dict = new Dictionary<string, Vector2>();
        //adds in all markers to the search field
        m = GameObject.FindGameObjectWithTag("map");
        /*
        foreach (OnlineMapsMarker i in m.GetComponent<OnlineMaps>().markers)
        {
            dict.Add(i.label, i.position);
        }*/
        //temp holds all the markers for buildings, parking lots, open spaces, and more if added
        OnlineMapsMarker[] temp = m.GetComponent<OnlineMaps>().markers;
        //clears the dictionary so that if they wanted something specific, other elements won't still be in dict
        //dict.Clear();
        //if statement for showing buildings in the search results
        if (/*buildings is selected*/ true)
        {
            //buildings in temp should go up to 119, but if more are added, this will have to be adjusted
            for (int i = 0; i < 119; i++)
            {
                dict.Add(temp[i].label, temp[i].position);
            }
        }
        //if statement for showing parking lots in the search results
        if (/*parking lots is selected*/false)
        {
            //there are 28 parking lots so this should only add those in
            for (int i = 119; i < 147; i++)
            {
                dict.Add(temp[i].label, temp[i].position);
            }
        }
        //if statement for showing open spaces in the search results
        if (/*open spaces is selected*/false)
        {
            //there are 15 open spaces so this shoudl only add those in
            for (int i = 147; i < 161; i++)
            {
                dict.Add(temp[i].label, temp[i].position);
            }
        }
        if (/*nothing is selected*/ false)
        {
            foreach (OnlineMapsMarker i in temp)
            {
                dict.Add(i.label, i.position);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
