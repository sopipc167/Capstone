using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

public class CameraManagerScript : MonoBehaviour
{
    WebCamTexture camTexture;
    public RawImage cameraViewImage;

    public void CameraOn()
    {
        if(!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        if(WebCamTexture.devices.Length == 0)
        {
            Debug.Log("no camera");
            return;
        }

        WebCamDevice[] devices = WebCamTexture.devices;
        int selectedCameraIndex = -1;

        for(int i = 0; i< devices.Length; i++) {
            if(devices[i].isFrontFacing == false) {
                selectedCameraIndex = i;
                break;
            }
        }

        if(selectedCameraIndex >= 0) {
            camTexture = new WebCamTexture(devices[selectedCameraIndex].name);
            camTexture.requestedFPS = 30;
            cameraViewImage.texture = camTexture;
            camTexture.Play();
        }
    }
    public void CameraOff()
    {
        if(camTexture != null) {
            camTexture.Stop();
            WebCamTexture.Destroy(camTexture);
            camTexture = null;
        }
    }
}
