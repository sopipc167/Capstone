using System;
using System.Collections.Generic;
using UnityEngine;

namespace PBRNightSky {
    /// <summary>
    /// Class to calculate the realtime celestial data for the celestial objects in the solar system.
    /// </summary>
    public class SolarSystemCelestial : MonoBehaviour {

        public static readonly Dictionary<CelestialObject, PlanetData> Planets = new Dictionary<CelestialObject, PlanetData>() {
            { CelestialObject.Mercury, new PlanetData() {
                semimajorAxisLength = 0.38710, eccentricity = 0.20563, inclination = 7.005, perihelion = 29.125, eclipticLongitude = 48.331, meanAnomaly = 174.795, dailyTraversalAngle = 4.092317 } },
            { CelestialObject.Venus, new PlanetData() {
                semimajorAxisLength = 0.72333, eccentricity = 0.00677, inclination = 3.395, perihelion = 54.884, eclipticLongitude = 76.680, meanAnomaly = 50.416, dailyTraversalAngle = 1.602136 } },
            { CelestialObject.Earth, new PlanetData() {
                semimajorAxisLength = 1.00000, eccentricity = 0.01671, inclination = 0, perihelion = 288.064, eclipticLongitude = 174.873176, meanAnomaly = 357.529, dailyTraversalAngle = 0.9856076686 } },
            { CelestialObject.Mars, new PlanetData() {
                semimajorAxisLength = 1.52368, eccentricity = 0.09340, inclination = 1.850, perihelion = 286.502, eclipticLongitude = 49.558, meanAnomaly = 19.373, dailyTraversalAngle = 0.524039 } },
            { CelestialObject.Jupiter, new PlanetData() {
                semimajorAxisLength = 5.20260, eccentricity = 0.04849, inclination = 1.303, perihelion = 273.867, eclipticLongitude = 100.464, meanAnomaly = 20.020, dailyTraversalAngle = 0.083056 } },
            { CelestialObject.Saturn, new PlanetData() {
                semimajorAxisLength = 9.55491, eccentricity = 0.05551, inclination = 2.489, perihelion = 339.391, eclipticLongitude = 113.666, meanAnomaly = 317.021, dailyTraversalAngle = 0.033371 } },
            { CelestialObject.Uranus, new PlanetData() {
                semimajorAxisLength = 19.21845, eccentricity = 0.04630, inclination = 0.773, perihelion = 98.999, eclipticLongitude = 74.006, meanAnomaly = 141.050, dailyTraversalAngle = 0.011698 } },
            { CelestialObject.Neptune, new PlanetData() {
                semimajorAxisLength = 30.11039, eccentricity = 0.00899, inclination = 1.770, perihelion = 276.340, eclipticLongitude = 131.784, meanAnomaly = 256.225, dailyTraversalAngle = 0.005965 } },
            { CelestialObject.Pluto, new PlanetData() {
                semimajorAxisLength = 39.543, eccentricity = 0.2490, inclination = 17.140, perihelion = 113.768, eclipticLongitude = 110.307, meanAnomaly = 14.882, dailyTraversalAngle = 0.003964 } }
        };

        /// <summary>
        /// Calculates the realtime celestial data for the moon.
        /// </summary>
        /// <param name="time">The time to calculate the moon's celestial data at.</param>
        /// <param name="latitude">The geographical latitude of the observer.</param>
        /// <param name="longitude">The geographical longitude of the observer.</param>
        /// <returns>The realtime celestial data.</returns>
        public static RealtimeCelestialData CalculateMoonRotation(DateTime time, double latitude, double longitude) {
            RealtimeCelestialData moonData = new RealtimeCelestialData();
            double daysSinceJulianDate = AstronomyCalculator.DaysSinceJulianDate(time);
            double L = AstronomyCalculator.BringAngleIntoRangeDegrees(218.316 + 13.176396 * daysSinceJulianDate);
            double M = AstronomyCalculator.BringAngleIntoRangeDegrees(134.963 + 13.064993 * daysSinceJulianDate);
            double F = AstronomyCalculator.BringAngleIntoRangeDegrees(93.272 + 13.229350 * daysSinceJulianDate);

            double geocentricLatitude = 5.128 * AstronomyCalculator.SinD(F);
            double geocentricLongitude = L + 6.289 * AstronomyCalculator.SinD(M);
            double distance = 385001 - 20905 * AstronomyCalculator.CosD(M);

            moonData.rightAscension = AstronomyCalculator.RightAscension(geocentricLatitude, geocentricLongitude);
            moonData.declination = AstronomyCalculator.Declination(geocentricLatitude, geocentricLongitude);
            moonData.siderealTime = AstronomyCalculator.SiderealTime(longitude, time);
            moonData.hourAngle = AstronomyCalculator.HourAngle(moonData.siderealTime, moonData.rightAscension);
            moonData.altitude = AstronomyCalculator.Altitude(moonData.declination, latitude, moonData.hourAngle);
            moonData.azimuth = AstronomyCalculator.Azimuth(moonData.declination, latitude, moonData.hourAngle);

            return moonData;
        }

