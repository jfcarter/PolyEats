/*     INFINITY CODE 2013-2017      */
/*   http://www.infinity-code.com   */

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define UNITY_5_2L
#else 
#define UNITY_5_3P
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if !UNITY_WEBGL
using System.Threading;
using UnityEngine.UI;
using Firebase.Storage;
using System.Threading.Tasks;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// The main class. With it you can control the map.
/// </summary>
[AddComponentMenu("Infinity Code/Online Maps/Online Maps")]
[Serializable]
public class OnlineMaps : MonoBehaviour, ISerializationCallbackReceiver
{
    #region Variables

    private GameObject toggler;
    private GameObject deliveryButton;
    private bool markerClicked = false;

    /// <summary>
    /// The current version of Online Maps
    /// </summary>
    public const string version = "2.5.6.1";

    /// <summary>
    /// The maximum zoom level.
    /// </summary>
    public const int MAXZOOM = 20;

    /// <summary>
    /// The maximum number simultaneously downloading tiles.
    /// </summary>
    public static int maxTileDownloads = 5;

    /// <summary>
    /// Allows you to customize the appearance of the tooltip.
    /// </summary>
    /// <param name="style">The reference to the style.</param>
    public delegate void OnPrepareTooltipStyleDelegate(ref GUIStyle style);

    /// <summary>
    /// Intercepts creates a marker.\n
    /// Return null to create marker using built-in manager.\n
    /// Return instance of marker to prevent using built-in manager.
    /// </summary>
    public Func<double, double, Texture2D, string, OnlineMapsMarker> OnAddMarker;

    /// <summary>
    /// Event caused when the user change map position.
    /// </summary>
    public Action OnChangePosition;

    /// <summary>
    /// Event caused when the user change map zoom.
    /// </summary>
    public Action OnChangeZoom;

    /// <summary>
    /// The event which is caused by garbage collection.\n
    /// This allows you to manage the work of the garbage collector.
    /// </summary>
    [Obsolete]
    public Action OnGCCollect;

    /// <summary>
    /// The event is invoked at the end LateUpdate.
    /// </summary>
    public Action OnLateUpdateAfter;

    /// <summary>
    /// The event is called at the start LateUpdate.
    /// </summary>
    public Action OnLateUpdateBefore;

    /// <summary>
    /// Event which is called after the redrawing of the map.
    /// </summary>
    public Action OnMapUpdated;

    /// <summary>
    /// The event occurs after the addition of the marker.
    /// </summary>
    public Action<OnlineMapsMarker> OnMarkerAdded;

    /// <summary>
    /// The event occurs after generating buffer and before update control to preload tiles for tileset.
    /// </summary>
    public Action OnPreloadTiles;

    /// <summary>
    /// Event caused when preparing tooltip style.
    /// </summary>
    public OnPrepareTooltipStyleDelegate OnPrepareTooltipStyle;

    /// <summary>
    /// An event that occurs when loading the tile. Allows you to intercept of loading tile, and load it yourself.
    /// </summary>
    public Action<OnlineMapsTile> OnStartDownloadTile;

    /// <summary>
    /// Intercepts removes a marker.\n
    /// Return FALSE to remove marker using built-in manager.\n
    /// Return TRUE to prevent using built-in manager.
    /// </summary>
    public Predicate<OnlineMapsMarker> OnRemoveMarker;

    /// <summary>
    /// Intercepts removes a marker.\n
    /// Return FALSE to remove marker using built-in manager.\n
    /// Return TRUE to prevent using built-in manager.
    /// </summary>
    public Predicate<int> OnRemoveMarkerAt;

    /// <summary>
    /// Invoked before saving settings.
    /// </summary>
    public Action OnSaveSettings;

    /// <summary>
    /// Event is called before Update.
    /// </summary>
    public Action OnUpdateBefore;

    /// <summary>
    /// Event is called after Update.
    /// </summary>
    public Action OnUpdateLate;

    /// <summary>
    /// Specifies whether the user interacts with the map.
    /// </summary>
    public static bool isUserControl = false;

    private static OnlineMaps _instance;

    /// <summary>
    /// Allows drawing of map.\n
    /// <strong>
    /// Important: The interaction with the map, add or remove markers and drawing elements, automatically allowed to redraw the map.\n
    /// Use lockRedraw, to prohibit the redrawing of the map.
    /// </strong>
    /// </summary>
    public bool allowRedraw;
 
    /// <summary>
    /// Display control script.
    /// </summary>
    public OnlineMapsControlBase control;

    /// <summary>
    /// URL of custom provider.\n
    /// Support tokens:\n
    /// {x} - tile x\n
    /// {y} - tile y\n
    /// {zoom} - zoom level\n
    /// {quad} - uniquely identifies a single tile at a particular level of detail.
    /// </summary>
    public string customProviderURL = "http://localhost/{zoom}/{y}/{x}";

    /// <summary>
    /// URL of custom traffic provider.\n
    /// Support tokens:\n
    /// {x} - tile x\n
    /// {y} - tile y\n
    /// {zoom} - zoom level\n
    /// {quad} - uniquely identifies a single tile at a particular level of detail.
    /// </summary>
    public string customTrafficProviderURL = "http://localhost/{zoom}/{y}/{x}";

    /// <summary>
    /// Alignment marker default.
    /// </summary>
    public OnlineMapsAlign defaultMarkerAlign = OnlineMapsAlign.Bottom;

    /// <summary>
    /// Texture used by default for the marker.
    /// </summary>
    public Texture2D defaultMarkerTexture;

    /// <summary>
    /// Texture displayed until the tile is not loaded.
    /// </summary>
    public Texture2D defaultTileTexture;

    /// <summary>
    /// Specifies whether to dispatch the event.
    /// </summary>
    public bool dispatchEvents = true;

    /// <summary>
    /// The drawing elements.
    /// </summary>
    public List<OnlineMapsDrawingElement> drawingElements;

    /// <summary>
    /// Color, which is used until the tile is not loaded, unless specified field defaultTileTexture.
    /// </summary>
    public Color emptyColor = Color.gray;

    /// <summary>
    /// Map height in pixels.
    /// </summary>
    public int height;

    /// <summary>
    /// Specifies whether to display the labels on the map.
    /// </summary>
    public bool labels = true;

    /// <summary>
    /// Language of the labels on the map.
    /// </summary>
    public string language = "en";

    /// <summary>
    /// Prohibits drawing of maps.\n
    /// <strong>
    /// Important: Do not forget to disable this restriction. \n
    /// Otherwise, the map will never be redrawn.
    /// </strong>
    /// </summary>
    public bool lockRedraw = false;

    /// <summary>
    /// List of all 2D markers. <br/>
    /// Use AddMarker, RemoveMarker and RemoveAllMarkers.
    /// </summary>
    public OnlineMapsMarker[] markers;

    /// <summary>
    /// A flag that indicates that need to redraw the map.
    /// </summary>
    public bool needRedraw;

    /// <summary>
    /// Not interact under the GUI.
    /// </summary>
    public bool notInteractUnderGUI = true;

    /// <summary>
    /// Limits the range of map coordinates.
    /// </summary>
    [NonSerialized]
    public OnlineMapsPositionRange positionRange;

    /// <summary>
    /// Map provider.
    /// </summary>
    [Obsolete("Use OnlineMapsProvider class")]
    public OnlineMapsProviderEnum provider = OnlineMapsProviderEnum.nokia;

    /// <summary>
    /// ID of current map type.
    /// </summary>
    public string mapType;

    /// <summary>
    /// A flag that indicates whether to redraw the map at startup.
    /// </summary>
    public bool redrawOnPlay;

    /// <summary>
    /// Render map in a separate thread. Recommended.
    /// </summary>
    public bool renderInThread = true;

    /// <summary>
    /// Template path in Resources, from where the tiles will be loaded.\n
    /// This field supports tokens.
    /// </summary>
    public string resourcesPath = "OnlineMapsTiles/{zoom}/{x}/{y}";

    /// <summary>
    /// Indicates when the marker will show tips.
    /// </summary>
    public OnlineMapsShowMarkerTooltip showMarkerTooltip = OnlineMapsShowMarkerTooltip.onHover;

    /// <summary>
    /// Reduced texture that is displayed when the user move map.
    /// </summary>
    public Texture2D smartTexture;

    /// <summary>
    /// Specifies from where the tiles should be loaded (Online, Resources, Online and Resources).
    /// </summary>
    public OnlineMapsSource source = OnlineMapsSource.Online;

    /// <summary>
    /// Indicates that Unity need to stop playing when compiling scripts.
    /// </summary>
    public bool stopPlayingWhenScriptsCompile = true;

    /// <summary>
    /// Specifies where the map should be drawn (Texture or Tileset).
    /// </summary>
    public OnlineMapsTarget target = OnlineMapsTarget.texture;

