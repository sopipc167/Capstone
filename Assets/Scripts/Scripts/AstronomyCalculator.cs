using System;

namespace PBRNightSky {
    /// <summary>
    /// Class used to calculate astronomical data.
    /// </summary>
    public static class AstronomyCalculator {

        public static readonly DateTime JulianReferenceDate = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        public const double Deg2Rad = Math.PI / 180.0;
        public const double Rad2Deg = 180.0 / Math.PI;
        private const double obliquityOfTheEcliptic = 23.4392911;
        private static readonly double SinOfObliquityOfTheEcliptic = SinD(obliquityOfTheEcliptic);
        private static readonly double CosOfObliquityOfTheEcliptic = CosD(obliquityOfTheEcliptic);

        /// <summary>
        /// Sine helper function in degrees.
        /// </summary>
        /// <param name="degrees">The degrees to get the sine value of.</param>
        /// <returns>The sine value of the provided degrees.</returns>
        public static double SinD(double degrees) => Math.Sin(degrees * Deg2Rad);

        /// <summary>
        /// Cosine helper function in degrees.
        /// </summary>
        /// <param name="degrees">The degrees to get the cosine value of.</param>
        /// <returns>The cosine value of the provided degrees.</returns>
        public static double CosD(double degrees) => Math.Cos(degrees * Deg2Rad);

        /// <summary>
        /// Tangent helper function in degrees.
        /// </summary>
        /// <param name="degrees">The degrees to get the tangent value of.</param>
        /// <returns>The tangent value of the provided degrees.</returns>
        public static double TanD(double degrees) => Math.Tan(degrees * Deg2Rad);

        /// <summary>
        /// Arcsine helper function in degrees.
        /// </summary>
        /// <param name="val">The value to get the arcsine degree of.</param>
        /// <returns>The arcsine degree of the provided value.</returns>
        static double ASinD(double val) => Rad2Deg * Math.Asin(val);

        /// <summary>
        /// Arctan2 helper function in degrees.
        /// </summary>
        /// <param name="y">The y value to get the arctan2 degree of.</param>
        /// <param name="x">The x value to get the arctan2 degree of.</param>
        /// <returns>The arctan2 degrees of the provided values.</returns>
        static double Atan2D(double y, double x) => Rad2Deg * Math.Atan2(y, x);

        /// <summary>
        /// Calculates the fractional days since the Julian reference date.
        /// </summary>
        /// <param name="dateTime">The date to calculate the difference in days.</param>
        /// <returns>The fractional days since the Julian date.</returns>
        public static double DaysSinceJulianDate(DateTime dateTime) => (dateTime - JulianReferenceDate).TotalDays;

        /// <summary>
        /// Calculates the mean anomaly of a celestial body.
        /// </summary>
        /// <param name="time">The time to calculate the mean anomaly at.</param>
        /// <param name="initialMeanAnomaly">The initial reference mean anomaly of the celestial body.</param>
        /// <param name="dailyTraversalAngle">The daily traversal angle of the celesital body around Earth.</param>
        /// <returns>The calculated mean anomaly of the celestial body</returns>
        public static double MeanAnomaly(DateTime time, double initialMeanAnomaly, double dailyTraversalAngle) {
            return BringAngleIntoRangeDegrees(initialMeanAnomaly + dailyTraversalAngle * DaysSinceJulianDate(time));
        }

        /// <summary>
        /// Calculates the true anomaly of a celestial body by solving Kepler's equation (Only supports eliptical orbits).
        /// </summary>
        /// <param name="eccentricity">The eccentricity of the body (must be less than 1).</param>
        /// <param name="meanAnomaly">The mean anomaly of the body.</param>
        /// <returns>The calculated true anomaly of the celestial body.</returns>
        public static double CalculateTrueAnomaly(double eccentricity, double meanAnomaly) {
            meanAnomaly *= Deg2Rad;

            double parabolicExcess = eccentricity - 1;
            double absParabolicExcess = Math.Abs(parabolicExcess);
            double perifocalAnomaly = meanAnomaly / Math.Sqrt(absParabolicExcess * absParabolicExcess * absParabolicExcess);

            double W = Math.Sqrt(9.0 / 8.0) * (perifocalAnomaly / eccentricity);
            double u = Math.Cbrt(W + Math.Sqrt(W * W + (1.0 / (eccentricity * eccentricity * eccentricity))));
            double T = u - (1.0 / (eccentricity * u));

            double E0 = T * Math.Sqrt(2 * absParabolicExcess);

            double Ei = E0;
            double deltaEi = 0;
            double Bi = 1;
            double epsilon = 2.2e-16;

            int i = 0;
            int maxIterations = 100;

            do {
                double si = eccentricity * Math.Sin(Ei);
                double ci = 1 - eccentricity * Math.Cos(Ei);
                double di = meanAnomaly - Ei + si;
                deltaEi = di / ci;
                Bi = Math.Abs((2 * epsilon * Ei * ci) / si);
                Ei = Ei + deltaEi;
                i++;
            }
            while (deltaEi * deltaEi > Bi && i < maxIterations);

            double E = Ei;
            double tau = Math.Sqrt((eccentricity + 1) / absParabolicExcess) * Math.Tan(0.5 * E);

            return BringAngleIntoRangeDegrees(2 * Math.Atan(tau) * Rad2Deg);
        }

