using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PBRNightSky
{
    public class PBRNightSkyControllerUI : MonoBehaviour
    {
        public PBRNightSkyController nightSkyController;
        public Text siderealTimeText;
        public InputField dayInput, monthInput, yearInput, hourInput, minuteInput, secondInput;
        public Toggle useRealTimeToggle;

        void Start()
        {
            ToggleUseRealTime(true);
            if (useRealTimeToggle != null)
            {
                useRealTimeToggle.onValueChanged.AddListener(ToggleUseRealTime);
            }
        }

        void Update()
        {
            if (nightSkyController.DateTime.UseRealTime)
            {
                UpdateDateTimeFields(DateTime.UtcNow);
            }

            UpdateSiderealTime();
        }

        void ToggleUseRealTime(bool useRealTime)
        {
            nightSkyController.DateTime.UseRealTime = useRealTime;
            if (useRealTime)
            {
                UpdateDateTimeFields(DateTime.UtcNow);
            }
        }

        void UpdateDateTimeFields(DateTime dateTime)
        {
            if (dayInput != null) dayInput.text = dateTime.Day.ToString();
            if (monthInput != null) monthInput.text = dateTime.Month.ToString();
            if (yearInput != null) yearInput.text = dateTime.Year.ToString();
            if (hourInput != null) hourInput.text = dateTime.Hour.ToString();
            if (minuteInput != null) minuteInput.text = dateTime.Minute.ToString();
            if (secondInput != null) secondInput.text = dateTime.Second.ToString();
        }

        public void ApplyDateTime()
        {
            if (!nightSkyController.DateTime.UseRealTime)
            {
                int day = int.Parse(dayInput.text);
                int month = int.Parse(monthInput.text);
                int year = int.Parse(yearInput.text);
                int hour = int.Parse(hourInput.text);
                int minute = int.Parse(minuteInput.text);
                int second = int.Parse(secondInput.text);

                nightSkyController.DateTime.SetDateTime(year, month, day, hour, minute, second);
            }
        }

        void UpdateSiderealTime()
        {
            double localSiderealTime = AstronomyCalculator.SiderealTime(nightSkyController.Longitude, nightSkyController.DateTime.GetDateTime());
            string siderealString = AstronomyCalculator.ConvertToDMS(localSiderealTime);
            if (siderealTimeText != null) siderealTimeText.text = $"Local Sidereal Time: {siderealString}";
        }
    }
}