    /// <summary>
    /// Texture, which is used to draw the map. <br/>
    /// <strong>To change this value, use OnlineMaps.SetTexture.</strong>
    /// </summary>
    public Texture2D texture;

    /// <summary>
    /// Width of tileset in pixels.
    /// </summary>
    public int tilesetWidth = 1024;

    /// <summary>
    /// Height of tileset in pixels.
    /// </summary>
    public int tilesetHeight = 1024;

    /// <summary>
    /// Tileset size in scene;
    /// </summary>
    public Vector2 tilesetSize = new Vector2(1024, 1024);

    /// <summary>
    /// Tooltip, which will be shown.
    /// </summary>
    public string tooltip = string.Empty;

    /// <summary>
    /// Drawing element for which displays tooltip.
    /// </summary>
    [NonSerialized]
    public OnlineMapsDrawingElement tooltipDrawingElement;

    /// <summary>
    /// Marker for which displays tooltip.
    /// </summary>
    public OnlineMapsMarkerBase tooltipMarker;

    /// <summary>
    /// Background texture of tooltip.
    /// </summary>
    public Texture2D tooltipBackgroundTexture;

    /// <summary>
    /// Specifies whether to draw traffic.
    /// </summary>
    public bool traffic = false;

    /// <summary>
    /// Provider of traffic jams
    /// </summary>
    [NonSerialized]
    public OnlineMapsTrafficProvider trafficProvider;

    /// <summary>
    /// ID of current traffic provider.
    /// </summary>
    public string trafficProviderID = "googlemaps";

    /// <summary>
    /// Map type.
    /// </summary>
    [Obsolete("Use OnlineMapsProvider class")]
    public int type;

    /// <summary>
    /// Use only the current zoom level of the tiles.
    /// </summary>
    public bool useCurrentZoomTiles = false;

    /// <summary>
    /// Specifies is necessary to use software JPEG decoder.
    /// Use only if you have problems with hardware decoding of JPEG.
    /// </summary>
    public bool useSoftwareJPEGDecoder = false;

    /// <summary>
    /// Specifies whether when you move the map showing the reduction texture.
    /// </summary>
    public bool useSmartTexture = true;

    /// <summary>
    /// Use a proxy server for Webplayer and WebGL?
    /// </summary>
    public bool useWebplayerProxy = true;

    /// <summary>
    /// URL of the proxy server used for Webplayer platform.
    /// </summary>
    public string webplayerProxyURL = "http://service.infinity-code.com/redirect.php?";

    /// <summary>
    /// Map width in pixels.
    /// </summary>
    public int width;

    /// <summary>
    /// Specifies the valid range of map zoom.
    /// </summary>
    [NonSerialized]
    public OnlineMapsRange zoomRange;

    [SerializeField]
    private double latitude;

    [SerializeField]
    private double longitude;

    [SerializeField]
    private int _zoom;

    [NonSerialized]
    private OnlineMapsProvider.MapType _activeType;

    [NonSerialized]
    private OnlineMapsBuffer _buffer;
    private bool _labels;
    private string _language;
    private string _mapType;
    private bool _traffic;
    
    private Color[] defaultColors;
    private OnlineMapsRedrawType redrawType = OnlineMapsRedrawType.none;
    private OnlineMapsMarkerBase rolledMarker;
    private GUIStyle tooltipStyle;

#if NETFX_CORE
    private OnlineMapsThreadWINRT renderThread;
#elif !UNITY_WEBGL
    private Thread renderThread;
#endif

    private double bottomRightLatitude;
    private double bottomRightLongitude;
    private double topLeftLatitude;
    private double topLeftLongitude;
    public Text orderWords;

    [SerializeField]
    private string _activeTypeSettings;

    private string _trafficProviderID;
    private OnlineMapsProjection _projection;

    #endregion

#region Properties

    /// <summary>
    /// Singleton instance of map.
    /// </summary>
    public static OnlineMaps instance
    {
        get { return _instance; }
    }

    /// <summary>
    /// Active type of map.
    /// </summary>
    public OnlineMapsProvider.MapType activeType
    {
        get
        {
            if (_activeType == null || _activeType.fullID != mapType)
            {
                _activeType = OnlineMapsProvider.FindMapType(mapType);
                _projection = _activeType.provider.projection;
                mapType = _activeType.fullID;
            }
            return _activeType;
        }
        set
        {
            if (_activeType == value) return;

            _activeType = value;
            _projection = _activeType.provider.projection;
            _mapType = mapType = value.fullID;

            if (Application.isPlaying) RedrawImmediately();
        }
    }

    /// <summary>
    /// Gets the bottom right position.
    /// </summary>
    /// <value>
    /// The bottom right position.
    /// </value>
    public Vector2 bottomRightPosition
    {
        get
        {
            if (Math.Abs(bottomRightLatitude) < double.Epsilon && Math.Abs(bottomRightLongitude) < double.Epsilon) UpdateBottonRightPosition();
            return new Vector2((float)bottomRightLongitude, (float)bottomRightLatitude);
        }
    }

    /// <summary>
    /// Reference to the current draw buffer.
    /// </summary>
    public OnlineMapsBuffer buffer
    {
        get
        {
            if (_buffer == null) _buffer = new OnlineMapsBuffer(this);
            return _buffer;
        }
    }

    /// <summary>
    /// The current state of the drawing buffer.
    /// </summary>
    public OnlineMapsBufferStatus bufferStatus
    {
        get { return buffer.status; }
    }

    /// <summary>
    /// Coordinates of the center point of the map.
    /// </summary>
    public Vector2 position
    {
        get { return new Vector2((float)longitude, (float)latitude); }
        set
        {
            SetPosition(value.x, value.y);
        }
    }

    /// <summary>
    /// Projection of active provider.
    /// </summary>
    public OnlineMapsProjection projection
    {
        get
        {
            if (_projection == null) _projection = activeType.provider.projection;
            return _projection;
        }
    }

    /// <summary>
    /// Gets the top left position.
    /// </summary>
    /// <value>
    /// The top left position.
    /// </value>
    public Vector2 topLeftPosition
    {
        get
        {
            if (Math.Abs(topLeftLatitude) < double.Epsilon && Math.Abs(topLeftLongitude) < double.Epsilon) UpdateTopLeftPosition();

            return new Vector2((float)topLeftLongitude, (float)topLeftLatitude);
        }
    }

    /// <summary>
    /// Current zoom.
    /// </summary>
    public int zoom
    {
        get { return _zoom; }
        set
        {
            int z = Mathf.Clamp(value, 3, MAXZOOM);
            if (zoomRange != null) z = zoomRange.CheckAndFix(z);
            z = CheckMapSize(z);
            if (_zoom == z) return;

            _zoom = z;
            UpdateBottonRightPosition();
            UpdateTopLeftPosition();
            allowRedraw = true;
            needRedraw = true;
            redrawType = OnlineMapsRedrawType.full;
            DispatchEvent(OnlineMapsEvents.changedZoom);
        }
    }

#endregion

#region Methods

    /// <summary>
    /// Adds a drawing element.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    public void AddDrawingElement(OnlineMapsDrawingElement element)
    {
        if (element.Validate())
        {
            drawingElements.Add(element);
            needRedraw = allowRedraw = true;
            redrawType = OnlineMapsRedrawType.full;
            buffer.updateBackBuffer = true;
        }
    }

    /// <summary>
    /// Adds a 2D marker on the map.
    /// </summary>
    /// <param name="marker">
    /// The marker you want to add.
    /// </param>
    /// <returns>
    /// Marker instance.
    /// </returns>
    public OnlineMapsMarker AddMarker(OnlineMapsMarker marker)
    {
        marker.Init();
        needRedraw = allowRedraw = true;
        Array.Resize(ref markers, markers.Length + 1);
        markers[markers.Length - 1] = marker;
        if (OnMarkerAdded != null) OnMarkerAdded(marker);
        return marker;
    }

    /// <summary>
    /// Adds a new 2D marker on the map.
    /// </summary>
    /// <param name="markerPosition">X - Longituge. Y - Latitude.</param>
    /// <param name="label">The text that will be displayed when you hover a marker.</param>
    /// <returns>Marker instance.</returns>
    public OnlineMapsMarker AddMarker(Vector2 markerPosition, string label)
    {
        return AddMarker(markerPosition.x, markerPosition.y, null, label);
    }

    /// <summary>
    /// Adds a new 2D marker on the map.
    /// </summary>
    /// <param name="markerLng">Marker longitude.</param>
    /// <param name="markerLat">Marker latitude.</param>
    /// <param name="label">The text that will be displayed when you hover a marker.</param>
    /// <returns>Marker instance.</returns>
    public OnlineMapsMarker AddMarker(double markerLng, double markerLat, string label)
    {
        return AddMarker(markerLng, markerLat, null, label);
    }