        /// <summary>
        /// Calculates the distance between the celestial body and the sun.
        /// </summary>
        /// <param name="semiMajorAxisLength">The semi major axis length of the celestial body.</param>
        /// <param name="eccentricity">The eccentricity of the body (must be less than 1).</param>
        /// <param name="trueAnomaly">The true anomaly of the celesital body.</param>
        /// <returns>The distance to the sun.</returns>
        public static double DistanceToTheSun(double semiMajorAxisLength, double eccentricity, double trueAnomaly) {
            double numerator = semiMajorAxisLength * (1 - eccentricity * eccentricity);
            return numerator / (1 + eccentricity * CosD(trueAnomaly));
        }

        /// <summary>
        /// Calculates the heliocentric ecliptical coordiantes of a celestial body.
        /// </summary>
        /// <param name="distanceToSun">The distance between the celestial body and the sun.</param>
        /// <param name="perihelion">The perihelion of the celestial body.</param>
        /// <param name="eclipticLongitude">The ecliptic longitude of the celestial body.</param>
        /// <param name="inclination">The inclination of the celestial body.</param>
        /// <param name="trueAnomaly">The true anomaly of the celesital body.</param>
        /// <returns>The heliocentric ecliptical coordiantes.</returns>
        public static HeliocentricEclipticalCoordinates HeliocentricEclipticalCoordinates(double distanceToSun, double perihelion, double eclipticLongitude, double inclination, double trueAnomaly) {
            HeliocentricEclipticalCoordinates planetXYZ = new HeliocentricEclipticalCoordinates();
            planetXYZ.x = distanceToSun * (CosD(eclipticLongitude) * CosD(perihelion + trueAnomaly) - SinD(eclipticLongitude) * CosD(inclination) * SinD(perihelion + trueAnomaly));
            planetXYZ.y = distanceToSun * (SinD(eclipticLongitude) * CosD(perihelion + trueAnomaly) + CosD(eclipticLongitude) * CosD(inclination) * SinD(perihelion + trueAnomaly));
            planetXYZ.z = distanceToSun * SinD(inclination) * SinD(perihelion + trueAnomaly);
            return planetXYZ;
        }

        /// <summary>
        /// Calculates the geocentric ecliptical longitude and latitude of a celestial body.
        /// </summary>
        /// <param name="x">The heliocentric ecliptical x coordinate.</param>
        /// <param name="y">The heliocentric ecliptical y coordinate.</param>
        /// <param name="z">The heliocentric ecliptical z coordinate.</param>
        /// <param name="geocentricEclipticLatitude">The geocentric ecliptic latitude.</param>
        /// <param name="geocentricEclipticLongitude">The geocentric ecliptic longitude.</param>
        public static void GeocentricEclipticalLongitudeAndLatitude(double x, double y, double z, out double geocentricEclipticLatitude, out double geocentricEclipticLongitude) {
            double distanceToEarth = Math.Sqrt(x * x + y * y + z * z);

            geocentricEclipticLatitude = ASinD(z / distanceToEarth);
            geocentricEclipticLongitude = Atan2D(y, x);
        }

        /// <summary>
        /// Calculates the declination of a celestial body.
        /// </summary>
        /// <param name="geocentricLatitude">The geocentric ecliptic latitude of the celestial body.</param>
        /// <param name="geocentricLongitude">The geocentric ecliptic longitude of the celestial body.</param>
        /// <returns></returns>
        public static double Declination(double geocentricLatitude, double geocentricLongitude) {
            return ASinD(SinD(geocentricLatitude) * CosOfObliquityOfTheEcliptic + CosD(geocentricLatitude) * SinOfObliquityOfTheEcliptic * SinD(geocentricLongitude));
        }

