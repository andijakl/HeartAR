using System.Collections.Generic;
using System.Linq;
using GoogleARCore;
using GoogleARCore.Examples.ObjectManipulation;
using UnityEngine;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class ManipulatorController : Manipulator, IArObjectController
{
    /// <summary>
    /// The first-person camera being used to render the passthrough camera image (i.e. AR
    /// background).
    /// </summary>
    public Camera FirstPersonCamera;

    /// <summary>
    /// A model to place when a raycast from a user touch hits a plane.
    /// </summary>
    public ArPrefab ArPrefabTemplate;

    private readonly List<GameObject> m_ArPrefabInstances = new List<GameObject>();

    /// <summary>
    /// Manipulator prefab to attach placed objects to.
    /// </summary>
    public GameObject ManipulatorPrefab;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        CleanUntrackedObjects();
    }

    /// <summary>
    /// Returns true if the manipulation can be started for the given gesture.
    /// </summary>
    /// <param name="gesture">The current gesture.</param>
    /// <returns>True if the manipulation can be started.</returns>
    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        if (gesture.TargetObject == null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Function called when the manipulation is ended.
    /// </summary>
    /// <param name="gesture">The current gesture.</param>
    protected override void OnEndManipulation(TapGesture gesture)
    {
        if (gesture.WasCancelled)
        {
            return;
        }

        // If gesture is targeting an existing object we are done.
        if (gesture.TargetObject != null)
        {
            return;
        }

        // Raycast against the location the player touched to search for planes.
        var raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(
            gesture.StartPosition.x, gesture.StartPosition.y, raycastFilter, out var hit))
        {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
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

                // Instantiate manipulator.
                var manipulator = Instantiate(ManipulatorPrefab, hit.Pose.position, hit.Pose.rotation);

                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Make model a child of the manipulator.
                newArPrefab.transform.parent = manipulator.transform;
                // Make manipulator a child of the anchor.
                manipulator.transform.parent = anchor.transform;

                // Save reference of our prefab for clean-up
                m_ArPrefabInstances.Add(newArPrefab.gameObject);

                Debug.Log("Created prefab (with manipulator) based on plane anchor");
                
                // Select the placed object.
                manipulator.GetComponent<Manipulator>().Select();
            }
        }
    }

    private void CleanUntrackedObjects()
    {
        for (var i = m_ArPrefabInstances.Count - 1; i >= 0; i--)
        {
            // Check the trackable attached to each instance to see if its tracking state stopped
            if (m_ArPrefabInstances[i]?.GetComponent<ArPrefab>()?.AttachedToTrackable.TrackingState == TrackingState.Stopped)
            {
                Debug.Log("Deleting parent (should be anchor): " + m_ArPrefabInstances[i].transform.parent.parent.gameObject.name);
                // Destroy gameobject and remove its anchor from the Unity scene
                // Attention - for the manipulator, the anchor is the parent of the parent of our game object!
                // Hierarchy is: Anchor -> Manipulator -> Our GameObject
                Destroy(m_ArPrefabInstances[i].transform.parent.parent.gameObject);

                // Also remove the item from our internal list
                m_ArPrefabInstances.RemoveAt(i);

                Debug.Log("Lost tracking of plane trackable and deleted the attached item");
            }
        }
    }

    public bool ArePrefabsInstantiated()
    {
        return m_ArPrefabInstances.Any();
    }
}
