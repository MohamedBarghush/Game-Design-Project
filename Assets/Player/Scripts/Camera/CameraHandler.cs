using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform player;
        private PlayerManager playerManager;
        private Vector2 lookInput;

        float rotAngleX = 0;
        float rotAngleY = 0;

        [SerializeField] private bool invertY = false;
        [SerializeField] private float minRotY = -35f;
        [SerializeField] private float maxRotY = 35f;
        // [SerializeField] private float cameraSmoothness = 0.2f;
        [SerializeField] private float sensitivityX = 2f;
        [SerializeField] private float sensitivityY = 1f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            // transform.rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
            rotAngleX = Camera.main.transform.eulerAngles.y;
            rotAngleY = Camera.main.transform.eulerAngles.x;

            playerManager = player.GetComponent<PlayerManager>();
        }

        // Update is called once per frame
        public void HandleCameraControl(bool isCameraLocked)
        {
            lookInput = playerManager.inputHandler.lookInput;
            float actualSensitivityX = sensitivityX;
            float actualSensitivityY = sensitivityY;

            // Adjust sensitivity based on control scheme
            if (TryGetComponent<PlayerInput>(out PlayerInput playerInput))
            {
                switch (playerInput.currentControlScheme)
                {
                    case "Gamepad":
                        actualSensitivityX /= 10f;
                        actualSensitivityY /= 10f;
                        break;
                    case "Keyboard&Mouse":
                        actualSensitivityX /= 10f;
                        actualSensitivityY /= 10f;
                        break;
                }
            }

            transform.position = player.position;
            rotAngleX += lookInput.x * actualSensitivityX;

            // Apply Y-axis inversion if enabled
            if (invertY)
                rotAngleY += lookInput.y * actualSensitivityY;
            else
                rotAngleY -= lookInput.y * actualSensitivityY;
            rotAngleY = Mathf.Clamp(rotAngleY, minRotY, maxRotY);
            // transform.rotation = Quaternion.Euler(rotAngleY, rotAngleX, 0f);
            Quaternion targetRotation = Quaternion.Euler(rotAngleY, rotAngleX, 0f);
            if (!isCameraLocked)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            else {
                // When camera is locked, update rotation to match the main camera
                Quaternion cameraRotation = Camera.main.transform.rotation;
                Vector3 cameraEuler = cameraRotation.eulerAngles;
                // Update our rotation angles to match the camera
                rotAngleX = cameraEuler.y;
                rotAngleY = cameraEuler.x;
                // Convert rotAngleY from 0-360 to -180 to 180 range if needed
                if (rotAngleY > 180)
                    rotAngleY -= 360;
                // Ensure it stays within our clamped range
                rotAngleY = Mathf.Clamp(rotAngleY, minRotY, maxRotY);
                // Apply the rotation directly
                transform.rotation = cameraRotation;
            }
        }
    }
}
