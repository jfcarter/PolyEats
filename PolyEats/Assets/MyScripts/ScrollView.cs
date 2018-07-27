using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollView : MonoBehaviour
{

    public GameObject button;

    public string SearchField = "";
    public Dictionary<string, Vector2> dict;
    public GameObject map;
    OnlineMapsMarker[] temp;

    // Use this for initialization
    void Start()
    {
       //adds in all markers to the search field
       //m = GameObject.FindGameObjectWithTag("map");
        //temp holds all the markers for buildings, parking lots, open spaces, and more if added
        temp = map.GetComponent<OnlineMaps>().markers;

      
        //clears the dictionary so that if they wanted something specific, other elements won't still be in dict
     
       

        UpdateDictionary();

    }

    public void UpdateDictionary()
    {
        
        dict = new Dictionary<string, Vector2>();
       // dict.Clear();
        //if statement for showing buildings in the search results
        if (/*buildings is selected*/ true)
        {
            //buildings in temp should go up to 119, but if more are added, this will have to be adjusted
            for (int i = 0; i < 120; i++)
            {
                dict.Add(temp[i].label, temp[i].position);
            }
        }

        //if statement for showing parking lots in the search results
        if (/*parking lots is selected*/false)
        {
            //there are 28 parking lots so this should only add those in
            for (int i = 120; i < 148; i++)
            {
                dict.Add(temp[i].label, temp[i].position);
            }
        }
        //if statement for showing open spaces in the search results
        if (/*open spaces is selected*/false)
        {
            //there are 15 open spaces so this shoudl only add those in
            for (int i = 148; i < 163; i++)
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
        List<string> KeyList = new List<string>(this.dict.Keys);

        foreach (string str in KeyList)
        {
            //Debug.Log(str);
            GameObject go = Instantiate(button) as GameObject;
            go.SetActive(true);
            Button tb = go.GetComponent<Button>();
            tb.SetName(str);
            go.transform.SetParent(button.transform.parent);
            go.name = str;
        }

        foreach (OnlineMapsMarker i in map.GetComponent<OnlineMaps>().markers)
        {
            Debug.Log(i.label);
            Debug.Log(i.position);
        }

        GameObject t = GameObject.Find("Button");
        Destroy(t);
        
    }


    public void ButtonClicked(string str)
    {
   
        SearchField = str;
        
    }

    


}

