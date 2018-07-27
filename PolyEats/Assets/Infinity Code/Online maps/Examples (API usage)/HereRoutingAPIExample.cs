/*     INFINITY CODE 2013-2017      */
/*   http://www.infinity-code.com   */

using System.Linq;
using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of a request to HERE Routing API.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/HereRoutingAPIExample")]
    public class HereRoutingAPIExample : MonoBehaviour
    {
        /// <summary>
        /// Application ID
        /// </summary>
        public string appId;

        /// <summary>
        /// Application code
        /// </summary>
        public string appCode;

        private void Start()
        {
            // Looking for public transport route between the coordinates.
            OnlineMapsHereRoutingAPI.Find(
                appId,
                appCode,
                new[] // Waypoints (2+)
                {
                    new OnlineMapsHereRoutingAPI.GeoWaypoint(-117.824441, 34.062447),
                    new OnlineMapsHereRoutingAPI.GeoWaypoint(-117.824208, 34.059593)
                },
                new OnlineMapsHereRoutingAPI.RoutingMode // Routing mode
                {
                    transportMode = OnlineMapsHereRoutingAPI.RoutingMode.TransportModes.pedestrian
                },
                new OnlineMapsHereRoutingAPI.Params // Optional params
                {
                    language = "ru-ru",
                    instructionFormat = OnlineMapsHereRoutingAPI.InstructionFormat.text,
                    routeAttributes = OnlineMapsHereRoutingAPI.RouteAttributes.waypoints | OnlineMapsHereRoutingAPI.RouteAttributes.summary | OnlineMapsHereRoutingAPI.RouteAttributes.legs | OnlineMapsHereRoutingAPI.RouteAttributes.shape,
                    alternatives = 0,
                }
                ).OnComplete += OnComplete;
        }

        /// <summary>
        /// This method is called when a response is received.
        /// </summary>
        /// <param name="response">Response string</param>
        private void OnComplete(string response)
        {
            Debug.Log(response);

            // Get result object
            OnlineMapsHereRoutingAPIResult result = OnlineMapsHereRoutingAPI.GetResult(response);

            if (result != null)
            {
                Debug.Log(result.metaInfo.timestamp);

                Color[] colors =
                {
                    Color.blue,
                    Color.red,
                    Color.green,
                    Color.magenta
                };
                int colorIndex = 0;

                // Draw all the routes in different colors.
                foreach (OnlineMapsHereRoutingAPIResult.Route route in result.routes)
                {
                    if (route.shape != null)
                    {
                        OnlineMapsDrawingElement line = new OnlineMapsDrawingLine(route.shape.Select(v => new Vector2((float)v.longitude, (float)v.latitude)).ToList(), colors[colorIndex++]);
                        OnlineMaps.instance.AddDrawingElement(line);
                    }
                }
            }
        }
    }
}