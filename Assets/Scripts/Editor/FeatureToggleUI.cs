using UnityEngine;

namespace PBRNightSky
{
    public class FeatureToggleUI : MonoBehaviour
    {
        /// <summary>
        /// Adds some buttons in the editor that when pressed, showcase different scenarios.
        /// </summary>
        PBRNightSkyController controller;

        public void Start()
        {
            controller.DateTime.UseRealTime = true;
            Input.location.Start();
            controller.Latitude = Input.location.lastData.latitude;
            controller.Longitude = Input.location.lastData.longitude;
        }

        public void Update()
        {
            
        }
    }
}
