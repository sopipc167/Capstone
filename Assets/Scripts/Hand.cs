using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using static Unity.VisualScripting.Member;

public class Hand : MonoBehaviour
{
    WebCamTexture _webCamTexture;
    OpenCvSharp.Rect MyStar;
    OpenCvSharp.Rect[] stars;
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        _webCamTexture = new WebCamTexture(devices[0].name);
        _webCamTexture.Play();
    }

    void Update()
    {
        //GetComponent<Renderer>().material.mainTexture = _webCamTexture;
        Mat frame = OpenCvSharp.Unity.TextureToMat(_webCamTexture);

        findNewStar(frame);
        display(frame);
    }
    void findNewStar(Mat frame)
    {

    }


    void display(Mat frame)
    {
        //if (MyStar != null && stars.Length >= 1)
        {
            //frame.Rectangle(MyStar, new Scalar(0, 255, 0), 2);
        }
        Mat thresh = new Mat();
        Mat grayMat = new Mat();
        Cv2.CvtColor(frame, grayMat, ColorConversionCodes.BGR2GRAY);
        //Cv2.Threshold(grayMat, thresh, 127, 255, ThresholdTypes.BinaryInv);
        Cv2.Canny(grayMat, thresh,127,255);
        Texture newtexture = OpenCvSharp.Unity.MatToTexture(thresh);
        GetComponent<Renderer>().material.mainTexture = newtexture;
    }
}