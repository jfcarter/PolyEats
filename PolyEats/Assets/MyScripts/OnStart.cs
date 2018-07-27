using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace InfinityCode.OnlineMapsExamples
{ 
     /// <summary>
     /// Search a route between two locations and draws the route.
     /// </summary>
[AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/FindDirectionExample")]
public class OnStart : MonoBehaviour {


   
  

        GameObject FullMap;
        int markerToFind = -1;
        public OnlineMapsMarker locMarker;
        bool used = false;
        // Use this for initialization
        IEnumerator Start()
        {
            yield return new WaitForSeconds(2);
            FullMap = GameObject.Find("Map");
            if (GameObject.Find("Storage") != null)
            {

                Text_Changed(GameObject.Find("Storage").GetComponent<Text>().text);
            }

            Destroy(GameObject.Find("Storage"));
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Text_Changed(string newText)
        {
            
            OnlineMapsMarker[] marks = FullMap.GetComponent<OnlineMaps>().markers;
            for (int i = 0; i < marks.Length; i++)
            {
                if (newText.ToUpper().Equals(marks[i].label.ToUpper()))
                {
                    if (used)
                    {
                        Debug.Log("used");
                        locMarker = marks[i];
                        FullMap.GetComponent<FindDirectionExample>().loc = new Vector2(locMarker.position.x, locMarker.position.y);
                        FullMap.GetComponent<FindDirectionExample>().DestroyPath();
                    }
                    Debug.Log("Marker found");
                    locMarker = marks[i];
                    FullMap.GetComponent<FindDirectionExample>().loc = new Vector2(locMarker.position.x, locMarker.position.y);
                    FullMap.GetComponent<FindDirectionExample>().Find();
                    used = true;
                }
                else
                {
                    Debug.Log("Marker not found");
                }
            }
            Debug.Log(marks.Length);
            Debug.Log(newText);
        }
    }
}
	
	

