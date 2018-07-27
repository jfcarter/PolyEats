using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class CloudDatabaseManager : MonoBehaviour, ICloudRecoEventHandler
{
    private CloudRecoBehaviour mCloudRecoBehaviour;
    private bool mIsScanning = false;
    private string mTargetMetadata = "";
    public ImageTargetBehaviour ImageTargetTemplate;

    // Use this for initialization
    void Start()
    {
        // register this event handler at the cloud reco behaviour
        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();

        if (mCloudRecoBehaviour)
        {
            mCloudRecoBehaviour.RegisterEventHandler(this);
        }
    }

    public void OnInitialized()
    {
        Debug.Log("Cloud Reco initialized");
    }
    public void OnInitError(TargetFinder.InitState initError)
    {
        Debug.Log("Cloud Reco init error " + initError.ToString());
    }
    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        Debug.Log("Cloud Reco update error " + updateError.ToString());
    }

    //lets you know if vuforia is scanning the cloud
    public void OnStateChanged(bool scanning)
    {
        mIsScanning = scanning;
        if (scanning)
        {
            // clear all known trackables
            var tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            tracker.TargetFinder.ClearTrackables(false);
        }
    }

    // Here we handle a cloud target recognition event (you found the image in the database)
    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        //Play cash sound
        GetComponent<AudioSource>().Play();
        // do something with the target metadata
        mTargetMetadata = targetSearchResult.MetaData;
        // stop the target finder (i.e. stop scanning the cloud)
        mCloudRecoBehaviour.CloudRecoEnabled = false;
        // Build augmentation based on target
        if (ImageTargetTemplate)
        {
            // enable the new result with the same ImageTargetBehaviour:
            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            ImageTargetBehaviour imageTargetBehaviour =
             (ImageTargetBehaviour)tracker.TargetFinder.EnableTracking(
             targetSearchResult, ImageTargetTemplate.gameObject);
        }
        //HANDLE PAYMENT HERE

        //Go back to home screen
        StartCoroutine(HomeScreen());
    }

    IEnumerator HomeScreen()
    {
        //give it some time so you can see horse
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene("HustlerOrHungry");
    }

    void OnGUI()
    {
        // Display current 'scanning' status
        GUI.Box(new Rect(100, 100, 200, 50), mIsScanning ? "Scanning" : "Not scanning");
        // Display metadata of latest detected cloud-target
        GUI.Box(new Rect(100, 200, 200, 50), "Metadata: " + mTargetMetadata);
        // If not scanning, show button
        // so that user can restart cloud scanning
        if (!mIsScanning)
        {
            if (GUI.Button(new Rect(100, 300, 200, 50), "Restart Scanning"))
            {
                // Restart TargetFinder
                mCloudRecoBehaviour.CloudRecoEnabled = true;
            }

        }
    }
}