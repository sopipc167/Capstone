using UnityEngine;

namespace PBRNightSky {
    /// <summary>
    /// A class to draw a debug UI for a demo scene.
    /// </summary>
    public class NightSkyGUI : MonoBehaviour {
        [SerializeField]
        private PBRNightSkyController controller;
        [SerializeField]
        private FeatureToggle featureToggle;

        /// <summary>
        /// Gets the PBR Night Sky Controller and the feature toggle components on awake.
        /// </summary>
        private void Awake() {
            controller = GetComponent<PBRNightSkyController>();
            featureToggle = GetComponent<FeatureToggle>();
        }

        /// <summary>
        /// Draws a custom debug UI for a demo version of the asset.
        /// </summary>
        private void OnGUI() {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            float guiWidth = 300f;
            float guiHeight = 220f;
            float padding = 10f;

            float xPos = screenWidth - guiWidth - padding;
            float yPos = screenHeight - guiHeight - padding;

            GUI.Label(new Rect(xPos + padding, yPos + padding, 150, 20), "Latitude:");
            controller.Latitude = float.Parse(GUI.TextField(new Rect(xPos + 160, yPos + padding, 100, 20), controller.Latitude.ToString()));

            GUI.Label(new Rect(xPos + padding, yPos + 30, 150, 20), "Longitude:");
            controller.Longitude = float.Parse(GUI.TextField(new Rect(xPos + 160, yPos + 30, 100, 20), controller.Longitude.ToString()));

            GUI.Label(new Rect(xPos + padding, yPos + 60, 50, 20), "Date:");
            controller.DateTime.Day = int.Parse(GUI.TextField(new Rect(xPos + 60, yPos + 60, 40, 20), controller.DateTime.Day.ToString()));
            controller.DateTime.Month = int.Parse(GUI.TextField(new Rect(xPos + 130, yPos + 60, 40, 20), controller.DateTime.Month.ToString()));
            controller.DateTime.Year = int.Parse(GUI.TextField(new Rect(xPos + 180, yPos + 60, 60, 20), controller.DateTime.Year.ToString()));

            GUI.Label(new Rect(xPos + padding, yPos + 90, 50, 20), "Time:");
            controller.DateTime.Hour = int.Parse(GUI.TextField(new Rect(xPos + 60, yPos + 90, 60, 20), controller.DateTime.Hour.ToString()));
            controller.DateTime.Minute = int.Parse(GUI.TextField(new Rect(xPos + 130, yPos + 90, 40, 20), controller.DateTime.Minute.ToString()));
            controller.DateTime.Second = int.Parse(GUI.TextField(new Rect(xPos + 180, yPos + 90, 40, 20), controller.DateTime.Second.ToString()));

            GUI.Label(new Rect(xPos + padding, yPos + 120, 100, 20), "Ticks:");
            controller.DateTime.Ticks = long.Parse(GUI.TextField(new Rect(xPos + 110, yPos + 120, 150, 20), controller.DateTime.Ticks.ToString()));

            featureToggle.Timelapse = GUI.Toggle(new Rect(xPos + padding, yPos + 150, 200, 20), featureToggle.Timelapse, "Enable Timelapse");
            featureToggle.ShowConstellations = GUI.Toggle(new Rect(xPos + padding, yPos + 180, 250, 20), featureToggle.ShowConstellations, "Show Constellations");
        }
    }
}