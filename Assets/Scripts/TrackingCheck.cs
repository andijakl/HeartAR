using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackingCheck : MonoBehaviour
{
    /// <summary>
    /// The overlay containing the fit to scan user guide.
    /// </summary>
    public GameObject FitToScanOverlay;

    private IEnumerable<IArObjectController> m_ArObjectControllers;

    void Start()
    {
        m_ArObjectControllers = FindObjectsOfType<MonoBehaviour>().OfType<IArObjectController>();
    }

    // Update is called once per frame
    void Update()
    {
        //foreach (var arObjectController in m_ArObjectControllers)
        //{
        //    Debug.Log("Instantiated: " + arObjectController.ArePrefabsInstantiated());
        //}
        var showOverlay = !m_ArObjectControllers.Any(arObjectController => arObjectController.ArePrefabsInstantiated());
        //Debug.Log("Show overlay: " + showOverlay);
        FitToScanOverlay.SetActive(showOverlay);
    }
    

    // Show the fit-to-scan overlay if there are no images that are Tracking.
    //FitToScanOverlay.SetActive(m_InstantiatedPrefabs.All(instantiatePrefab => 
    //    instantiatePrefab?.GetComponent<ArPrefab>()?.AttachedToTrackable.TrackingState != TrackingState.Tracking));
}
