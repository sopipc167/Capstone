using UnityEngine;

namespace PBRNightSky {
    /// <summary>
    /// A class to rotate the camera from mouse input.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class ScenePanner : MonoBehaviour {

        [SerializeField]
        [Tooltip("Controls the sensitivity of the mouse.")]
        private float rotationSpeed = 1f;
        [SerializeField]
        [Tooltip("Controls how much smoothing to apply to the mouse movement.")]
        private bool enableSmoothing = true;

        private float smoothX = 0f;
        private float smoothY = 0f;
        private readonly float smoothingValue = 0.01f;

        /// <summary>
        /// Locks the cursor when the game is started.
        /// </summary>
        private void Start() {
            //Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// Gets the mouse input and updates the rotation of the camera.
        /// </summary>
        private void Update() {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (!Input.GetMouseButton(0)) {
                mouseX = 0;
                mouseY = 0;
            }

            SmoothInput(mouseX, mouseY);
            RotateCamera(smoothX, smoothY);
        }

        /// <summary>
        /// Displays a tooltip in the corner of the screen describing the controls.
        /// </summary>
        private void OnGUI() {
            GUILayout.BeginArea(new Rect(10, 10, 600, 300));
            GUIStyle titleStyle = GUI.skin.label;
            titleStyle.fontSize *= 3;
            GUILayout.Label("Hold Left Mouse to Pan Around.", titleStyle);
            GUILayout.EndArea();
        }

        /// <summary>
        /// Smooths the input of the mouse using linear interpolation.
        /// </summary>
        /// <param name="mouseX">The current mouse x delta rotation.</param>
        /// <param name="mouseY">The current mouse y delta rotation.</param>
        private void SmoothInput(float mouseX, float mouseY) {
            if (enableSmoothing) {
                smoothX = Mathf.Lerp(smoothX, mouseX, smoothingValue);
                smoothY = Mathf.Lerp(smoothY, mouseY, smoothingValue);
            }
            else {
                smoothX = mouseX;
                smoothY = mouseY;
            }
        }

        /// <summary>
        /// Rotates the camera based on the provided delta rotations.
        /// </summary>
        /// <param name="mouseX">The current mouse x delta rotation.</param>
        /// <param name="mouseY">The current mouse y delta rotation.</param>
        private void RotateCamera(float mouseX, float mouseY) {

            float rotationX = mouseY * rotationSpeed;
            float rotationY = mouseX * rotationSpeed;

            rotationX = -rotationX;

            Vector3 currentRotation = transform.localEulerAngles;

            if (currentRotation.x > 180) {
                currentRotation.x -= 360;
            }
            if (currentRotation.y > 180) {
                currentRotation.y -= 360;
            }

            float newRotationX = currentRotation.x + rotationX;
            float newRotationY = currentRotation.y + rotationY;

            newRotationX = Mathf.Clamp(newRotationX, -90, 90);

            transform.localRotation = Quaternion.Euler(newRotationX, newRotationY, 0f);
        }
    }
}