using System.Collections.Generic;
using System.Linq;
using GoogleARCore;
using UnityEngine;

public class MarkerAnchorController : MonoBehaviour, IArObjectController
{
    /// <summary>
    /// List of prefabs, sorted by the database IDs of the augmented image database file.
    /// </summary>
    public ArPrefab[] ArPrefabs;

    /// <summary>
    /// Only allows 1 gameobject instance per augmented images database index.
    /// This array contains the instantiated gameobjects.
    /// Array ID corresponds to database index.
    /// Contains null if marker hasn't been found yet, or the gameobject instance
    /// after marker has been detected (and is still being tracked).
    /// </summary>
    private GameObject[] m_InstantiatedPrefabs;
    
    /// <summary>
    /// List filled by ARCore API containing all augmented images that have been updated.
    /// </summary>
    private readonly List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();


    /// <summary>
    /// The overlay containing the fit to scan user guide.
    /// </summary>
    //public GameObject FitToScanOverlay;

    void Start()
    {
        // Only one instance per marker
        m_InstantiatedPrefabs = new GameObject[ArPrefabs.Length];
    }

    // Update is called once per frame
    void Update()
    {
        // Check that motion tracking is tracking.
        if (Session.Status == SessionStatus.Tracking)
        {
            TrackAugmentedImages();
        }
    }


    private void TrackAugmentedImages()
    {
        // Get updated augmented images for this frame.
        Session.GetTrackables(m_TempAugmentedImages, TrackableQueryFilter.Updated);

        // Create visualizers and anchors for updated augmented images that are tracking and do not previously
        // have a visualizer. Remove visualizers for stopped images.
        foreach (var tempAugmentedImage in m_TempAugmentedImages)
        {
            // Sanity check to prevent out of bound index if a database index was returned
            // where we don't have a prefab for.
            if (m_InstantiatedPrefabs.Length <= tempAugmentedImage.DatabaseIndex) continue;


            if (tempAugmentedImage.TrackingState == TrackingState.Tracking
                && tempAugmentedImage.TrackingMethod == AugmentedImageTrackingMethod.FullTracking
                && m_InstantiatedPrefabs[tempAugmentedImage.DatabaseIndex] == null)
            {

                // Prefab has not yet been instantiated in the scene
                // -> Create a new anchor
                var anchor = tempAugmentedImage.CreateAnchor(tempAugmentedImage.CenterPose);
                // Instantiate the prefab corresponding to the augmented image.
                var newArPrefab = Instantiate(ArPrefabs[tempAugmentedImage.DatabaseIndex], anchor.transform);
                // Give our prefab instance the reference to its trackable so that it can adjust itself if needed.
                newArPrefab.AttachedToTrackable = tempAugmentedImage;
                // Make object child of the anchor
                // Not done so in the AugmentedImages example, but in all others
                // -> seems logical to do the same here, as planes are also Trackables,
                // so they shouldn't be different.
                newArPrefab.transform.parent = anchor.transform;
                // Store the prefab instance at the position corresponding to the database index
                m_InstantiatedPrefabs[tempAugmentedImage.DatabaseIndex] = newArPrefab.gameObject;
                Debug.Log("MarkerAnchorController: Created new marker prefab instance, idx: " +
                          tempAugmentedImage.DatabaseIndex);

            }
            else if ((tempAugmentedImage.TrackingState != TrackingState.Tracking ||
                      tempAugmentedImage.TrackingMethod != AugmentedImageTrackingMethod.FullTracking) &&
                     m_InstantiatedPrefabs[tempAugmentedImage.DatabaseIndex] != null)
            {
                DestroyPrefab(tempAugmentedImage.DatabaseIndex);
            }
        }
    }



    public bool ArePrefabsInstantiated()
    {
        return m_InstantiatedPrefabs.Any(instantiatedPrefab => instantiatedPrefab != null);
    }

    private void DestroyPrefab(int databaseIndex)
    {
        // Stopped tracking a previously found marker.
        // Delete the anchor that was created in the Unity scene (its child is our own prefab instance)
        Destroy(m_InstantiatedPrefabs[databaseIndex].transform.parent.gameObject);
        // Now also delete the reference from our array so that we can create a new 
        // instance if the marker is discovered again
        m_InstantiatedPrefabs[databaseIndex] = null;
        Debug.Log("MarkerAnchorController: Destroyed marker prefab instance, idx: " + databaseIndex);
    }
}