    /// <summary>
    /// Adds a new 2D marker on the map.
    /// </summary>
    /// <param name="markerPosition">X - Longituge. Y - Latitude.</param>
    /// <param name="markerTexture">
    /// <strong>Optional</strong><br/>
    /// Marker texture. <br/>
    /// In import settings must be enabled "Read / Write enabled". <br/>
    /// Texture format: ARGB32. <br/>
    /// If not specified, the will be used default marker texture.</param>
    /// <param name="label">
    /// <strong>Optional</strong><br/>
    /// The text that will be displayed when you hover a marker.</param>
    /// <returns>Marker instance.</returns>
    public OnlineMapsMarker AddMarker(Vector2 markerPosition, Texture2D markerTexture = null, string label = "")
    {
        return AddMarker(markerPosition.x, markerPosition.y, markerTexture, label);
    }

    /// <summary>
    /// Adds a new 2D marker on the map.
    /// </summary>
    /// <param name="markerLng">Marker longitude.</param>
    /// <param name="markerLat">Marker latitude.</param>
    /// <param name="markerTexture"><strong>Optional</strong><br/>
    /// Marker texture. <br/>
    /// In import settings must be enabled "Read / Write enabled". <br/>
    /// Texture format: ARGB32. <br/>
    /// If not specified, the will be used default marker texture.</param>
    /// <param name="label">
    /// <strong>Optional</strong><br/>
    /// The text that will be displayed when you hover a marker.</param>
    /// <returns>Marker instance.</returns>
    public OnlineMapsMarker AddMarker(double markerLng, double markerLat, Texture2D markerTexture = null, string label = "")
    {
        if (markerTexture == null) markerTexture = defaultMarkerTexture;

        OnlineMapsMarker marker;

        if (OnAddMarker != null)
        {
            marker = OnAddMarker(markerLng, markerLat, markerTexture, label);
            if (marker != null) return marker;
        }

        marker = new OnlineMapsMarker
        {
            texture = markerTexture,
            label = label,
            align = defaultMarkerAlign
        };
        marker.SetPosition(markerLng, markerLat);
        marker.Init();
        Array.Resize(ref markers, markers.Length + 1);
        markers[markers.Length - 1] = marker;
        needRedraw = allowRedraw = true;
        redrawType = OnlineMapsRedrawType.full;
        buffer.updateBackBuffer = true;

        if (OnMarkerAdded != null) OnMarkerAdded(marker);

        return marker;
    }

    /// <summary>
    /// Adds a 2D markers on the map.
    /// </summary>
    /// <param name="newMarkers">
    /// The markers.
    /// </param>
    public void AddMarkers(OnlineMapsMarker[] newMarkers)
    {
        int markersCount = markers.Length;
        int newCount = markersCount + newMarkers.Length;

        Array.Resize(ref markers, newCount);

        for (int i = 0; i < newMarkers.Length; i++)
        {
            OnlineMapsMarker marker = newMarkers[i];
            marker.Init();
            markers[i + markersCount] = marker;
        }

        needRedraw = allowRedraw = true;
        redrawType = OnlineMapsRedrawType.full;
        buffer.updateBackBuffer = true;
    }

    private void OnMarkerClick (OnlineMapsMarkerBase marker)
    {
        Debug.Log("You clicked a marker");
        if (!markerClicked)
        {
            markerClicked = true;

            //stop updating map and remove all displayed markers
            GameObject map = GameObject.FindGameObjectWithTag("Map_Controller");
            map.GetComponent<MapUpdater>().stopMapUpdate();
            hideMarkers();
            marker.enabled = true;

            //turn on messaging and toggle button
            toggler.GetComponent<ToggleMessanger>().TurnOnToggleButton();
            toggler.GetComponent<ToggleMessanger>().Toggle();

            //find gameobject that controls messaging and start messaging protocol
            GameObject messaging = GameObject.FindGameObjectWithTag("messaging");
            messaging.GetComponent<Messaging>().startMessageUpdate(marker.user);

            //get picture for later verification use
            //GetPicture(marker.user); //this method shows how to set the database downloaded picture to a texture
            GameObject pHolder = GameObject.FindGameObjectWithTag("Picture_Holder");
            pHolder.GetComponent<PictureHolder>().setPicture(marker.user);

            //make list of food appear, show delivered button, and hide back button
            GameObject orderInfo = GameObject.FindGameObjectWithTag("OrderController");
            orderInfo.GetComponent<OrderWriter>().WriteOrder("Bldg: " + marker.buildingNumber + "-" + marker.roomNumber + ", Price: " + marker.price + "\n" + marker.orderForPerson);
            deliveryButton.SetActive(true);
            GameObject back = GameObject.FindGameObjectWithTag("Back");
            back.SetActive(false);
        }
    }

    public void GetPicture(string user)
    {
        // Get a reference to the storage service, using the default Firebase App
        FirebaseStorage storage = FirebaseStorage.GetInstance("gs://broncobytes-35974.appspot.com");

        // Create a reference to user's storage
        StorageReference images_ref = storage.RootReference.Child(user);

        // Load image from database && set as confirmation Picture
        images_ref.GetBytesAsync(256000).ContinueWith((Task<byte[]> task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
            }
            else
            {
                byte[] fileContents = task.Result;
                Debug.Log("Finished downloading!");

                Texture2D temp = new Texture2D(480, 287);
                ImageConversion.LoadImage(temp, fileContents);
                //confirmationPic.texture = temp;
            }
        });
    }

    public void hideMarkers() {
        for(int i = 0; i < markers.Length; i ++) {
            markers[i].OnClick += OnMarkerClick;
            markers[i].enabled = false;
        }
    }

    public void ShowMarker(string user, string buildingNumber, List<string> orderForPerson, string price, string roomNumber)
    {
        string concat = "";
        foreach(string s in orderForPerson)
        {
            concat += s + "\n";
        }
        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i].label == buildingNumber)
            {
                markers[i].enabled = true;
                markers[i].polyEatsInfoBase(user, buildingNumber, concat, price, roomNumber);
            }
        }
    }

    public void Awake()
    {
        _instance = this;

        if (target == OnlineMapsTarget.texture)
        {
            width = texture.width;
            height = texture.height;
        }
        else
        {
            width = tilesetWidth;
            height = tilesetHeight;
            texture = null;
        }

        control = GetComponent<OnlineMapsControlBase>();
        if (control == null) Debug.LogError("Can not find a Control.");
        else control.OnAwakeBefore();

        if (target == OnlineMapsTarget.texture)
        {
            if (texture != null) defaultColors = texture.GetPixels();

            if (defaultTileTexture == null)
            {
                OnlineMapsTile.defaultColors = new Color32[OnlineMapsUtils.sqrTileSize];
                for (int i = 0; i < OnlineMapsUtils.sqrTileSize; i++) OnlineMapsTile.defaultColors[i] = emptyColor;
            }
            else OnlineMapsTile.defaultColors = defaultTileTexture.GetPixels32();
        }

        foreach (OnlineMapsMarker marker in markers) marker.Init();

        if (target == OnlineMapsTarget.texture && useSmartTexture && smartTexture == null)
        {
            smartTexture = new Texture2D(texture.width / 2, texture.height / 2, TextureFormat.RGB24, false);
            smartTexture.wrapMode = TextureWrapMode.Clamp;
        }
    }

    private void CheckBaseProps()
    {
        if (mapType != _mapType || _language != language || _labels != labels)
        {
            _labels = labels;
            _language = language;

            activeType = OnlineMapsProvider.FindMapType(mapType);
            _mapType = mapType = activeType.fullID;

            if (_buffer != null)
            {
                _buffer.Dispose();
                _buffer = null;
#if NETFX_CORE
                if (renderThread != null) renderThread.Dispose();
#endif
#if !UNITY_WEBGL
                renderThread = null;
#endif
            }
            
            Redraw();
        }
        if (traffic != _traffic || trafficProviderID != _trafficProviderID)
        {
            _traffic = traffic;

            _trafficProviderID = trafficProviderID;
            trafficProvider = OnlineMapsTrafficProvider.GetByID(trafficProviderID);

            OnlineMapsTile[] tiles;
            lock (OnlineMapsTile.tiles)
            {
                tiles = OnlineMapsTile.tiles.ToArray();
            }
            if (traffic)
            {
                foreach (OnlineMapsTile tile in tiles)
                {
                    tile.trafficProvider = trafficProvider;
                    tile.trafficWWW = OnlineMapsUtils.GetWWW(tile.trafficURL);
                    tile.trafficWWW.customData = tile;
                    tile.trafficWWW.OnComplete += OnTrafficWWWComplete;
                    if (tile.trafficTexture != null)
                    {
                        OnlineMapsUtils.DestroyImmediate(tile.trafficTexture);
                        tile.trafficTexture = null;
                    }
                }
            }
            else
            {
                foreach (OnlineMapsTile tile in tiles)
                {
                    if (tile.trafficTexture != null)
                    {
                        OnlineMapsUtils.DestroyImmediate(tile.trafficTexture);
                        tile.trafficTexture = null;
                    }
                    tile.trafficWWW = null;
                }
            }
            Redraw();
        }
    }

    private void CheckBufferComplete()
    {
        if (buffer.status != OnlineMapsBufferStatus.complete) return;

        OnlineMapsTile.UnloadUnusedTiles();

        if (allowRedraw)
        {
            if (target == OnlineMapsTarget.texture)
            {
                if (!useSmartTexture || !buffer.generateSmartBuffer)
                {
                    texture.SetPixels32(buffer.frontBuffer);
                    texture.Apply(false);
                    if (control.activeTexture != texture) control.SetTexture(texture);
                }
                else
                {
                    smartTexture.SetPixels32(buffer.smartBuffer);
                    smartTexture.Apply(false);
                    if (control.activeTexture != smartTexture) control.SetTexture(smartTexture);

                    if (!isUserControl) needRedraw = true;
                }
            }

            if (OnPreloadTiles != null) OnPreloadTiles();
            if (control is OnlineMapsControlBase3D) (control as OnlineMapsControlBase3D).UpdateControl();

            if (OnMapUpdated != null) OnMapUpdated();
        }

        buffer.status = OnlineMapsBufferStatus.wait;
    }

    private int CheckMapSize(int z)
    {
        try
        {
            int maxX = (1 << z) * OnlineMapsUtils.tileSize;
            int maxY = (1 << z) * OnlineMapsUtils.tileSize;
            int w = target == OnlineMapsTarget.texture ? texture.width : tilesetWidth;
            int h = target == OnlineMapsTarget.texture ? texture.height : tilesetHeight;
            if (maxX < w || maxY < h) return CheckMapSize(z + 1);
        }
        catch{}
        
        return z;
    }

    /// <summary>
    /// Sets the desired type of redrawing the map.
    /// </summary>
    public void CheckRedrawType()
    {
        if (allowRedraw)
        {
            redrawType = OnlineMapsRedrawType.full;
            needRedraw = true;
        }
    }

