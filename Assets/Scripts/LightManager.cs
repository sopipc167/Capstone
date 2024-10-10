using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class LightManager : MonoBehaviour
{
    public ARCameraManager aRCameraManager;
    public Text textUI;

    // Start is called before the first frame update
    void Start()
    {
        aRCameraManager.frameReceived += FrameLightUpdated;
    }

    public void FrameLightUpdated(ARCameraFrameEventArgs args)
    {
        var brightness = args.lightEstimation.averageBrightness;

        if(brightness.HasValue)
        {
            textUI.text = brightness.Value.ToString();
        }
    }
}
