using System;
using UnityEngine;

namespace PBRNightSky {
    /// <summary>
    /// Wrapper class for System.DateTime to use Unity serialisation.
    /// </summary>
    [Serializable]
    public class SerializedDateTime {

        /// <summary>
        /// Gets or sets the serialized time in ticks.
        /// </summary>
        public long Ticks {
            get {
                return ticks;
            }
            set {
                ApplyDateTime(new DateTime(value));
            }
        }

        /// <summary>
        /// Gets or sets the year component of the serialized time.
        /// </summary>
        public int Year {
            get {
                return year;
            }
            set {
                ApplyDateTime(new DateTime(Mathf.Clamp(value, 1980, 2050), month, day, hour, minute, second));
            }
        }

        /// <summary>
        /// Gets or sets the month component of the serialized time.
        /// </summary>
        public int Month {
            get {
                return month;
            }
            set {
                ApplyDateTime(new DateTime(year, Mathf.Clamp(value, 1, 12), day, hour, minute, second));
            }
        }

        /// <summary>
        /// Gets or sets the day component of the serialized time.
        /// </summary>
        public int Day {
            get {
                return day;
            }
            set {
                ApplyDateTime(new DateTime(year, month, Mathf.Clamp(value, 1, DateTime.DaysInMonth(year, month)), hour, minute, second));
            }
        }

        /// <summary>
        /// Gets or sets the hour component of the serialized time.
        /// </summary>
        public int Hour {
            get {
                return hour;
            }
            set {
                ApplyDateTime(new DateTime(year, month, day, Mathf.Clamp(value, 0, 23), minute, second));
            }
        }

        /// <summary>
        /// Gets or sets the minute component of the serialized time.
        /// </summary>
        public int Minute {
            get {
                return minute;
            }
            set {
                ApplyDateTime(new DateTime(year, month, day, hour, Mathf.Clamp(value, 0, 59), second));
            }
        }

        /// <summary>
        /// Gets or sets the second component of the serialized time.
        /// </summary>
        public int Second {
            get {
                return second;
            }
            set {
                ApplyDateTime(new DateTime(year, month, day, hour, minute, Mathf.Clamp(value, 0, 59)));
            }
        }

        /// <summary>
        /// Gets or sets a boolean indicating whether to use real-time or serialized time.
        /// </summary>
        [HideInInspector]
        public bool UseRealTime;

        [SerializeField]
        [HideInInspector]
        private long ticks;

        [SerializeField]
        [HideInInspector]
        private int year;

        [SerializeField]
        [HideInInspector]
        private int month;

        [SerializeField]
        [HideInInspector]
        private int day;

        [SerializeField]
        [HideInInspector]
        private int hour;

        [SerializeField]
        [HideInInspector]
        private int minute;

        [SerializeField]
        [HideInInspector]
        private int second;

        /// <summary>
        /// Gets the current date and time either in real-time or from the serialized time.
        /// </summary>
        /// <returns>A DateTime object representing the current date and time.</returns>
        public DateTime GetDateTime() {
            if (UseRealTime) {
                return DateTime.UtcNow;
            }
            else {
                return new DateTime(Ticks);
            }
        }

        /// <summary>
        /// Sets the serialized time using a DateTime object.
        /// </summary>
        /// <param name="dateTime">The DateTime object to set the serialized time to.</param>
        public void SetDateTime(DateTime dateTime) {
            ApplyDateTime(dateTime);
        }

        /// <summary>
        /// Sets the serialized time using individual components.
        /// </summary>
        /// <param name="year">The year component of the new time.</param>
        /// <param name="month">The month component of the new time.</param>
        /// <param name="day">The day component of the new time.</param>
        /// <param name="hour">The hour component of the new time.</param>
        /// <param name="minute">The minute component of the new time.</param>
        /// <param name="second">The second component of the new time.</param>
        /// <returns>A DateTime object representing the newly set date and time.</returns>
        public DateTime SetDateTime(int year, int month, int day, int hour, int minute, int second) {
            DateTime newDateTime = new DateTime(year, month, day, hour, minute, second);
            ApplyDateTime(newDateTime);
            return newDateTime;
        }

        /// <summary>
        /// Adds a given timespan to the current time.
        /// </summary>
        /// <param name="timeSpan">The span of time to increase the current time by.</param>
        public void AddTime(TimeSpan timeSpan) {
            DateTime currentTime = GetDateTime();
            DateTime newTIme = currentTime + timeSpan;
            ApplyDateTime(newTIme);
        }

        /// <summary>
        /// Applies the components of a DateTime object to the serialized time.
        /// </summary>
        /// <param name="dateTime">The DateTime object to apply.</param>
        private void ApplyDateTime(DateTime dateTime) {
            hour = dateTime.Hour;
            minute = dateTime.Minute;
            second = dateTime.Second;
            day = dateTime.Day;
            month = dateTime.Month;
            year = dateTime.Year;
            ticks = dateTime.Ticks;
        }
    }
}