using System.Collections.Generic;
using System.Linq;
using GoogleARCore;
using UnityEngine;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

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
    private GameObject[] InstantiatedPrefabs;
    
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
        InstantiatedPrefabs = new GameObject[ArPrefabs.Length];
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
        foreach (var image in m_TempAugmentedImages)
        {
            //Debug.Log("Found " + m_TempAugmentedImages.Count + " augmented images, length: " + 
            //          m_InstantiatedPrefabs.Length + ", db index: " + image.DatabaseIndex);
            // Sanity check to prevent out of bound index if a database index was returned
            // where we don't have a prefab for.
            if (InstantiatedPrefabs.Length <= image.DatabaseIndex) continue;
            
            if (image.TrackingState == TrackingState.Tracking
                && InstantiatedPrefabs[image.DatabaseIndex] == null)
            {
                // Prefab has not yet been instantiated in the scene
                // -> Create a new anchor
                var anchor = image.CreateAnchor(image.CenterPose);
                // Instantiate the prefab corresponding to the augmented image.
                var newArPrefab = Instantiate(ArPrefabs[image.DatabaseIndex], anchor.transform);
                // Give our prefab instance the reference to its trackable so that it can adjust itself if needed.
                newArPrefab.AttachedToTrackable = image;
                // Make object child of the anchor
                // Not done so in the AugmentedImages example, but in all others
                // -> seems logical to do the same here, as planes are also Trackables,
                // so they shouldn't be different.
                newArPrefab.transform.parent = anchor.transform;
                // Store the prefab instance at the position corresponding to the database index
                InstantiatedPrefabs[image.DatabaseIndex] = newArPrefab.gameObject;
                Debug.Log("Created new marker prefab instance, idx: " + image.DatabaseIndex);
            }
            else if (image.TrackingState == TrackingState.Stopped
                     && InstantiatedPrefabs[image.DatabaseIndex] != null)
            {
                // Stopped tracking a previously found marker.
                // Delete the anchor that was created in the Unity scene (its child is our own prefab instance)
                Destroy(InstantiatedPrefabs[image.DatabaseIndex].transform.parent.gameObject);
                // Now also delete the reference from our array so that we can create a new 
                // instance if the marker is discovered again
                InstantiatedPrefabs[image.DatabaseIndex] = null;
                Debug.Log("Destroyed marker prefab instance, idx: " + image.DatabaseIndex);
            }
            else if (image.TrackingState == TrackingState.Paused)
            {
                Debug.Log("Marker tracking paused, idx: " + image.DatabaseIndex);
            }

        }
    }


    public bool ArePrefabsInstantiated()
    {
        return InstantiatedPrefabs.Any(instantiatedPrefab => instantiatedPrefab != null);
    }
}
