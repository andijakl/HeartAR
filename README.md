# HeartAR
ARCore sample project that visualizes the human heart on a marker and allows to freely place and transform nerve cells.

![](https://raw.githubusercontent.com/andijakl/HeartAR/master/HeartAR-Animated.gif)

## What is this about?

This project extends the samples that ship with the [Google ARCore for Unity SDK](https://github.com/google-ar/arcore-unity-sdk/releases). It combines multiple scenarios:

* **Augmented Images (marker tracking):** allows a 1:1 relationship between marker and associated 3D model / prefab to create. Only instantiates every object once in the world.\
*See script:* `MarkerAnchorController.cs`\
*Difference to Original Google example:* uses the same Prefab for every augmented image in the database. Sets up the correct object hierarchy in Unity (Anchor -> Prefab) and deletes the item as well as its anchor when tracking is lost.

* **Plane Anchoring:** Tap on a detected plane to position an item. Not activated by default in the scene.
*See script:* `PlaneAnchorController.cs`\
*Difference to Original Google example:* Checks if the plane has been lost and cleans up the anchor + insantiated prefab.

* **Plane Anchoring + Manipulation:** Tap on a detected plane to position an item, with the manipulation system in place. Allows moving, rotating and scaling objects that have been placed in the scene.\
*See script:* `ManipulatorController.cs`\
*Difference to Original Google example:* Checks if the plane has been lost and cleans up the anchor + insantiated prefab.

* **Lifecycle:** Takes care of permissions, screen time out and quitting the app.\
*See script:* `LifecycleController.cs`\
*Difference to Original Google example:* Completely integrated into the HelloAR example from the SDK. But these tasks are good to have in an extra script, instead of being integrated into a controller that handles user interaction.

* **Autofocus Controller:** Turn autofocus on / off while the app is running through a tap with two fingers on the screen. Google currently still recommends using the default setting - which is autofocus off. With this script, you can explore the differences. If the phone camera doesn't support autofocus, it will ignore this setting.\
*See script:* `AutofocusController.cs`\
*Difference to Original Google example:* Starting with the ARCore for Unity SDK 1.8, Google now enables Autofocus by default for augmented images.

### General architecture notes

* **Augmented Images:** The two pictures to use as markers are in the /Assets/Images/ folder. For the best performance, print them on paper. For quick testing, it also works to show the markers on your computer screen.

* **ArPrefab:** Should be part of every AR prefab instantiated in the scene. Stores a reference to the trackable the prefab instance is connected to as a property. Allows for quickly checking which trackables have been given up by the ARCore SDK, to properly clean up the game objects in the scene.

* **TrackingCheck:** Regularly checks instantiated prefabs whether the associated ARCore trackable reports that tracking has been lost. Automatically checks all controller scripts that implement the `IArObjectController` interface. This simple interface allows querying the instantiated prefabs.

* **Light Estimation:** Is enabled in this scene and currently used by the nerve cell prefab.

* **Screen Auto-Rotation:** Disabled, as this gives a better user experience on Android without screen flickering while turning the screen. Auto-rotation would mainly be needed for static textual content that is not anchored to a plane or augmented image or otherwise placed in screen-space instead of world-space.

* **GLB / GLTF Import:** The 3D models of the human heart and nerve cells were imported from GLB files. This format is not yet natively supported in Unity. This app therefore includes the [SketchFab unitypackage](https://github.com/sketchfab/UnityGLTF/releases/tag/1.0.3), which imported the models.

## Credits

The 3D models used in this scene have been created by Microsoft: [Beating heart](https://www.remix3d.com/details/059f2766c027458787256ebb47a4094e?section=other-models), [Human heart](https://www.remix3d.com/details/G009SW90XX63?section=other-models), [Nerve cell](https://www.remix3d.com/details/4792d1b6b6954e7882c429e8b4dcaa8c?section=other-models).

Released under the MIT license - see the LICENSE file for details.

Developed by Andreas Jakl
* https://www.andreasjakl.com/
* https://twitter.com/andijakl
