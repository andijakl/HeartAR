using System.Collections.Generic;
using System.Linq;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif


public class PlaneAnchorController : MonoBehaviour, IArObjectController
{
    /// <summary>
    /// The prefab to place when the user taps on the screen.
    /// </summary>
    public ArPrefab ArPrefabTemplate;

    private readonly List<GameObject> m_ArPrefabInstances = new List<GameObject>();

    /// <summary>
    /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
    /// </summary>
    public Camera FirstPersonCamera;
    
    // Update is called once per frame
    void Update()
    {
        TrackPlaneAnchors();
        CleanUntrackedObjects();
    }


    private void TrackPlaneAnchors()
    {
        // If the player has not touched the screen, we are done with this update.
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began) return;

        // Should not handle input if the player is pointing on UI.
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }

        // Raycast against the location the player touched to search for planes.
        const TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                                                TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out var hit))
        {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if (hit.Trackable is DetectedPlane &&
                Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                // Instantiate model at the hit pose.
                var newArPrefab = Instantiate(ArPrefabTemplate, hit.Pose.position, hit.Pose.rotation);
                newArPrefab.AttachedToTrackable = hit.Trackable;

                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Make model a child of the anchor.
                newArPrefab.transform.parent = anchor.transform;
                m_ArPrefabInstances.Add(newArPrefab.gameObject);

                Debug.Log("Created prefab based on plane or feature point anchor");
            }
        }
        else
        {
            Debug.Log("No raycast hit detected");
        }
    }


    private void CleanUntrackedObjects()
    {
        for (var i = m_ArPrefabInstances.Count - 1; i >= 0; i--)
        {
            // Check the trackable attached to each instance to see if its tracking state stopped
            if (m_ArPrefabInstances[i]?.GetComponent<ArPrefab>()?.AttachedToTrackable.TrackingState == TrackingState.Stopped)
            {
                // Destroy gameobject and remove its anchor from the Unity scene
                // See: https://stackoverflow.com/questions/51466946/destroy-anchors-in-unity-arcore
                Destroy(m_ArPrefabInstances[i].transform.parent.gameObject);

                // Also remove the item from our internal list
                m_ArPrefabInstances.RemoveAt(i);

                Debug.Log("Lost tracking of plane / feature point trackable and deleted the item");
            }
        }
    }

    public bool ArePrefabsInstantiated()
    {
        return m_ArPrefabInstances.Any();
    }
}
