using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace PBRNightSky {
    /// <summary>
    /// Controller class for managing the PBR night sky.
    /// </summary>
    [ExecuteAlways]
    public class PBRNightSkyController : MonoBehaviour {
        /// <summary>
        /// Gets the celestial data for various celestial objects.
        /// </summary>
        //public Dictionary<CelestialObject, RealtimeCelestialData> CelestialData => new Dictionary<CelestialObject, RealtimeCelestialData>(celestialData);

        /// <summary>
        /// Gets the material that is currently rendering the sky.
        /// </summary>
        public Material SkyMaterial => material;

        /// <summary>
        /// Should the celestial data be updated?
        /// </summary>
        public bool ShouldUpdate => Application.isPlaying || executeInEditor;

        /// <summary>
        /// Gets or sets the latitude of the observer.
        /// </summary>
        public double Latitude {
            get {
                return latitude;
            }
            set {
                latitude = value;
            }
        }

        /// <summary>
        /// Gets or sets the longitude of the observer.
        /// </summary>
        public double Longitude {
            get {
                return longitude;
            }
            set {
                longitude = value;
            }
        }

        public int Year
        {
            get
            {
                return year;
            }
            set
            {
                year = value;
            }
        }
        public int Month
        {
            get
            {
                return month;
            }
            set
            {
                month = value;
            }
        }
        public int Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
            }
        }
        public int Hour
        {
            get
            {
                return hour;
            }
            set
            {
                hour = value;
            }
        }
        public int Minute
        {
            get
            {
                return minute;
            }
            set
            {
                minute = value;
            }
        }
        public int Second
        {
            get
            {
                return second;
            }
            set
            {
                second = value;
            }
        }


        [SerializeField]
        [Tooltip("The material used to render the stars skybox.")]
        private Material material;

        [SerializeField]
        [Tooltip("The light that is used to represent the sun in the default skybox.")]
        private Light sun;

        [SerializeField]
        [Tooltip("The rate at which the calculations are executed in seconds (0 = every frame).")]
        [Range(0, 1)]
        private float updateRate;

        [SerializeField]
        [Tooltip("Should the sky be updated whilst in the editor. If this is unchecked, the celestial bodies will not " +
                 "update their positions in the editor, but will still render.")]
        private bool executeInEditor = true;

        [SerializeField]
        [Tooltip("The latitude of the observer.")]
        private double latitude;

        [SerializeField]
        [Tooltip("The longitude of the observer.")]
        private double longitude;

        [SerializeField]
        [Tooltip("The year of the observer.")]
        private int year;

        [SerializeField]
        [Tooltip("The month of the observer.")]
        private int month;

        [SerializeField]
        [Tooltip("The date of the observer.")]
        private int date;

        [SerializeField]
        [Tooltip("The hour of the observer.")]
        private int hour;

        [SerializeField]
        [Tooltip("The minute of the observer.")]
        private int minute;

        [SerializeField]
        [Tooltip("The second of the observer.")]
        private int second;

        //[HideInInspector]
        public SerializedDateTime DateTime;

        

        /*// Realtime celestial data calculated after every updateRate period has passed
        private Dictionary<CelestialObject, RealtimeCelestialData> celestialData = new Dictionary<CelestialObject, RealtimeCelestialData>() {
            { CelestialObject.Sun, new RealtimeCelestialData() },
            { CelestialObject.Moon, new RealtimeCelestialData() },
            { CelestialObject.Mercury, new RealtimeCelestialData() },
            { CelestialObject.Venus, new RealtimeCelestialData() },
            { CelestialObject.Earth, new RealtimeCelestialData() },
            { CelestialObject.Mars, new RealtimeCelestialData() },
            { CelestialObject.Jupiter, new RealtimeCelestialData() },
            { CelestialObject.Saturn, new RealtimeCelestialData() },
            { CelestialObject.Uranus, new RealtimeCelestialData() },
            { CelestialObject.Neptune, new RealtimeCelestialData() },
            { CelestialObject.Pluto , new RealtimeCelestialData() }
        };

        // Planet data to be used in the shader
        private Vector4[] planetRotations = new Vector4[8];
        private float[] planetSizes = new float[] { 
            0.0036f,
            0.18f,
            0.0069f,
            0.14f,
            0.0056f,
            0.0011f,
            0.00067f,
            0.00003f
        };
        private Color[] planetColours = new Color[] { 
            new Color(0.9f, 0.8f, 1.0f),
            new Color(1.0f, 1.0f, 1.0f),
            new Color(1.0f, 0.5f, 0.5f),
            new Color(1.0f, 1.0f, 0.5f),
            new Color(1.0f, 1.0f, 1.0f),
            new Color(1.0f, 1.0f, 1.0f),
            new Color(1.0f, 1.0f, 1.0f),
            new Color(1.0f, 1.0f, 1.0f),
        };*/
        // The internal date time used for calculations
        private DateTime systemDateTime;
        // The mesh used to render the night skybox
        private Mesh mesh;
        // Used to track the accumulation of time for rate limiting
        private float accumulator;

        /// <summary>
        /// Initializes the script by getting the default cube mesh.
        /// </summary>
        private void Awake() {
            mesh = GetDefaultCubeMesh();
        }

        private void Start()
        {
            
        }

        /// <summary>
        /// Subscribes to the camera's onPreCull event when the script is enabled. Allows for rendering in the scene and game
        /// view.
        /// </summary>
        private void OnEnable() {
            RenderPipelineAsset currentRenderPipeline = GraphicsSettings.renderPipelineAsset;

            if (currentRenderPipeline == null) {
                Camera.onPreCull -= DrawSky;
                Camera.onPreCull += DrawSky;
            }
            else {
                RenderPipelineManager.beginCameraRendering -= DrawSky;
                RenderPipelineManager.beginCameraRendering += DrawSky;
            }
        }

        /// <summary>
        /// Unsubscribes from the camera's onPreCull event when the script is disabled.
        /// </summary>
        private void OnDisable() {
            Camera.onPreCull -= DrawSky;
            RenderPipelineManager.beginCameraRendering -= DrawSky;
        }

        /// <summary>
        /// Draws the night sky using the specified camera.
        /// </summary>
        /// <param name="camera">The camera used to render the sky.</param>
        private void DrawSky(Camera camera) {

            if (camera.cameraType == CameraType.Preview) {
                return;
            }

            camera.depthTextureMode |= DepthTextureMode.Depth;
            Matrix4x4 matrix = Matrix4x4.TRS(camera.transform.position, Quaternion.identity, Vector3.one * camera.farClipPlane);
            Graphics.DrawMesh(mesh, matrix, material, 0, camera);
        }

        /// <summary>
        /// Draws the night sky using the specified camera.
        /// </summary>
        /// <param name="camera">The camera used to render the sky.</param>
        private void DrawSky(ScriptableRenderContext context, Camera camera) {
            DrawSky(camera);
        }

        /// <summary>
        /// Grabs the cube mesh by instantiating the primitive and destroying it.
        /// </summary>
        private Mesh GetDefaultCubeMesh() {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Mesh cubeMesh = cube.GetComponent<MeshFilter>().sharedMesh;
            if (Application.isPlaying) {
                Destroy(cube);
            }
            else if (executeInEditor) {
                DestroyImmediate(cube);
            }
            return cubeMesh;
        }

        /// <summary>
        /// Updates the script by calculating celestial object rotations based on the update rate.
        /// </summary>
        private void Update() {
            if (!ShouldUpdate) {
                return;
            }

            if (accumulator >= updateRate) {
                accumulator -= updateRate;
                systemDateTime = DateTime.GetDateTime();
                //UpdateCelestialData();
            }

            accumulator += Time.deltaTime;
        }

        /// <summary>
        /// Calculates and sets all celestial data in the shader.
        /// </summary>
        /*private void UpdateCelestialData() {
            RealtimeCelestialData earthData = SolarSystemCelestial.CalculateCelestialXYZ(systemDateTime, CelestialObject.Earth);

            Vector3 sunRotation = CalculateSunRotation(earthData.position);
            Vector3 moonDirection = CalculateMoonDirection();
            Vector3 starsRotation = GetStarsRotation();

            planetRotations[0] = CalculatePlanetDirection(CelestialObject.Mercury, earthData.position);
            planetRotations[1] = CalculatePlanetDirection(CelestialObject.Venus, earthData.position);
            planetRotations[2] = CalculatePlanetDirection(CelestialObject.Mars, earthData.position);
            planetRotations[3] = CalculatePlanetDirection(CelestialObject.Jupiter, earthData.position);
            planetRotations[4] = CalculatePlanetDirection(CelestialObject.Saturn, earthData.position);
            planetRotations[5] = CalculatePlanetDirection(CelestialObject.Uranus, earthData.position);
            planetRotations[6] = CalculatePlanetDirection(CelestialObject.Neptune, earthData.position);
            planetRotations[7] = CalculatePlanetDirection(CelestialObject.Pluto, earthData.position);

            material.SetVector("_MoonDirection", moonDirection);
            material.SetVector("_StarsRotation", starsRotation);
            material.SetVectorArray("_PlanetDirections", planetRotations);
            material.SetColorArray("_PlanetColours", planetColours);
            material.SetFloatArray("_PlanetSizes", planetSizes);
            material.SetMatrix("_GlobalToLocalMoonMat", GetMoonTextureMatrix(moonDirection));
            material.SetMatrix("_StarsRotationMatrix", GetStarsMatrix(starsRotation));

            material.SetVector("_SunDirection", SphericalToNormalizedCartesian(sunRotation.x, sunRotation.y));

            if (sun) {
                sun.transform.eulerAngles = sunRotation;
            }
        }*/

        /// <summary>
        /// Calculates the matrix used to rotate the stars.
        /// </summary>
        /// <param name="starsRotation">The rotation to apply to the stars.</param>
        /// <returns>The star's rotation matrix.</returns>
        private Matrix4x4 GetStarsMatrix(Vector2 starsRotation) {
            float cosLat = Mathf.Cos(-starsRotation.x);
            float sinLat = Mathf.Sin(-starsRotation.x);
            float cosLocalSiderealTime = Mathf.Cos(starsRotation.y);
            float sinLocalSiderealTime = Mathf.Sin(starsRotation.y);

            Vector4 row1 = new Vector4(1, 0, 0, 0);
            Vector4 row2 = new Vector4(0, cosLat, -sinLat, 0);
            Vector4 row3 = new Vector4(0, sinLat, cosLat, 0);
            Vector4 row4 = Vector4.zero;
            Matrix4x4 rotationX = new Matrix4x4(row1, row2, row3, row4);

            row1 = new Vector4(cosLocalSiderealTime, 0, sinLocalSiderealTime, 0);
            row2 = new Vector4(0, 1, 0, 0);
            row3 = new Vector4(-sinLocalSiderealTime, 0, cosLocalSiderealTime, 0);
            row4 = Vector4.zero;
            Matrix4x4 rotationY = new Matrix4x4(row1, row2, row3, row4);

            Matrix4x4 rotationMatrix = rotationY * rotationX;

            return rotationMatrix;
        }

        /// <summary>
        /// Calculates the matrix used to rotate the moons texture.
        /// </summary>
        /// <param name="moonDirection">The direction of the moon.</param>
        /// <returns>The moon's rotation matrix.</returns>
        private Matrix4x4 GetMoonTextureMatrix(Vector3 moonDirection) {
            Quaternion lookRotation = Quaternion.LookRotation(moonDirection);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(lookRotation);
            Quaternion roll = Quaternion.AngleAxis(-(float)latitude - 90, moonDirection);
            Matrix4x4 rotation = rotationMatrix.transpose * Matrix4x4.Rotate(roll);
            return rotation;
        }

        /// <summary>
        /// Calculates the suns altitude and azimuth rotation.
        /// </summary>
        /// <param name="earthXYZ">The current position of Earth.</param>
        /// <returns>The suns euler angle rotation.</returns>
        /*private Vector3 CalculateSunRotation(HeliocentricEclipticalCoordinates earthXYZ) {
            RealtimeCelestialData sunData = new RealtimeCelestialData();
            sunData.position.x = sunData.position.y = sunData.position.z = 0;
            sunData = SolarSystemCelestial.CalculateCelestialObjectRotation(systemDateTime, latitude, longitude, earthXYZ, sunData);
            UpdateCelestialData(CelestialObject.Sun, sunData);
            return new Vector3((float)sunData.altitude, (float)sunData.azimuth, 0);
        }*/

        /// <summary>
        /// Calculates the direction of the moon in the skybox.
        /// </summary>
        /// <returns>The moon's direction in the skybox.</returns>
       /* private Vector3 CalculateMoonDirection() {
            RealtimeCelestialData moonData = SolarSystemCelestial.CalculateMoonRotation(systemDateTime, latitude, longitude);
            UpdateCelestialData(CelestialObject.Moon, moonData);
            return SphericalToNormalizedCartesian((float)moonData.altitude, (float)moonData.azimuth);
        }

        /// <summary>
        /// Calculates the direction of a planet in the skybox.
        /// </summary>
        /// <param name="planet">The planet to calculate.</param>
        /// <param name="earthXYZ">The Earth's current position.</param>
        /// <returns>The direction of the planet.</returns>
        private Vector3 CalculatePlanetDirection(CelestialObject planet, HeliocentricEclipticalCoordinates earthXYZ) {
            RealtimeCelestialData data = SolarSystemCelestial.CalculateCelestialXYZ(systemDateTime, planet);
            data = SolarSystemCelestial.CalculateCelestialObjectRotation(systemDateTime, latitude, longitude, earthXYZ, data);
            UpdateCelestialData(planet, data);
            return SphericalToNormalizedCartesian((float)data.altitude, (float)data.azimuth);
        }*/

        /// <summary>
        /// Gets the rotation of the stars.
        /// </summary>
        /// <returns>The rotation of the stars.</returns>
        /*private Vector3 GetStarsRotation() {
            return new Vector3((float)latitude * Mathf.Deg2Rad, (float)AstronomyCalculator.SiderealTime(longitude, systemDateTime) * Mathf.Deg2Rad, 0);
        }*/

        /// <summary>
        /// Updates the realtime information about a celestial object.
        /// </summary>
        /// <param name="celestialObject">The celestial object to update.</param>
        /// <param name="data">The data to set.</param>
       /* private void UpdateCelestialData(CelestialObject celestialObject, RealtimeCelestialData data) {
            if (celestialData.ContainsKey(celestialObject)) {
                celestialData[celestialObject] = data;
            }
            else {
                celestialData.Add(celestialObject, data);
            }
        }*/

        /// <summary>
        /// Converts spherical coordinates to normalized Cartesian coordinates.
        /// </summary>
        /// <param name="altitude">The altitude angle in degrees.</param>
        /// <param name="azimuth">The azimuth angle in degrees.</param>
        /// <returns>The normalized Cartesian coordinates.</returns>
        private Vector3 SphericalToNormalizedCartesian(float altitude, float azimuth) {
            altitude = Mathf.Deg2Rad * altitude;
            azimuth = Mathf.Deg2Rad * azimuth;
            float x = Mathf.Cos(altitude) * Mathf.Sin(azimuth);
            float y = Mathf.Sin(altitude) * -1;
            float z = Mathf.Cos(altitude) * Mathf.Cos(azimuth);
            return new Vector3(x, y, z);
        }
    }
}