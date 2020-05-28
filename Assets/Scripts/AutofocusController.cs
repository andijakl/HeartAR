using GoogleARCore;
using UnityEngine;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class AutofocusController : MonoBehaviour
{
    /// <summary>
    /// The ARCoreSession monobehavior that manages the ARCore session.
    /// </summary>
    public ARCoreSession ARSessionManager;
    
    void Update()
    {
        // Use a two-finger tap to toggle autofocus mode
        if (Input.touchCount < 2 || Input.GetTouch(1).phase != TouchPhase.Began)
        {
            return;
        }

        // Toggle auto-focus mode
        var config = ARSessionManager.SessionConfig;
        if (config != null)
        {
            config.CameraFocusMode = config.CameraFocusMode == CameraFocusMode.FixedFocus ? CameraFocusMode.AutoFocus : CameraFocusMode.FixedFocus;
            Debug.Log("Autofocus: " + config.CameraFocusMode);
        }
    }
}
