using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

public class BeatingHeartPrefab : MonoBehaviour
{
    /// <summary>
    /// The AugmentedImage to visualize.
    /// </summary>
    public AugmentedImage Image;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Image.CenterPose.position;
        //transform.rotation = Image.CenterPose.rotation;
        //transform.localScale = new Vector3(Image.ExtentX, Image.ExtentX, Image.ExtentX);
    }
}
