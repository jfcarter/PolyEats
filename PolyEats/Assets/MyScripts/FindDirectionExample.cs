/*     INFINITY CODE 2013-2017      */
/*   http://www.infinity-code.com   */

using System.Collections.Generic;
using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Search a route between two locations and draws the route.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/FindDirectionExample")]
    public class FindDirectionExample : MonoBehaviour
    {
        public Vector2 loc;
        public OnlineMapsDrawingLine route;

        public void Find()
        {
            Debug.Log("Im active");
            // Begin to search a route from Los Angeles to the specified coordinates.
            GameObject map = GameObject.FindGameObjectWithTag("map");
            Vector2 player = new Vector2(map.GetComponent<UserLocation>().longitude, map.GetComponent<UserLocation>().latitude);
            //location x and y are backwards
            //OnlineMapsGoogleDirections.Mode.walking.Find
            OnlineMapsGoogleDirections query = OnlineMapsGoogleDirections.Find(player,
                loc);

            // Specifies that search results must be sent to OnFindDirectionComplete.
            query.OnComplete += OnFindDirectionComplete;
        }

        public void DestroyPath()
        {
            OnlineMaps.instance.RemoveAllDrawingElements();
        }

        private void OnFindDirectionComplete(string response)
        {
            // Get the route steps.
            List<OnlineMapsDirectionStep> steps = OnlineMapsDirectionStep.TryParse(response);

            if (steps != null)
            {
                // Showing the console instructions for each step.
                //foreach (OnlineMapsDirectionStep step in steps) Debug.Log(step.stringInstructions);

                // Get all the points of the route.
                List<Vector2> points = OnlineMapsDirectionStep.GetPoints(steps);

                // Create a line, on the basis of points of the route.
                route = new OnlineMapsDrawingLine(points, Color.green);

                // Draw the line route on the map.
                OnlineMaps.instance.AddDrawingElement(route);
            }
            else
            {
                Debug.Log("Find direction failed");
            }
        }
    }
}