#if UNITY_EDITOR
    private void CheckScriptCompiling() 
    {
        if (!EditorApplication.isPlaying) EditorApplication.update -= CheckScriptCompiling;

        if (stopPlayingWhenScriptsCompile && EditorApplication.isPlaying && EditorApplication.isCompiling)
        {
            Debug.Log("Online Maps stop playing to compile scripts.");
            EditorApplication.isPlaying = false;
        }
    }
#endif

    /// <summary>
    /// Allows you to test the connection to the Internet.
    /// </summary>
    /// <param name="callback">Function, which will return the availability of the Internet.</param>
    public void CheckServerConnection(Action<bool> callback)
    {
        OnlineMapsTile tempTile = new OnlineMapsTile(350, 819, 11, this, false);
        string url = tempTile.url;
        tempTile.Dispose();

        OnlineMapsWWW checkConnectionWWW = OnlineMapsUtils.GetWWW(url);
        checkConnectionWWW.OnComplete += www =>
        {
            callback(string.IsNullOrEmpty(www.error));
        };
    }

    /// <summary>
    /// Dispatch map events.
    /// </summary>
    /// <param name="evs">Events you want to dispatch.</param>
    public void DispatchEvent(params OnlineMapsEvents[] evs)
    {
        if (!dispatchEvents) return;

        foreach (OnlineMapsEvents ev in evs)
        {
            if (ev == OnlineMapsEvents.changedPosition && OnChangePosition != null) OnChangePosition();
            else if (ev == OnlineMapsEvents.changedZoom && OnChangeZoom != null) OnChangeZoom();
        }
    }

    /// <summary>
    /// Gets the name of the map types available for the provider.
    /// </summary>
    /// <param name="provider">Provider</param>
    /// <returns>Array of names.</returns>
    public static string[] GetAvailableTypes(OnlineMapsProviderEnum provider)
    {
        string[] types = {"Satellite", "Relief", "Terrain", "Map"};
        if (provider == OnlineMapsProviderEnum.aMap) return new[] {types[0], types[2]};
        if (provider == OnlineMapsProviderEnum.arcGis) return new[] {types[0], types[2]};
        if (provider == OnlineMapsProviderEnum.custom) return null;
        if (provider == OnlineMapsProviderEnum.google) return new[] {types[0], types[1], types[2]};
        if (provider == OnlineMapsProviderEnum.mapQuest) return new[] {types[0], types[2]};
        if (provider == OnlineMapsProviderEnum.nokia) return new[] {types[0], types[2], types[3]};
        if (provider == OnlineMapsProviderEnum.openStreetMap) return null;
        if (provider == OnlineMapsProviderEnum.sputnik) return null;
        if (provider == OnlineMapsProviderEnum.virtualEarth) return new[] {types[0], types[2]};
        return types;
    }

    /// <summary>
    /// Get the bottom-right corner of the map.
    /// </summary>
    /// <param name="lng">Longitude</param>
    /// <param name="lat">Latitude</param>
    public void GetBottomRightPosition(out double lng, out double lat)
    {
        if (Math.Abs(bottomRightLatitude) < double.Epsilon && Math.Abs(bottomRightLongitude) < double.Epsilon) UpdateBottonRightPosition();
        lng = bottomRightLongitude;
        lat = bottomRightLatitude;
    }

    /// <summary>
    /// Returns the coordinates of the corners of the map
    /// </summary>
    /// <param name="tlx">Longitude of the left border</param>
    /// <param name="tly">Latitude of the top border</param>
    /// <param name="brx">Longitude of the right border</param>
    /// <param name="bry">Latitude of the bottom border</param>
    public void GetCorners(out double tlx, out double tly, out double brx, out double bry)
    {
        if (Math.Abs(bottomRightLatitude) < double.Epsilon && Math.Abs(bottomRightLongitude) < double.Epsilon) UpdateBottonRightPosition();
        if (Math.Abs(topLeftLatitude) < double.Epsilon && Math.Abs(topLeftLongitude) < double.Epsilon) UpdateTopLeftPosition();

        brx = bottomRightLongitude;
        bry = bottomRightLatitude;
        tlx = topLeftLongitude;
        tly = topLeftLatitude;
    }

    /// <summary>
    /// Gets drawing element from screen.
    /// </summary>
    /// <param name="screenPosition">Screen position.</param>
    /// <returns>Drawing element</returns>
    public OnlineMapsDrawingElement GetDrawingElement(Vector2 screenPosition)
    {
        return drawingElements.LastOrDefault(el => el.HitTest(OnlineMapsControlBase.instance.GetCoords(screenPosition), zoom));
    }

    /// <summary>
    /// Gets 2D marker from screen.
    /// </summary>
    /// <param name="screenPosition">
    /// Screen position.
    /// </param>
    /// <returns>
    /// The 2D marker.
    /// </returns>
    public OnlineMapsMarker GetMarkerFromScreen(Vector2 screenPosition)
    {
        if (target == OnlineMapsTarget.tileset) return OnlineMapsTileSetControl.instance.GetMarkerFromScreen(screenPosition);

        Vector2 coords = OnlineMapsControlBase.instance.GetCoords(screenPosition);
        if (coords == Vector2.zero) return null;

        OnlineMapsMarker marker = null;
        double lng = double.MinValue, lat = double.MaxValue;
        double mx, my;

        foreach (OnlineMapsMarker m in markers)
        {
            if (!m.enabled || !m.range.InRange(zoom)) continue;
            if (m.HitTest(coords, zoom))
            {
                m.GetPosition(out mx, out my);
                if (my < lat || (Math.Abs(my - lat) < double.Epsilon && mx > lng))
                {
                    marker = m;
                    lat = my;
                    lng = mx;
                }
            }
        }

        return marker;
    }

    /// <summary>
    /// Get the map coordinate.
    /// </summary>
    /// <param name="lng">Longitude</param>
    /// <param name="lat">Latitude</param>
    public void GetPosition(out double lng, out double lat)
    {
        lat = latitude;
        lng = longitude;
    }

    /// <summary>
    /// Get the tile coordinates of the corners of the map
    /// </summary>
    /// <param name="tlx">Left tile X</param>
    /// <param name="tly">Top tile Y</param>
    /// <param name="brx">Right tile X</param>
    /// <param name="bry">Bottom tile Y</param>
    public void GetTileCorners(out double tlx, out double tly, out double brx, out double bry)
    {
        if (Math.Abs(bottomRightLatitude) < double.Epsilon && Math.Abs(bottomRightLongitude) < double.Epsilon) UpdateBottonRightPosition();
        if (Math.Abs(topLeftLatitude) < double.Epsilon && Math.Abs(topLeftLongitude) < double.Epsilon) UpdateTopLeftPosition();

        projection.CoordinatesToTile(topLeftLongitude, topLeftLatitude, _zoom, out tlx, out tly);
        projection.CoordinatesToTile(bottomRightLongitude, bottomRightLatitude, _zoom, out brx, out bry);
    }

    /// <summary>
    /// Get the tile coordinates of the corners of the map
    /// </summary>
    /// <param name="tlx">Left tile X</param>
    /// <param name="tly">Top tile Y</param>
    /// <param name="brx">Right tile X</param>
    /// <param name="bry">Bottom tile Y</param>
    /// <param name="zoom">Zoom</param>
    public void GetTileCorners(out double tlx, out double tly, out double brx, out double bry, int zoom)
    {
        if (Math.Abs(bottomRightLatitude) < double.Epsilon && Math.Abs(bottomRightLongitude) < double.Epsilon) UpdateBottonRightPosition();
        if (Math.Abs(topLeftLatitude) < double.Epsilon && Math.Abs(topLeftLongitude) < double.Epsilon) UpdateTopLeftPosition();

        projection.CoordinatesToTile(topLeftLongitude, topLeftLatitude, zoom, out tlx, out tly);
        projection.CoordinatesToTile(bottomRightLongitude, bottomRightLatitude, zoom, out brx, out bry);
    }

    /// <summary>
    /// Get the tile coordinates of the map
    /// </summary>
    /// <param name="px">Tile X</param>
    /// <param name="py">Tile Y</param>
    public void GetTilePosition(out double px, out double py)
    {
        projection.CoordinatesToTile(longitude, latitude, _zoom, out px, out py);
    }

    /// <summary>
    /// Get the tile coordinates of the map
    /// </summary>
    /// <param name="px">Tile X</param>
    /// <param name="py">Tile Y</param>
    /// <param name="zoom">Zoom</param>
    public void GetTilePosition(out double px, out double py, int zoom)
    {
        projection.CoordinatesToTile(longitude, latitude, zoom, out px, out py);
    }

    /// <summary>
    /// Get the top-left corner of the map.
    /// </summary>
    /// <param name="lng">Longitude</param>
    /// <param name="lat">Latitude</param>
    public void GetTopLeftPosition(out double lng, out double lat)
    {
        if (Math.Abs(topLeftLatitude) < double.Epsilon && Math.Abs(topLeftLongitude) < double.Epsilon) UpdateTopLeftPosition();
        lng = topLeftLongitude;
        lat = topLeftLatitude;
    }

    private void LateUpdate()
    {
        if (OnLateUpdateBefore != null) OnLateUpdateBefore();

        if (control == null || lockRedraw) return;
        StartBuffer();
        CheckBufferComplete();

        if (OnLateUpdateAfter != null) OnLateUpdateAfter();
    }

    public void OnAfterDeserialize()
    {
        try
        {
            activeType.LoadSettings(_activeTypeSettings);
        }
        catch (Exception exception)
        {
            Debug.LogWarning(exception.Message + "\n" + exception.StackTrace);
            //throw;
        }
    }

    public void OnBeforeSerialize()
    {
        _activeTypeSettings = activeType.GetSettings();
    }

    private void OnDestroy()
    {
        OnlineMapsThreadManager.Dispose();

        if (_buffer != null)
        {
            _buffer.Dispose();
            _buffer = null;
        }
#if NETFX_CORE
        if (renderThread != null) renderThread.Dispose();
#endif
#if !UNITY_WEBGL
        renderThread = null;
#endif

        if (defaultColors != null && texture != null)
        {
            texture.SetPixels(defaultColors);
            texture.Apply();
        }

        drawingElements = null;
        markers = null;
    }

    private void OnDisable ()
    {
        OnlineMapsThreadManager.Dispose();

        if (_buffer != null)
        {
            _buffer.Dispose();
            _buffer = null;
        }

#if NETFX_CORE
        if (renderThread != null) renderThread.Dispose();
#endif
#if !UNITY_WEBGL
        renderThread = null;
#endif

        OnChangePosition = null;
        OnChangeZoom = null;
        OnMapUpdated = null;
        OnMapUpdated = null;
        OnUpdateBefore = null;
        OnUpdateLate = null;
        OnlineMapsTile.OnGetResourcesPath = null;
        OnlineMapsTile.OnTileDownloaded = null;
        OnlineMapsTile.OnTrafficDownloaded = null;
        OnlineMapsMarkerBase.OnMarkerDrawTooltip = null;

        if (_instance == this) _instance = null;
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.update += CheckScriptCompiling;
#endif

        _instance = this;

        if (drawingElements == null) drawingElements = new List<OnlineMapsDrawingElement>();

#pragma warning disable 612, 618
        if (string.IsNullOrEmpty(mapType)) mapType = OnlineMapsProvider.Upgrade((int) provider, type);
#pragma warning restore 612, 618

        activeType = OnlineMapsProvider.FindMapType(mapType);
        _mapType = mapType = activeType.fullID;

        trafficProvider = OnlineMapsTrafficProvider.GetByID(trafficProviderID);

        if (language == "") language = activeType.provider.twoLetterLanguage ? "en" : "eng";

        _language = language;
        _labels = labels;
        _traffic = traffic;
        _trafficProviderID = trafficProviderID;

        UpdateTopLeftPosition();
        UpdateBottonRightPosition();

        tooltipStyle = new GUIStyle
        {
            normal =
            {
                background = tooltipBackgroundTexture,
                textColor = new Color32(230, 230, 230, 255)
            },
            border = new RectOffset(8, 8, 8, 8),
            margin = new RectOffset(4, 4, 4, 4),
            wordWrap = true,
            richText = true,
            alignment = TextAnchor.MiddleCenter,
            stretchWidth = true,
            padding = new RectOffset(0, 0, 3, 3)
        };
    }

    private void OnGUI()
    {
        if (string.IsNullOrEmpty(tooltip) && showMarkerTooltip != OnlineMapsShowMarkerTooltip.always) return;

        GUIStyle style = new GUIStyle(tooltipStyle);
			
        if (OnPrepareTooltipStyle != null) OnPrepareTooltipStyle(ref style);

        if (!string.IsNullOrEmpty(tooltip))
        {
            Vector2 inputPosition = control.GetInputPosition();

            if (tooltipMarker != null)
            {
                if (tooltipMarker.OnDrawTooltip != null) tooltipMarker.OnDrawTooltip(tooltipMarker);
                else if (OnlineMapsMarkerBase.OnMarkerDrawTooltip != null) OnlineMapsMarkerBase.OnMarkerDrawTooltip(tooltipMarker);
                else OnGUITooltip(style, tooltip, inputPosition);
            }
            else if (tooltipDrawingElement != null)
            {
                if (tooltipDrawingElement.OnDrawTooltip != null) tooltipDrawingElement.OnDrawTooltip(tooltipDrawingElement);
                else if (OnlineMapsDrawingElement.OnElementDrawTooltip != null) OnlineMapsDrawingElement.OnElementDrawTooltip(tooltipDrawingElement);
                else OnGUITooltip(style, tooltip, inputPosition);
            }
        }

        if (showMarkerTooltip == OnlineMapsShowMarkerTooltip.always)
        {
            if (OnlineMapsControlBase.instance is OnlineMapsTileSetControl)
            {
                OnlineMapsTileSetControl tsControl = OnlineMapsTileSetControl.instance;

                double tlx = topLeftLongitude;
                double tly = topLeftLatitude;
                double brx = bottomRightLongitude;
                double bry = bottomRightLatitude;
                if (brx < tlx) brx += 360;

                foreach (OnlineMapsMarker marker in markers)
                {
                    if (string.IsNullOrEmpty(marker.label)) continue;

                    double mx, my;
                    marker.GetPosition(out mx, out my);

                    if (!(((mx > tlx && mx < brx) || (mx + 360 > tlx && mx + 360 < brx) || (mx - 360 > tlx && mx - 360 < brx)) && my < tly && my > bry)) continue;

                    if (marker.OnDrawTooltip != null) marker.OnDrawTooltip(marker);
                    else if (OnlineMapsMarkerBase.OnMarkerDrawTooltip != null) OnlineMapsMarkerBase.OnMarkerDrawTooltip(marker);
                    else
                    {
                        Vector3 p1 = tsControl.GetWorldPositionWithElevation(mx, my, tlx, tly, brx, bry);
                        Vector3 p2 = p1 + new Vector3(0, 0, tilesetSize.y / tilesetHeight * marker.height * marker.scale);

                        Vector2 screenPoint1 = tsControl.activeCamera.WorldToScreenPoint(p1);
                        Vector2 screenPoint2 = tsControl.activeCamera.WorldToScreenPoint(p2);

                        float yOffset = (screenPoint1.y - screenPoint2.y) * transform.localScale.x - 10;

                        OnGUITooltip(style, marker.label, screenPoint1 + new Vector2(0, yOffset));
                    }
                }

                foreach (OnlineMapsMarker3D marker in tsControl.markers3D)
                {
                    if (string.IsNullOrEmpty(marker.label)) continue;

                    double mx, my;
                    marker.GetPosition(out mx, out my);

                    if (!(((mx > tlx && mx < brx) || (mx + 360 > tlx && mx + 360 < brx) ||
                       (mx - 360 > tlx && mx - 360 < brx)) &&
                      my < tly && my > bry)) continue;

                    if (marker.OnDrawTooltip != null) marker.OnDrawTooltip(marker);
                    else if (OnlineMapsMarkerBase.OnMarkerDrawTooltip != null) OnlineMapsMarkerBase.OnMarkerDrawTooltip(marker);
                    else
                    {
                        Vector3 p1 = tsControl.GetWorldPositionWithElevation(mx, my, tlx, tly, brx, bry);
                        Vector3 p2 = p1 + new Vector3(0, 0, tilesetSize.y / tilesetHeight * marker.scale);

                        Vector2 screenPoint1 = tsControl.activeCamera.WorldToScreenPoint(p1);
                        Vector2 screenPoint2 = tsControl.activeCamera.WorldToScreenPoint(p2);

                        float yOffset = (screenPoint1.y - screenPoint2.y) * transform.localScale.x - 10;

                        OnGUITooltip(style, marker.label, screenPoint1 + new Vector2(0, yOffset));
                    }
                }
            }
            else
            {
                foreach (OnlineMapsMarker marker in markers)
                {
                    if (string.IsNullOrEmpty(marker.label)) continue;

                    Rect rect = marker.screenRect;

                    if (rect.xMax > 0 && rect.xMin < Screen.width && rect.yMax > 0 && rect.yMin < Screen.height)
                    {
                        if (marker.OnDrawTooltip != null) marker.OnDrawTooltip(marker);
                        else if (OnlineMapsMarkerBase.OnMarkerDrawTooltip != null) OnlineMapsMarkerBase.OnMarkerDrawTooltip(marker);
                        else OnGUITooltip(style, marker.label, new Vector2(rect.x + rect.width / 2, rect.y + rect.height));
                    }
                }

                if (control is OnlineMapsControlBase3D)
                {
                    double tlx = topLeftLongitude;
                    double tly = topLeftLatitude;
                    double brx = bottomRightLongitude;
                    double bry = bottomRightLatitude;
                    if (brx < tlx) brx += 360;

                    foreach (OnlineMapsMarker3D marker in OnlineMapsControlBase3D.instance.markers3D)
                    {
                        if (string.IsNullOrEmpty(marker.label)) continue;

                        double mx, my;
                        marker.GetPosition(out mx, out my);

                        if (!(((mx > tlx && mx < brx) || (mx + 360 > tlx && mx + 360 < brx) ||
                           (mx - 360 > tlx && mx - 360 < brx)) &&
                          my < tly && my > bry)) continue;

                        if (marker.OnDrawTooltip != null) marker.OnDrawTooltip(marker);
                        else if (OnlineMapsMarkerBase.OnMarkerDrawTooltip != null) OnlineMapsMarkerBase.OnMarkerDrawTooltip(marker);
                        else
                        {
                            double mx1, my1;
                            OnlineMapsControlBase3D.instance.GetPosition(mx, my, out mx1, out my1);

                            double px = (-mx1 / width + 0.5) * OnlineMapsControlBase3D.instance.cl.bounds.size.x;
                            double pz = (my1 / height - 0.5) * OnlineMapsControlBase3D.instance.cl.bounds.size.z;

                            Vector3 offset = transform.rotation * new Vector3((float)px, 0, (float)pz);
                            offset.Scale(transform.lossyScale);

                            Vector3 p1 = transform.position + offset;
                            Vector3 p2 = p1 + new Vector3(0, 0, OnlineMapsControlBase3D.instance.cl.bounds.size.z / height * marker.scale);

                            Vector2 screenPoint1 = OnlineMapsControlBase3D.instance.activeCamera.WorldToScreenPoint(p1);
                            Vector2 screenPoint2 = OnlineMapsControlBase3D.instance.activeCamera.WorldToScreenPoint(p2);

                            float yOffset = (screenPoint1.y - screenPoint2.y) * transform.localScale.x - 10;

                            OnGUITooltip(style, marker.label, screenPoint1 + new Vector2(0, yOffset));
                        }
                    }
                }
            }
        }
    }

    private void OnGUITooltip(GUIStyle style, string text, Vector2 position)
    {
        GUIContent tip = new GUIContent(text);
        //This is where you change label font size
        style.fontSize = 40;
        Vector2 size = style.CalcSize(tip);
        GUI.Label(new Rect(position.x - size.x / 2 - 5, Screen.height - position.y - size.y - 20, size.x + 10, size.y + 5), text, style);
    }

    private void OnPostRender()
    {
        Debug.Log("OnPostRender");
        
    }

    private void OnTileWWWComplete(OnlineMapsWWW www)
    {
        OnlineMapsTile tile = www.customData as OnlineMapsTile;
        
        if (tile == null) return;

        if (tile.status == OnlineMapsTileStatus.disposed)
        {
            tile.www = null;
            return;
        }

        if (string.IsNullOrEmpty(www.error))
        {
            if (target == OnlineMapsTarget.texture)
            {
                tile.OnDownloadComplete();
                if (tile.status != OnlineMapsTileStatus.error) buffer.ApplyTile(tile);
            }
            else
            {
                Texture2D tileTexture = new Texture2D(256, 256, TextureFormat.RGB24, false)
                {
                    wrapMode = TextureWrapMode.Clamp
                };

                if (useSoftwareJPEGDecoder) OnlineMapsTile.LoadTexture(tileTexture, www.bytes);
                else www.LoadImageIntoTexture(tileTexture);

                tileTexture.name = tile.zoom + "x" + tile.x + "x" + tile.y;

                tile.CheckTextureSize(tileTexture);

                if (tile.status != OnlineMapsTileStatus.error && tile.status != OnlineMapsTileStatus.disposed)
                {
                    tile.texture = tileTexture;
                    tile.status = OnlineMapsTileStatus.loaded;
                }
            }

            if (tile.status != OnlineMapsTileStatus.error && tile.status != OnlineMapsTileStatus.disposed)
            {
                if (OnlineMapsTile.OnTileDownloaded != null) OnlineMapsTile.OnTileDownloaded(tile);
            }

            CheckRedrawType();
        }
        else tile.OnDownloadError();

        tile.www = null;
    }

    public void OnTrafficWWWComplete(OnlineMapsWWW www)
    {
        OnlineMapsTile tile = www.customData as OnlineMapsTile;
        
        if (tile == null) return;

        if (tile.status == OnlineMapsTileStatus.disposed)
        {
            tile.trafficWWW = null;
            return;
        }

        if (string.IsNullOrEmpty(www.error))
        {
            if (target == OnlineMapsTarget.texture)
            {
                if (tile.OnLabelDownloadComplete()) buffer.ApplyTile(tile);
            }
            else
            {
                Texture2D trafficTexture = new Texture2D(256, 256, TextureFormat.RGB24, false)
                {
                    wrapMode = TextureWrapMode.Clamp
                };
                if (useSoftwareJPEGDecoder) OnlineMapsTile.LoadTexture(trafficTexture, www.bytes);
                else tile.trafficWWW.LoadImageIntoTexture(trafficTexture);
                tile.trafficTexture = trafficTexture;
            }

            if (OnlineMapsTile.OnTrafficDownloaded != null) OnlineMapsTile.OnTrafficDownloaded(tile);

            CheckRedrawType();
        }

        tile.trafficWWW = null;
    }

    /// <summary>
    /// Full redraw map.
    /// </summary>
    public void Redraw()
    {
        needRedraw = true;
        allowRedraw = true;
        redrawType = OnlineMapsRedrawType.full;
        buffer.updateBackBuffer = true;
    }

    /// <summary>
    /// Stops the current process map generation, clears all buffers and completely redraws the map.
    /// </summary>
    public void RedrawImmediately()
    {
        OnlineMapsThreadManager.Dispose();

        if (renderInThread)
        {
            if (_buffer != null)
            {
                _buffer.Dispose();
                _buffer = null;
            }

#if NETFX_CORE
            if (renderThread != null) renderThread.Dispose();
#endif
#if !UNITY_WEBGL
            renderThread = null;
#endif
        }
        else StartBuffer();

        Redraw();
    }

    /// <summary>
    /// Remove the all drawing elements from the map.
    /// </summary>
    public void RemoveAllDrawingElements()
    {
        foreach (OnlineMapsDrawingElement element in drawingElements)
        {
            element.OnRemoveFromMap();
            element.Dispose();
        }
        drawingElements.Clear();
        needRedraw = true;
    }

    /// <summary>
    /// Remove all 2D markers from map.
    /// </summary>
    public void RemoveAllMarkers()
    {
        foreach (OnlineMapsMarker marker in markers)
        {
            if (OnRemoveMarker != null && OnRemoveMarker(marker)) continue;
            marker.Dispose();
        }
        markers = new OnlineMapsMarker[0];
        Redraw();
    }

    /// <summary>
    /// Remove the specified drawing element from the map.
    /// </summary>
    /// <param name="element">Drawing element you want to remove.</param>
    /// <param name="disposeElement">Indicates that need to dispose drawingElement.</param>
    public void RemoveDrawingElement(OnlineMapsDrawingElement element, bool disposeElement = true)
    {
        element.OnRemoveFromMap();
        if (disposeElement) element.Dispose();
        drawingElements.Remove(element);
        needRedraw = true;
    }

    /// <summary>
    /// Remove drawing element from the map by index.
    /// </summary>
    /// <param name="elementIndex">Drawing element index.</param>
    public void RemoveDrawingElementAt(int elementIndex)
    {
        if (elementIndex < 0 || elementIndex >= markers.Length) return;

        OnlineMapsDrawingElement element = drawingElements[elementIndex];
        element.Dispose();

        element.OnRemoveFromMap();
        drawingElements.Remove(element);
        needRedraw = true;
    }

    /// <summary>
    /// Remove the specified 2D marker from the map.
    /// </summary>
    /// <param name="marker">2D marker you want to remove.</param>
    /// <param name="disposeMarker">Dispose marker.</param>
    public void RemoveMarker(OnlineMapsMarker marker, bool disposeMarker = true)
    {
        if (OnRemoveMarker != null && OnRemoveMarker(marker)) return;

        int index = -1;
        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i] == marker)
            {
                index = i;
                break;
            }
        }

        if (index == -1) return;
        for (int i = index; i < markers.Length - 1; i++) markers[i] = markers[i + 1];
        if (disposeMarker) marker.Dispose();
        Array.Resize(ref markers, markers.Length - 1);
        Redraw();
    }

    /// <summary>
    /// Remove 2D marker from the map by marker index.
    /// </summary>
    /// <param name="markerIndex">Marker index.</param>
    public void RemoveMarkerAt(int markerIndex)
    {
        if (OnRemoveMarkerAt != null && OnRemoveMarkerAt(markerIndex)) return;

        if (markerIndex < 0 || markerIndex >= markers.Length) return;

        OnlineMapsMarker marker = markers[markerIndex];
        marker.Dispose();
        for (int i = markerIndex; i < markers.Length - 1; i++) markers[i] = markers[i + 1];
        Array.Resize(ref markers, markers.Length - 1);        
        Redraw();
    }

    /// <summary>
    /// This method is for the editor. \n
    /// Please do not use it.
    /// </summary>
    public void Save()
    {
        if (target == OnlineMapsTarget.texture) defaultColors = texture.GetPixels();
        else Debug.LogWarning("OnlineMaps.Save() only works with texture maps.  Current map is: " + target);
    }

    /// <summary>
    /// This method is for the editor. \n
    /// Please do not use it.
    /// </summary>
    /// <param name="parent">Parent XML Element</param>
    public void SaveMarkers(OnlineMapsXML parent)
    {
        if (markers == null || markers.Length == 0) return;

        OnlineMapsXML element = parent.Create("Markers");
        foreach (OnlineMapsMarker marker in markers) marker.Save(element);
    }

    /// <summary>
    /// This method is for the editor. \n
    /// Please do not use it.
    /// </summary>
    /// <param name="parent">Parent XML Element</param>
    /// <returns></returns>
    public OnlineMapsXML SaveSettings(OnlineMapsXML parent)
    {
        if (OnSaveSettings != null) OnSaveSettings();

         OnlineMapsXML element = parent.Create("Settings");

        element.Create("Position", position);
        element.Create("Zoom", zoom);

        if (target == OnlineMapsTarget.texture) element.Create("Texture", texture);
        else
        {
            element.Create("TilesetWidth", tilesetWidth);
            element.Create("TilesetHeight", tilesetHeight);
            element.Create("TilesetSize", tilesetSize);
        }

        element.Create("Source", (int)source);
        element.Create("MapType", mapType);
        if (activeType.isCustom) element.Create("CustomProviderURL", customProviderURL);
        element.Create("Labels", labels);
        element.Create("Traffic", traffic);
        element.Create("RedrawOnPlay", redrawOnPlay);
        element.Create("UseSmartTexture", useSmartTexture);
        element.Create("EmptyColor", emptyColor);
        element.Create("DefaultTileTexture", defaultTileTexture);
        element.Create("TooltipTexture", tooltipBackgroundTexture);
        element.Create("DefaultMarkerTexture", defaultMarkerTexture);
        element.Create("DefaultMarkerAlign", (int)defaultMarkerAlign);
        element.Create("ShowMarkerTooltip", (int)showMarkerTooltip);
        element.Create("UseSoftwareJPEGDecoder", useSoftwareJPEGDecoder);

        return element;
    }

    /// <summary>
    /// Set the the map coordinate.
    /// </summary>
    /// <param name="lng">Longitude</param>
    /// <param name="lat">Latitude</param>
    public void SetPosition(double lng, double lat)
    {
        int countX = width / OnlineMapsUtils.tileSize;
        int countY = height / OnlineMapsUtils.tileSize;

        if (positionRange != null)
        {
            if (positionRange.type == OnlineMapsPositionRangeType.center)
            {
                positionRange.CheckAndFix(ref lng, ref lat);
            }
            else if (positionRange.type == OnlineMapsPositionRangeType.border)
            {
                double px, py;
                projection.CoordinatesToTile(lng, lat, _zoom, out px, out py);
                double ox = countX / 2d;
                double oy = countY / 2d;

                double tlx, tly, brx, bry;

                projection.TileToCoordinates(px - ox, py - oy, _zoom, out tlx, out tly);
                projection.TileToCoordinates(px + ox, py + oy, _zoom, out brx, out bry);

                bool tlxc = false;
                bool tlyc = false;
                bool brxc = false;
                bool bryc = false;

                if (tlx < positionRange.minLng)
                {
                    tlxc = true;
                    tlx = positionRange.minLng;
                }
                if (brx > positionRange.maxLng)
                {
                    brxc = true;
                    brx = positionRange.maxLng;
                }
                if (tly > positionRange.maxLat)
                {
                    tlyc = true;
                    tly = positionRange.maxLat;
                }
                if (bry < positionRange.minLat)
                {
                    bryc = true;
                    bry = positionRange.minLat;
                }

                if (tlxc || brxc || tlyc || bryc)
                {
                    double tx, ty, tmp;
                    projection.CoordinatesToTile(lng, lat, _zoom, out tx, out ty);
                    if (tlxc)
                    {
                        projection.CoordinatesToTile(tlx, tly, _zoom, out tx, out tmp);
                        tx += ox;
                    }
                    else if (brxc)
                    {
                        projection.CoordinatesToTile(brx, bry, _zoom, out tx, out tmp);
                        tx -= ox;
                    }

                    if (tlyc)
                    {
                        projection.CoordinatesToTile(tlx, tly, _zoom, out tmp, out ty);
                        ty += oy;
                    }
                    else if (bryc)
                    {
                        projection.CoordinatesToTile(brx, bry, _zoom, out tmp, out ty);
                        ty -= oy;
                    }

                    projection.TileToCoordinates(tx, ty, _zoom, out lng, out lat);
                }
            }
        }

        double tpx, tpy;
        projection.CoordinatesToTile(lng, lat, _zoom, out tpx, out tpy);

        float haftCountY = countY / 2f;
        int maxY = (2 << zoom) / 2;
        bool modified = false;
        if (tpy - haftCountY < 0)
        {
            tpy = haftCountY;
            modified = true;
        }
        else if (tpy + haftCountY >= maxY - 1)
        {
            tpy = maxY - haftCountY - 1;
            modified = true;
        }

        if (modified) projection.TileToCoordinates(tpx, tpy, _zoom, out lng, out lat);

        if (Math.Abs(latitude - lat) < double.Epsilon && Math.Abs(longitude - lng) < double.Epsilon) return;

        allowRedraw = true;
        needRedraw = true;
        if (redrawType == OnlineMapsRedrawType.none || redrawType == OnlineMapsRedrawType.move)
            redrawType = OnlineMapsRedrawType.move;
        else redrawType = OnlineMapsRedrawType.full;

        latitude = lat;
        longitude = lng;
        UpdateTopLeftPosition();
        UpdateBottonRightPosition();

        DispatchEvent(OnlineMapsEvents.changedPosition);
    }

    /// <summary>
    /// Sets the position and zoom.
    /// </summary>
    /// <param name="lng">Longitude</param>
    /// <param name="lat">Latitude</param>
    /// <param name="ZOOM">Zoom</param>
    public void SetPositionAndZoom(double lng, double lat, int ZOOM = 0)
    {
        if (ZOOM != 0) zoom = ZOOM;
        SetPosition(lng, lat);
    }

    /// <summary>
    /// Sets the texture, which will draw the map.
    /// Texture displaying on the source you need to change yourself.
    /// </summary>
    /// <param name="newTexture">Texture, where you want to draw the map.</param>
    public void SetTexture(Texture2D newTexture)
    {
        texture = newTexture;
        width = newTexture.width;
        height = newTexture.height;
        allowRedraw = true;
        needRedraw = true;
        redrawType = OnlineMapsRedrawType.full;
    }

    /// <summary>
    /// Checks if the marker in the specified screen coordinates, and shows him a tooltip.
    /// </summary>
    /// <param name="screenPosition">Screen coordinates</param>
    public void ShowMarkersTooltip(Vector2 screenPosition)
    {
        if (showMarkerTooltip != OnlineMapsShowMarkerTooltip.onPress)
        {
            tooltip = string.Empty;
            tooltipDrawingElement = null;
            tooltipMarker = null;
        }

        IOnlineMapsInteractiveElement el = control.GetInteractiveElement(screenPosition);
        OnlineMapsMarkerBase marker = el as OnlineMapsMarkerBase;

        if (showMarkerTooltip == OnlineMapsShowMarkerTooltip.onHover)
        {
            if (marker != null)
            {
                tooltip = marker.label;
                tooltipMarker = marker;
            }
            else
            {
                OnlineMapsDrawingElement drawingElement = GetDrawingElement(screenPosition);
                if (drawingElement != null)
                {
                    tooltip = drawingElement.tooltip;
                    tooltipDrawingElement = drawingElement;
                }
            }
        }

        if (rolledMarker != marker)
        {
            if (rolledMarker != null && rolledMarker.OnRollOut != null) rolledMarker.OnRollOut(rolledMarker);
            rolledMarker = marker;
            if (rolledMarker != null && rolledMarker.OnRollOver != null) rolledMarker.OnRollOver(rolledMarker);
        }
    }

    private void Start()
    {
        toggler = GameObject.FindGameObjectWithTag("toggler");
        deliveryButton = GameObject.FindGameObjectWithTag("DeliveryButton");
        toggler.GetComponent<ToggleMessanger>().TurnOffToggleButton();
        deliveryButton.SetActive(false);
        if (redrawOnPlay)
        {
            allowRedraw = true;
            redrawType = OnlineMapsRedrawType.full;
        }
        needRedraw = true;
        _zoom = CheckMapSize(_zoom);
    }

    private void StartDownloading()
    {
        long startTick = DateTime.Now.Ticks;

        int countDownload = 0;
        OnlineMapsTile[] downloadTiles;
        int c = 0;

        lock (OnlineMapsTile.tiles)
        {
            List<OnlineMapsTile> tiles = OnlineMapsTile.tiles;
            for (int i = 0; i < tiles.Count; i++)
            {
                OnlineMapsTile tile = tiles[i];
                if (tile.status == OnlineMapsTileStatus.loading && tile.www != null)
                {
                    countDownload++;
                    if (countDownload >= maxTileDownloads) return;
                }
            }

            int needDownload = maxTileDownloads - countDownload;
            downloadTiles = new OnlineMapsTile[needDownload];

            for (int i = 0; i < tiles.Count; i++)
            {
                OnlineMapsTile tile = tiles[i];
                if (tile.status != OnlineMapsTileStatus.none) continue;

                if (c == 0)
                {
                    downloadTiles[0] = tile;
                    c++;
                }
                else
                {
                    int index = c;
                    int index2 = index - 1;

                    while (index2 >= 0)
                    {
                        if (downloadTiles[index2].zoom <= tile.zoom) break;

                        index2--;
                        index--;
                    }

                    if (index < needDownload)
                    {
                        for (int j = needDownload - 1; j > index ; j--) downloadTiles[j] = downloadTiles[j - 1];
                        downloadTiles[index] = tile;
                        if (c < needDownload) c++;
                    }
                }
            }
        }

        for (int i = 0; i < c; i++)
        {
            if (DateTime.Now.Ticks - startTick > 20000) break;
            OnlineMapsTile tile = downloadTiles[i];

            countDownload++;
            if (countDownload > maxTileDownloads) break;

            if (OnStartDownloadTile != null) OnStartDownloadTile(tile);
            else StartDownloadTile(tile);
        }
    }

    /// <summary>
    /// Starts dowloading of specified tile.
    /// </summary>
    /// <param name="tile">Tile to be downloaded.</param>
    public void StartDownloadTile(OnlineMapsTile tile)
    {
        tile.status = OnlineMapsTileStatus.loading;
        StartCoroutine(StartDownloadTileAsync(tile));
    }

    private IEnumerator StartDownloadTileAsync(OnlineMapsTile tile)
    {
        bool loadOnline = true;

        if (source != OnlineMapsSource.Online)
        {
            ResourceRequest resourceRequest = Resources.LoadAsync(tile.resourcesPath);
            yield return resourceRequest;
            UnityEngine.Object tileTexture = resourceRequest.asset;

            if (tileTexture != null)
            {
                tileTexture = Instantiate(tileTexture);
                if (target == OnlineMapsTarget.texture)
                {
                    tile.ApplyTexture(tileTexture as Texture2D);
                    buffer.ApplyTile(tile);
                }
                else
                {
                    tile.texture = tileTexture as Texture2D;
                    tile.status = OnlineMapsTileStatus.loaded;
                }
                CheckRedrawType();
                loadOnline = false;
            }
            else if (source == OnlineMapsSource.Resources)
            {
                tile.status = OnlineMapsTileStatus.error;
                yield break;
            }
        }

        if (loadOnline)
        {
            if (tile.www != null)
            {
                Debug.Log("tile has www " + tile + "   " + tile.status);
                yield break;
            }

            tile.www = OnlineMapsUtils.GetWWW(tile.url);
            tile.www.customData = tile;
            tile.www.OnComplete += OnTileWWWComplete;
            tile.status = OnlineMapsTileStatus.loading;
        }

        if (traffic && !string.IsNullOrEmpty(tile.trafficURL))
        {
            tile.trafficWWW = OnlineMapsUtils.GetWWW(tile.trafficURL);
            tile.trafficWWW.customData = tile;
            tile.trafficWWW.OnComplete += OnTrafficWWWComplete;
        }
    }

    private void StartBuffer()
    {
        if (!allowRedraw || !needRedraw) return;
        if (buffer.status != OnlineMapsBufferStatus.wait) return;

        if (latitude < -90) latitude = -90;
        else if (latitude > 90) latitude = 90;
        while (longitude < -180 || longitude > 180)
        {
            if (longitude < -180) longitude += 360;
            else if (longitude > 180) longitude -= 360;
        }
        
        buffer.redrawType = redrawType;
        buffer.generateSmartBuffer = isUserControl;
        buffer.status = OnlineMapsBufferStatus.start;        

#if !UNITY_WEBGL
        if (renderInThread)
        {
            if (renderThread == null)
            {
#if NETFX_CORE
                renderThread = new OnlineMapsThreadWINRT(buffer.GenerateFrontBuffer);
#else
                renderThread = new Thread(buffer.GenerateFrontBuffer);
#endif
                renderThread.Start();
            }
        }
        else buffer.GenerateFrontBuffer();
#else
        buffer.GenerateFrontBuffer();
#endif

        redrawType = OnlineMapsRedrawType.none;
        needRedraw = false;
    }

    private void Update()
    {
        if (OnUpdateBefore != null) OnUpdateBefore();
        
        CheckBaseProps();
        StartDownloading();

        if (OnUpdateLate != null) OnUpdateLate();
    }

    public void UpdateBorders()
    {
        UpdateTopLeftPosition();
        UpdateBottonRightPosition();
    }

    private void UpdateBottonRightPosition()
    {
        int countX = width / OnlineMapsUtils.tileSize;
        int countY = height / OnlineMapsUtils.tileSize;

        double px, py;
        projection.CoordinatesToTile(longitude, latitude, _zoom, out px, out py);

        px += countX / 2.0;
        py += countY / 2.0;

        projection.TileToCoordinates(px, py, _zoom, out bottomRightLongitude, out bottomRightLatitude);
    }

    private void UpdateTopLeftPosition()
    {
        int countX = width / OnlineMapsUtils.tileSize;
        int countY = height / OnlineMapsUtils.tileSize;

        double px, py;

        projection.CoordinatesToTile(longitude, latitude, _zoom, out px, out py);

        px -= countX / 2.0;
        py -= countY / 2.0;

        projection.TileToCoordinates(px, py, _zoom, out topLeftLongitude, out topLeftLatitude);
    }

#endregion
}