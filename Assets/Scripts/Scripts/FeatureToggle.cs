using System;
using UnityEngine;
using UnityEngine.UI;

namespace PBRNightSky {
    /// <summary>
    /// A class used to showcase some features of the "PBRNightSkyController" component.
    /// </summary>
    [RequireComponent(typeof(PBRNightSkyController))]
    public class FeatureToggle : MonoBehaviour {

        public bool Timelapse { get { return timelapse; } set { timelapse = value; } }
        public bool ShowConstellations { get { return showConstellations; } set { showConstellations = value; } }

        [SerializeField]
        private bool timelapse;
        [SerializeField]
        private float timelapseSpeed;
        [SerializeField]
        private bool showConstellations;

        [HideInInspector]
        public PBRNightSkyController controller;

        /// <summary>
        /// Gets the PBR Night Sky Controller on validation.
        /// </summary>
        private void OnValidate() {
            controller = GetComponent<PBRNightSkyController>();
        }

        /// <summary>
        /// Updates the PBR Night Sky Controller.
        /// </summary>
        private void Update() {
            if (timelapse) {
                int milliseconds = (int)(Time.deltaTime * 1000 * timelapseSpeed);
                TimeSpan nextStep = new TimeSpan(0, 0, 0, 0, milliseconds);
                controller.DateTime.AddTime(nextStep);
            }

            controller.SkyMaterial.SetFloat("_ConstellationsFade", showConstellations ? 0.1f : 0);
        }
    }
}