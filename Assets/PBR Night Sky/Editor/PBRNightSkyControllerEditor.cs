using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PBRNightSky {
    /// <summary>
    /// Custom editor script for the PBRNightSkyController.
    /// </summary>
    [CustomEditor(typeof(PBRNightSkyController), true)]
    public class PBRNightSkyControllerEditor : Editor {

        private int[] daysOfTheMonths = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private Dictionary<CelestialObject, bool> isExpandedDict = new Dictionary<CelestialObject, bool>();

        /// <summary>
        /// Creates the custom editor and displays it.
        /// </summary>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();

            PBRNightSkyController nightSkyController = target as PBRNightSkyController;

            SerializedDateTime dateTime = nightSkyController.DateTime;
            EditorGUI.indentLevel++;

            dateTime.UseRealTime = EditorGUILayout.Toggle("Use Real Time: ", dateTime.UseRealTime);

            if (dateTime.UseRealTime) {
                dateTime.SetDateTime(DateTime.UtcNow);
            }

            EditorGUI.BeginDisabledGroup(dateTime.UseRealTime);

            dateTime.Ticks = Math.Clamp(EditorGUILayout.LongField("Ticks:", dateTime.Ticks), 624511296000000000, 646917407990000000);

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 80;
            EditorGUIUtility.fieldWidth = 0;
            EditorGUILayout.LabelField("Time (HH:mm:ss):", GUILayout.ExpandWidth(false));
            EditorGUIUtility.labelWidth = 30;
            EditorGUIUtility.fieldWidth = 20;
            dateTime.Hour = Mathf.Clamp(EditorGUILayout.IntField(" ", dateTime.Hour, GUILayout.ExpandWidth(false)), 0, 23);
            dateTime.Minute = Mathf.Clamp(EditorGUILayout.IntField(":", dateTime.Minute, GUILayout.ExpandWidth(false)), 0, 59);
            dateTime.Second = Mathf.Clamp(EditorGUILayout.IntField(":", dateTime.Second, GUILayout.ExpandWidth(false)), 0, 59);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 80;
            EditorGUIUtility.fieldWidth = 0;
            EditorGUILayout.LabelField("Date (dd:MM:yyyy):", GUILayout.ExpandWidth(false));
            EditorGUIUtility.labelWidth = 30;
            EditorGUIUtility.fieldWidth = 20;
            dateTime.Day = Mathf.Clamp(EditorGUILayout.IntField(" ", dateTime.Day, GUILayout.ExpandWidth(false)), 1, daysOfTheMonths[Mathf.Max(0, dateTime.Month - 1)]);
            dateTime.Month = Mathf.Clamp(EditorGUILayout.IntField("/", dateTime.Month, GUILayout.ExpandWidth(false)), 1, 12);
            EditorGUIUtility.fieldWidth = 40;
            dateTime.Year = Mathf.Clamp(EditorGUILayout.IntField("/", dateTime.Year, GUILayout.ExpandWidth(false)), 1980, 2050);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(nightSkyController);
                Undo.RegisterCompleteObjectUndo(nightSkyController, "Modified DateTime in Physically Based Night Sky Controller");
            }

            EditorGUI.EndDisabledGroup();

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.richText = true;
            GUIStyle titleStyle = new GUIStyle(EditorStyles.label);
            titleStyle.richText = true;
            titleStyle.fontSize *= 2;
            GUIStyle informationStyle = new GUIStyle(EditorStyles.label);
            informationStyle.richText = true;


            double localSiderealTime = nightSkyController.ShouldUpdate ? AstronomyCalculator.SiderealTime(nightSkyController.Longitude, dateTime.GetDateTime()) : 0;
            string siderealString = AstronomyCalculator.ConvertToDMS(localSiderealTime);

            GUILayout.Space(10);
            GUILayout.Label("<b>Space Data:</b>", titleStyle);

            EditorGUI.BeginDisabledGroup(!nightSkyController.ShouldUpdate);
            GUILayout.Label("<b>Local Sidereal Time:</b> " + siderealString, informationStyle);

            foreach (var pair in nightSkyController.CelestialData) {
                string rightAscensionPP = AstronomyCalculator.ConvertToHMS(pair.Value.rightAscension);
                string declinationPP = AstronomyCalculator.ConvertToDMS(pair.Value.declination);
                string azimuthPP = AstronomyCalculator.ConvertToDMS(pair.Value.azimuth);
                string altitudePP = AstronomyCalculator.ConvertToDMS(pair.Value.altitude);

                bool isExpanded = isExpandedDict.TryGetValue(pair.Key, out isExpanded) && isExpanded;

                isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(isExpanded, $"<b>{pair.Key}</b>", foldoutStyle);

                if (isExpanded) {
                    GUILayout.Label("<b>RA:</b> " + rightAscensionPP, informationStyle);
                    GUILayout.Label("<b>DEC:</b> " + declinationPP, informationStyle);
                    GUILayout.Label("<b>AZM:</b> " + azimuthPP, informationStyle);
                    GUILayout.Label("<b>ALT:</b> " + altitudePP, informationStyle);
                }

                EditorGUILayout.EndFoldoutHeaderGroup();

                isExpandedDict[pair.Key] = isExpanded;
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}