        /// <summary>
        /// Calculates a given planets's heliocentric ecliptical coordinates and all the data required to do so at a given date
        /// and time.
        /// </summary>
        /// <param name="time">The date and time.</param>
        /// <param name="planet">The planet.</param>
        /// <returns>The realtime data required to calculate the heliocentric ecliptical coordinates.</returns>
        public static RealtimeCelestialData CalculateCelestialXYZ(DateTime time, CelestialObject planet) {
            RealtimeCelestialData data = new RealtimeCelestialData();
            PlanetData celestialBodyData = Planets[planet];
            data.meanAnomaly = AstronomyCalculator.MeanAnomaly(time, celestialBodyData.meanAnomaly, celestialBodyData.dailyTraversalAngle);
            data.trueAnomaly = AstronomyCalculator.CalculateTrueAnomaly(celestialBodyData.eccentricity, data.meanAnomaly);
            data.distanceToSun = AstronomyCalculator.DistanceToTheSun(celestialBodyData.semimajorAxisLength, celestialBodyData.eccentricity, data.trueAnomaly);
            data.position = AstronomyCalculator.HeliocentricEclipticalCoordinates(data.distanceToSun, celestialBodyData.perihelion, celestialBodyData.eclipticLongitude, celestialBodyData.inclination, data.trueAnomaly);
            return data;
        }

        /// <summary>
        /// Calculates a given celestial body's altitude and azimuth rotation and all the data required to do so at a given date, 
        /// time, latitude and longitude.
        /// </summary>
        /// <param name="time">The date and time.</param>
        /// <param name="latitude">The geographic latitude of the observer.</param>
        /// <param name="longitude">The geographic longitude of the observer.</param>
        /// <param name="earthXYZ">The heliocentric ecliptical coordinates of earth.</param>
        /// <param name="data">The existing data of the celestial body, which should have the heliocentric ecliptical coordinates
        /// already calculated.</param>
        /// <returns>The realtime data for the given celestial body, with all data required to calculate the altitude and 
        /// azimuth angles.</returns>
        public static RealtimeCelestialData CalculateCelestialObjectRotation(DateTime time, double latitude, double longitude, HeliocentricEclipticalCoordinates earthXYZ, RealtimeCelestialData data) {
            double x = data.position.x - earthXYZ.x;
            double y = data.position.y - earthXYZ.y;
            double z = data.position.z - earthXYZ.z;

            AstronomyCalculator.GeocentricEclipticalLongitudeAndLatitude(x, y, z, out double geocentricEclipticLatitude, out double geocentricEclipticLongitude);
            data.geocentricEclipticLatitude = geocentricEclipticLatitude;
            data.geocentricEclipticLongitude = geocentricEclipticLongitude;
            data.rightAscension = AstronomyCalculator.RightAscension(geocentricEclipticLatitude, geocentricEclipticLongitude);
            data.declination = AstronomyCalculator.Declination(geocentricEclipticLatitude, geocentricEclipticLongitude);
            data.siderealTime = AstronomyCalculator.SiderealTime(longitude, time);
            data.hourAngle = AstronomyCalculator.HourAngle(data.siderealTime, data.rightAscension);
            data.altitude = AstronomyCalculator.Altitude(data.declination, latitude, data.hourAngle);
            data.azimuth = AstronomyCalculator.Azimuth(data.declination, latitude, data.hourAngle);

            return data;
        }

        /// <summary>
        /// The data required to calculate a planet's various realtime data.
        /// </summary>
        public class PlanetData {
            public double semimajorAxisLength;
            public double eccentricity;
            public double perihelion;
            public double eclipticLongitude;
            public double inclination;
            public double meanAnomaly;
            public double dailyTraversalAngle;
        }
    }

    /// <summary>
    /// Realtime celestial data calculated for a celestial body.
    /// </summary>
    public struct RealtimeCelestialData {
        public double meanAnomaly;
        public double trueAnomaly;
        public double distanceToSun;
        public HeliocentricEclipticalCoordinates position;
        public double geocentricEclipticLatitude;
        public double geocentricEclipticLongitude;
        public double rightAscension;
        public double declination;
        public double siderealTime;
        public double hourAngle;
        public double altitude;
        public double azimuth;
    }

    /// <summary>
    /// Celestial heliocentric ecliptical coordinates.
    /// </summary>
    public struct HeliocentricEclipticalCoordinates {
        public double x, y, z;
    }

    /// <summary>
    /// Types of supported celestial objects.
    /// </summary>
    [Serializable]
    public enum CelestialObject {
        Sun,
        Moon,
        Mercury,
        Venus,
        Earth,
        Mars,
        Jupiter,
        Saturn,
        Uranus,
        Neptune,
        Pluto
    }
}