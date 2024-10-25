using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using static Unity.VisualScripting.Member;

public class Hand : MonoBehaviour
{
    WebCamTexture _webCamTexture;
    CascadeClassifier cascade;
    OpenCvSharp.Rect MyStar;
    OpenCvSharp.Rect[] stars;
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        _webCamTexture = new WebCamTexture(devices[0].name);
        _webCamTexture.Play();
        cascade = new CascadeClassifier(Application.dataPath + "/OpenCV+Unity/Demo/Face_Detector/" + "haarcascade_frontalface_default.xml");
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
        stars = cascade.DetectMultiScale(frame, 1.1, 2, HaarDetectionType.ScaleImage);
        if (stars.Length >= 1)
        {
            Debug.Log(stars[0].Location);
            Debug.Log("detected face num : " + stars.Length);

            MyStar = stars[0];
        }
        else
        {
            Debug.Log("could not reconginze the face");
        }
    }


    void display(Mat frame)
    {
        
        //Mat thresh = new Mat();
        //Mat grayMat = new Mat();
        //Cv2.CvtColor(frame, grayMat, ColorConversionCodes.BGR2GRAY);
        FastFeatureDetector fast = FastFeatureDetector.Create();
        KeyPoint[] kps = fast.Detect(frame);
        Cv2.DrawKeypoints(frame, kps, frame);
        //Cv2.Threshold(grayMat, thresh, 127, 255, ThresholdTypes.BinaryInv);
        //Cv2.Canny(grayMat, thresh, 64,255);
        //if (MyStar != null && stars.Length >= 1)
        {
            //frame.Rectangle(MyStar, new Scalar(255, 255, 255), 2);
        }
        Texture newtexture = OpenCvSharp.Unity.MatToTexture(frame);
        GetComponent<Renderer>().material.mainTexture = newtexture;
    }
}