using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Rendering.PostProcessing;

public class EclipseController : MonoBehaviour
{
    public ARCameraManager ARCameraManager;
    public PostProcessVolume postProcessVolume;

    private ColorGrading colorGrading;

    [SerializeField]
    private Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        postProcessVolume.profile.TryGetSettings(out colorGrading);

        ARCameraManager.frameReceived += FrameLightUpdated;

        toggle.onValueChanged.AddListener(onToggle);
    }

    public void onToggle(bool isOn)
    {
        if(!isOn)
        {
            colorGrading.postExposure.value = 0f;
            colorGrading.saturation.value = 0f;
            colorGrading.gamma.value = new Vector4(1f, 1f, 1f, 0);
        }
    }

    public void FrameLightUpdated(ARCameraFrameEventArgs args)
    {
        if(!toggle.isOn) return;

        var brightness = args.lightEstimation.averageBrightness;

        if(brightness.HasValue)
        {
            if(brightness.Value <= 0.35f)
            {
                colorGrading.postExposure.value = Mathf.Lerp(colorGrading.postExposure.value, 2.5f, 0.5f*Time.deltaTime);
                colorGrading.saturation.value = 25f;
                colorGrading.gamma.value = new Vector4(0.65f,0.65f,0.65f,0);
            } 
            else
            {
                colorGrading.postExposure.value = Mathf.Lerp(colorGrading.postExposure.value, -6f, 0.5f*Time.deltaTime);
                colorGrading.saturation.value = 0f;
                colorGrading.gamma.value = new Vector4(2.2f,2.2f,2.2f,0);
            }
        }
    }
}