        /// <summary>
        /// Calculates the right ascension of a celestial body.
        /// </summary>
        /// <param name="geocentricLatitude">The geocentric ecliptic latitude of the celestial body.</param>
        /// <param name="geocentricLongitude">The geocentric ecliptic longitude of the celestial body.</param>
        /// <returns>The right ascension</returns>
        public static double RightAscension(double geocentricLatitude, double geocentricLongitude) {
            return BringAngleIntoRangeDegrees(Atan2D(SinD(geocentricLongitude) * CosOfObliquityOfTheEcliptic - TanD(geocentricLatitude) * SinOfObliquityOfTheEcliptic, CosD(geocentricLongitude)));
        }

        /// <summary>
        /// Calculates the sidereal time from an observers location at a given time, the time is based on the UTC time
        /// and not the local time.
        /// </summary>
        /// <param name="geographicalLongitude">The longitude of the observer.</param>
        /// <param name="time">The time to calculate the sidereal time at.</param>
        /// <returns>The sidereal time.</returns>
        public static double SiderealTime(double geographicalLongitude, DateTime time) {
            double T = DaysSinceJulianDate(time) / 36525.0;
            double greenwichSidereal = 280.46061837 + 360.98564736629 * DaysSinceJulianDate(time) + 0.000387933 * T * T - (T * T * T) / 38710000.0;
            return BringAngleIntoRangeDegrees(greenwichSidereal + geographicalLongitude);
        }

        /// <summary>
        /// Calculates the hour angle of a celestial body.
        /// </summary>
        /// <param name="siderealTime">The sidereal time.</param>
        /// <param name="rightAscension">The right ascension of the celestial body</param>
        /// <returns>The hour angle.</returns>
        public static double HourAngle(double siderealTime, double rightAscension) {
            return siderealTime - rightAscension;
        }

        /// <summary>
        /// Calculates the altitude angle of a celestial body.
        /// </summary>
        /// <param name="declination">The declination in degrees of the celestial body.</param>
        /// <param name="geographicalLatitude">The geographical latitude.</param>
        /// <param name="hourAngle">The hour angle of the celestial body.</param>
        /// <returns>The altitude angle in degrees.</returns>
        public static double Altitude(double declination, double geographicalLatitude, double hourAngle) {
            return ASinD(SinD(geographicalLatitude) * SinD(declination) + CosD(geographicalLatitude) * CosD(declination) * CosD(hourAngle));
        }

        /// <summary>
        /// Calculates the azimuth angle of a celestial body.
        /// </summary>
        /// <param name="declination">The declination in degrees of the celestial body.</param>
        /// <param name="geographicalLatitude">The geographical latitude.</param>
        /// <param name="hourAngle">The hour angle of the celestial body.</param>
        /// <returns>The azmiuth angle in degrees.</returns>
        public static double Azimuth(double declination, double geographicalLatitude, double hourAngle) {
            double sinAzimuth = -CosD(declination) * SinD(hourAngle);
            double cosAzimuth = SinD(declination) * CosD(geographicalLatitude) - CosD(declination) * SinD(geographicalLatitude) * CosD(hourAngle);
            double azimuth = Atan2D(sinAzimuth, cosAzimuth);
            return BringAngleIntoRangeDegrees(azimuth);
        }

        /// <summary>
        /// Brings an angle into the range of 0 to 360 degrees.
        /// </summary>
        /// <param name="angle">The angle to bring into range in degrees.</param>
        /// <returns>The modified angle.</returns>
        public static double BringAngleIntoRangeDegrees(double angle) {
            double remainder = angle % 360.0;
            if (remainder < 0) {
                remainder += 360.0;
            }
            return remainder;
        }

        /// <summary>
        /// Converts an angle in degrees to a pretty print string in the hours:minutes:seconds format.
        /// </summary>
        /// <param name="degrees">The angle to convert in degrees.</param>
        /// <returns>The pretty print string.</returns>
        public static string ConvertToHMS(double degrees) {
            int sign = Math.Sign(degrees);
            degrees = Math.Abs(degrees);

            double hours = degrees / 15;
            double minutes = (hours - Math.Floor(hours)) * 60;
            double seconds = (minutes - Math.Floor(minutes)) * 60;
            return $"{sign * Math.Floor(hours)}h, {Math.Floor(minutes)}m {Math.Round(seconds)}s";
        }

        /// <summary>
        /// Converts an angle in degrees to a pretty print string in the degrees:minutes:seconds format.
        /// </summary>
        /// <param name="degrees">The angle to convert in degrees.</param>
        /// <returns>The pretty print string.</returns>
        public static string ConvertToDMS(double degrees) {
            int sign = Math.Sign(degrees);
            degrees = Math.Abs(degrees);

            double minutes = (degrees - Math.Floor(degrees)) * 60;
            double seconds = (minutes - Math.Floor(minutes)) * 60;
            return $"{sign * Math.Floor(degrees)}°, {Math.Floor(minutes)}' {Math.Round(seconds)}\"";
        }
    }
}