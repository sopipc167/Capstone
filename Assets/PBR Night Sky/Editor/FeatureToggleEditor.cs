using UnityEditor;
using UnityEngine;

namespace PBRNightSky {
    /// <summary>
    /// A custom editor for the feature toggle class.
    /// </summary>
    [CustomEditor(typeof(FeatureToggle))]
    public class FeatureToggleEditor : Editor {
        /// <summary>
        /// Adds some buttons in the editor that when pressed, showcase different scenarios.
        /// </summary>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            FeatureToggle toggle = (FeatureToggle)target;
            PBRNightSkyController controller = toggle.controller;

            if (GUILayout.Button("Set to Day")) {
                EditorUtility.SetDirty(controller);
                Undo.RegisterCompleteObjectUndo(controller, "Set to Day in Feature Toggle Editor");
                controller.DateTime.UseRealTime = false;
                controller.DateTime.SetDateTime(2023, 06, 01, 12, 00, 00);
                controller.Latitude = 51.5286071;
                controller.Longitude = -0.4312088;
            }

            if (GUILayout.Button("Set to Night")) {
                EditorUtility.SetDirty(controller);
                Undo.RegisterCompleteObjectUndo(controller, "Set to Night in Feature Toggle Editor");
                controller.DateTime.UseRealTime = false;
                controller.DateTime.SetDateTime(2023, 01, 01, 00, 00, 00);
                controller.Latitude = 51.5286071;
                controller.Longitude = -0.4312088;
            }

            if (GUILayout.Button("Set to Sunset")) {
                EditorUtility.SetDirty(controller);
                Undo.RegisterCompleteObjectUndo(controller, "Set to Sunset in Feature Toggle Editor");
                controller.DateTime.UseRealTime = false;
                controller.DateTime.SetDateTime(2023, 01, 01, 16, 00, 00);
                controller.Latitude = 51.5286071;
                controller.Longitude = -0.4312088;
            }

            if (GUILayout.Button("Set to Sunrise")) {
                EditorUtility.SetDirty(controller);
                Undo.RegisterCompleteObjectUndo(controller, "Set to Sunrise in Feature Toggle Editor");
                controller.DateTime.UseRealTime = false;
                controller.DateTime.SetDateTime(2023, 01, 01, 08, 10, 00);
                controller.Latitude = 51.5286071;
                controller.Longitude = -0.4312088;
            }

            if (GUILayout.Button("Set to Solar Eclipse")) {
                EditorUtility.SetDirty(controller);
                Undo.RegisterCompleteObjectUndo(controller, "Set to Solar Eclipse in Feature Toggle Editor");
                controller.DateTime.UseRealTime = false;
                controller.DateTime.SetDateTime(2001, 06, 21, 13, 34, 06);
                controller.Latitude = -11.3;
                controller.Longitude = -2.7;
            }

            if (GUILayout.Button("Set to Lunar Eclipse")) {
                EditorUtility.SetDirty(controller);
                Undo.RegisterCompleteObjectUndo(controller, "Set to Lunar Eclipse in Feature Toggle Editor");
                controller.DateTime.UseRealTime = false;
                controller.DateTime.SetDateTime(2000, 01, 21, 03, 08, 30);
                controller.Latitude = -11.3;
                controller.Longitude = -2.7;
            }
        }
    }
}